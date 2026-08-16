using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using StaffManagement.Data;

namespace StaffManagement.Core.TestData
{
    /// <summary>
    /// Test-mode only: creates a fresh local database from the EF model (accountmanagementContext has
    /// no migrations - this project uses Database.EnsureCreated instead) and adds the raw SQL objects
    /// the app calls directly (a Soundex scalar function + three stored procedures queried via
    /// FromSqlInterpolated) plus a minimal seed dataset so the app is actually clickable against an
    /// empty local SQL Server instead of the production DB.
    ///
    /// Not a production migration tool - idempotent (safe to call on every startup) and intended only
    /// for local development against a throwaway database. Wired up from Program.cs, gated to
    /// Development environment only.
    /// </summary>
    public static class TestDatabaseSeeder
    {
        public static async Task EnsureTestDatabaseAsync(accountmanagementContext db, ILogger logger)
        {
            await CreateSchemaWithoutForeignKeysAsync(db, logger);
            await EnsureSqlObjectsAsync(db, logger);
            await SeedReferenceAndPermissionDataAsync(db, logger);
        }

        /// <summary>
        /// Plain Database.EnsureCreatedAsync() fails on this model: the production schema was
        /// hand-authored via raw SQL rather than generated from this EF model, so a handful of
        /// FK/column-type pairings that were never round-tripped through EF's own schema generator
        /// don't actually satisfy it (e.g. EmployeeJob.fsfBuildingId is char(12) while its FK target
        /// hr.Building.fsfBuildingId is varchar(12) - "not the same data type"). Rather than chase
        /// each one individually (there could be more), this generates the full CREATE script and
        /// applies everything except FK constraints - a local test database doesn't need referential-
        /// integrity enforcement to be useful, just the tables/columns/procs the app actually queries.
        /// </summary>
        private static async Task CreateSchemaWithoutForeignKeysAsync(accountmanagementContext db, ILogger logger)
        {
            var creator = (RelationalDatabaseCreator)db.GetService<IDatabaseCreator>();
            if (!await creator.ExistsAsync())
            {
                await creator.CreateAsync();
            }

            // Check for a specific well-known table rather than just "does the database exist" - a
            // database that only got as far as being created (e.g. a prior run that died partway
            // through applying the script below) would otherwise be mistaken for a fully-schema'd one
            // forever after, since CreateAsync() only creates the empty database itself.
            var jobRecordTableExists = (await db.Database
                .SqlQuery<int?>($"SELECT OBJECT_ID('dbo.JobRecord', 'U') AS [Value]")
                .ToListAsync())
                .FirstOrDefault() != null;

            if (jobRecordTableExists)
            {
                return;
            }

            var script = db.Database.GenerateCreateScript();
            var statements = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (var rawStatement in statements)
            {
                if (string.IsNullOrWhiteSpace(rawStatement)) continue;

                if (Regex.IsMatch(rawStatement, @"ADD CONSTRAINT \[FK_", RegexOptions.IgnoreCase))
                {
                    continue; // whole statement is just adding one FK constraint - drop it entirely
                }

                // EF also inlines some FK constraints directly inside CREATE TABLE (...) rather than
                // as a separate ALTER TABLE statement - strip those clauses out too, or the CREATE
                // TABLE itself fails and cascades into every downstream table/index that depends on it.
                var statement = Regex.Replace(rawStatement,
                    @",\s*CONSTRAINT \[FK_[^\]]+\]\s*FOREIGN KEY\s*\([^)]*\)\s*REFERENCES\s*\[[^\]]+\]\.\[[^\]]+\]\s*\([^)]*\)(\s*ON DELETE \w+)?",
                    "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                try
                {
                    await db.Database.ExecuteSqlRawAsync(statement);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Test database schema statement failed, continuing: {Statement}",
                        statement.Length > 200 ? statement[..200] + "..." : statement);
                }
            }

            // Belt and braces: query sys.foreign_keys directly and drop anything the regex-based
            // stripping above missed (it did, at least once - a one-to-one Employee/Account
            // relationship configured with Employee as the dependent side produced a FK naming/
            // formatting shape the regexes above didn't match). Catches every remaining case
            // regardless of how EF happened to format it.
            var remainingForeignKeys = await db.Database
                .SqlQuery<string>($"SELECT QUOTENAME(s.name) + '.' + QUOTENAME(t.name) + '.' + QUOTENAME(fk.name) AS [Value] FROM sys.foreign_keys fk JOIN sys.tables t ON fk.parent_object_id = t.object_id JOIN sys.schemas s ON t.schema_id = s.schema_id")
                .ToListAsync();

            foreach (var qualifiedName in remainingForeignKeys)
            {
                var parts = qualifiedName.Split('.');
                var dropSql = $"ALTER TABLE {parts[0]}.{parts[1]} DROP CONSTRAINT {parts[2]}";
                try
                {
                    await db.Database.ExecuteSqlRawAsync(dropSql);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to drop leftover FK constraint {Fk}", qualifiedName);
                }
            }
        }

        private static async Task EnsureSqlObjectsAsync(accountmanagementContext db, ILogger logger)
        {
            async Task ExecAsync(string sql)
            {
                try
                {
                    await db.Database.ExecuteSqlRawAsync(sql);
                }
                catch (Exception ex)
                {
                    // A typo in one of these hand-written objects shouldn't take the whole app down -
                    // log it and let startup continue; whatever page needs the broken object will
                    // surface its own error when it actually queries it.
                    logger.LogWarning(ex, "Test database SQL object creation failed, continuing.");
                }
            }

            // PermissionRole.HasOne(d => d.PermissionLevelRole) (no matching WithOne/WithMany - see
            // the seeding method's comments) makes EF's schema generator guess this is one-to-one and
            // emit a synthetic UNIQUE index on PermissionLevelRole.PermissionRoleID. It isn't: a role
            // bundling multiple discrete permission levels (e.g. both "Can View System Review Page"
            // and "Can Impersonate" on one role) is exactly how GetUserLevelsAsync's query is written
            // to work, and Account only having at most one Permission row (a real, explicitly
            // configured 1:1 elsewhere in the model) would otherwise force every account down to a
            // single permission level. Drop the artifact so seeding (and any real usage) can attach
            // more than one level to a role.
            await ExecAsync(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PermissionLevelRole_PermissionRoleID')
                DROP INDEX IX_PermissionLevelRole_PermissionRoleID ON dbo.PermissionLevelRole");

            // dbo.Soundex: accountmanagementContext.Soundex(string) is mapped (HasDbFunction) to a
            // user-defined scalar function named "Soundex" in the default (dbo) schema - production
            // apparently defines its own wrapper around the T-SQL builtin SOUNDEX(). A local test DB
            // needs the same object to exist or every name-search query (Soundex-based fuzzy match)
            // throws "Invalid object name".
            await ExecAsync(@"
                IF OBJECT_ID('dbo.Soundex', 'FN') IS NULL
                EXEC('CREATE FUNCTION dbo.Soundex(@input nvarchar(4000)) RETURNS nvarchar(4000)
                AS BEGIN RETURN SOUNDEX(@input) END')");

            // dbo.JobRecord_Ordered: another ToView-mapped entity - ranks each account's job records
            // with Order = 1 as the "current" one (primary, active, most recent start), which is what
            // AccountRepository's search-grid query (BuildSearchQuery -> GetAccountsAsync/
            // GetContractorsAsync) joins against and filters to Order == 1. Silently crashed every
            // page's search grid with 0 results until this was added (Blazor Server swallowed the
            // ""Invalid object name"" into a circuit-level exception rather than surfacing it in the
            // UI - check the server log, not just the rendered page, when a grid looks suspiciously empty).
            await ExecAsync(@"
                IF OBJECT_ID('dbo.JobRecord_Ordered', 'V') IS NULL
                EXEC('CREATE VIEW dbo.JobRecord_Ordered AS
                SELECT
                    JobRecordId, SystemId, IsPrimary, IsActive, UnitId, BuildingId, SubstituteForId, TitleId, TitleGroupId,
                    StartDate, EndDate, IsPHRST, HR_RecordNumber AS HrRecordNumber, HR_WorkLocation AS HrWorkLocation,
                    HR_PersonnelCode AS HrPersonnelCode, HR_JobCode AS HrJobCode, HR_Primary AS HrPrimary, HR_Matched AS HrMatched,
                    NeedsReview, CreateDate, CreateUser, LastUpdate, LastUpdateUser,
                    ROW_NUMBER() OVER (PARTITION BY SystemId ORDER BY IsPrimary DESC, IsActive DESC, StartDate DESC) AS [Order]
                FROM dbo.JobRecord')");

            // dbo.AccountUnit: a keyed entity (SystemId/UnitId) mapped via ToView("AccountUnit", "dbo")
            // in the model - production has a real view here, but EF's schema generation only creates
            // tables, never the views that view-backed entities point at, so this needs to exist by
            // hand. It's queried on essentially every page load (CurrentUserService -> GetEmployeeSummaryAsync).
            await ExecAsync(@"
                IF OBJECT_ID('dbo.AccountUnit', 'V') IS NULL
                EXEC('CREATE VIEW dbo.AccountUnit AS
                SELECT SystemId, PrimaryUnitId AS UnitId FROM dbo.Account WHERE PrimaryUnitId IS NOT NULL')");

            await ExecAsync(@"
                IF OBJECT_ID('dbo.Select_TitleInfo_ByTitleGroup', 'P') IS NULL
                EXEC('CREATE PROCEDURE dbo.Select_TitleInfo_ByTitleGroup @titlegroupid int
                AS
                BEGIN
                    SELECT t.TitleId, t.Text, t.Active,
                           CAST(CASE WHEN EXISTS (
                               SELECT 1 FROM dbo.TitleGroup_Title tgt
                               WHERE tgt.TitleId = t.TitleId AND tgt.TitleGroupId = @titlegroupid AND tgt.Active = 1
                           ) THEN 1 ELSE 0 END AS int) AS Selected,
                           (SELECT COUNT(*) FROM dbo.JobRecord jr WHERE jr.TitleId = t.TitleId) AS Count
                    FROM dbo.Title t
                    ORDER BY t.Text
                END')");

            // dbo.AuditLogView: another view-backed entity (AuditLogView.cs, .ToView(""AuditLogView"")) -
            // production has a real view here, distinct from the Select_AuditLog stored procedure above
            // (same underlying dbo.AuditLog table, but queried directly via LINQ - see
            // AccountRepository.GetAuditLogForAccountAsync - rather than through a no-arg proc, so it
            // can be filtered by ReferenceTable/ReferenceID/Employee before hitting the database).
            await ExecAsync(@"
                IF OBJECT_ID('dbo.AuditLogView', 'V') IS NULL
                EXEC('CREATE VIEW dbo.AuditLogView AS
                SELECT
                    al.LogId, al.LogDate, al.ReferenceTable, al.ReferenceId AS ReferenceID,
                    ISNULL(au.FirstName + '' '' + au.LastName, '''') AS Employee,
                    CASE al.ActionTypeId
                        WHEN 1 THEN ''Create'' WHEN 2 THEN ''Edit'' WHEN 3 THEN ''Delete''
                        WHEN 4 THEN ''ReActivate'' WHEN 5 THEN ''Search'' ELSE ''Unknown''
                    END AS ActionType,
                    ISNULL(au.FirstName + '' '' + au.LastName, '''') AS ApplicationUser,
                    ISNULL(iu.FirstName + '' '' + iu.LastName, '''') AS ImpersonatedUser,
                    al.Comment
                FROM dbo.AuditLog al
                LEFT JOIN dbo.Account au ON au.SystemId = al.AppUser_SystemId
                LEFT JOIN dbo.Account iu ON iu.SystemId = al.AppUser_ImpersonatedUserId')");

            // Simplified for test mode: ApplicationUser/Employee are both derived from the acting
            // Account (AppUser_SystemId) since there is no separate "who is the audit subject" column
            // to resolve generically across every ReferenceTable value the app logs against.
            await ExecAsync(@"
                IF OBJECT_ID('dbo.Select_AuditLog', 'P') IS NULL
                EXEC('CREATE PROCEDURE dbo.Select_AuditLog
                AS
                BEGIN
                    SELECT
                        al.LogId, al.LogDate, al.ReferenceTable, al.ReferenceId AS ReferenceID,
                        ISNULL(au.FirstName + '' '' + au.LastName, '''') AS Employee,
                        CASE al.ActionTypeId
                            WHEN 1 THEN ''Create'' WHEN 2 THEN ''Edit'' WHEN 3 THEN ''Delete''
                            WHEN 4 THEN ''ReActivate'' WHEN 5 THEN ''Search'' ELSE ''Unknown''
                        END AS ActionType,
                        ISNULL(au.FirstName + '' '' + au.LastName, '''') AS ApplicationUser,
                        ISNULL(iu.FirstName + '' '' + iu.LastName, '''') AS ImpersonatedUser,
                        al.Comment
                    FROM dbo.AuditLog al
                    LEFT JOIN dbo.Account au ON au.SystemId = al.AppUser_SystemId
                    LEFT JOIN dbo.Account iu ON iu.SystemId = al.AppUser_ImpersonatedUserId
                    ORDER BY al.LogId DESC
                END')");

            // Simplified for test mode: only "New" (HR employee with no linked AD account) and "Name"
            // (linked account whose name differs from the HR record) are computed for real - "Job"/
            // "End" reconciliation depends on production PHRST feed semantics with no local equivalent
            // to reproduce. Good enough to exercise the mismatched-employees UI without crashing.
            await ExecAsync(@"
                IF OBJECT_ID('dbo.Select_PHRST_Mismatched_Employees', 'P') IS NULL
                EXEC('CREATE PROCEDURE dbo.Select_PHRST_Mismatched_Employees @systemId int
                AS
                BEGIN
                    SELECT
                        a.SystemId, e.EmployeeId, jr.JobRecordId,
                        a.FirstName AS AccountFirstName, a.MiddleName AS AccountMiddleName, a.LastName AS AccountLastName, a.SuffixName AS AccountSuffixName,
                        e.FirstName AS EmployeeFirstName, e.MiddleName AS EmployeeMiddleName, e.LastName AS EmployeeLastName, CAST(NULL AS varchar(50)) AS EmployeeSuffixName,
                        jr.StartDate, jr.EndDate,
                        jr.UnitId AS jrUnitId, jr.BuildingId AS jrBuildingId, jr.HR_WorkLocation AS jrHR_WorkLocation, jr.HR_PersonnelCode AS jrHR_PersonnelCode, jr.HR_JobCode AS jrHR_JobCode, jr.IsActive AS jrIsActive, jr.HR_Primary AS jrHR_Primary,
                        e.UnitId AS ejUnitId, CAST(NULL AS int) AS ejBuildingId, e.fsfBuildingId AS ejWorkLocation, e.personnel_code AS ejPersonnel_code, e.jobcode AS ejJobcode, CAST(0 AS bit) AS ejIsPrimary,
                        ''New'' AS Reason,
                        u.Name AS Unit, CAST(NULL AS varchar(50)) AS Building, e.personnel_code AS PersonnelCode, CAST(e.jobcode AS varchar(20)) AS JobCode, e.JobTitle AS JobTitle,
                        GETDATE() AS EffectiveDate
                    FROM hr.Employee e
                    LEFT JOIN dbo.Account a ON a.EmployeeId = e.EmployeeId
                    LEFT JOIN dbo.JobRecord jr ON jr.SystemId = a.SystemId
                    LEFT JOIN dbo.Unit u ON u.UnitId = e.UnitId
                    WHERE a.SystemId IS NULL AND @systemId = 0

                    UNION ALL

                    SELECT
                        a.SystemId, e.EmployeeId, jr.JobRecordId,
                        a.FirstName, a.MiddleName, a.LastName, a.SuffixName,
                        e.FirstName, e.MiddleName, e.LastName, CAST(NULL AS varchar(50)),
                        jr.StartDate, jr.EndDate,
                        jr.UnitId, jr.BuildingId, jr.HR_WorkLocation, jr.HR_PersonnelCode, jr.HR_JobCode, jr.IsActive, jr.HR_Primary,
                        e.UnitId, CAST(NULL AS int), e.fsfBuildingId, e.personnel_code, e.jobcode, CAST(0 AS bit),
                        ''Name'',
                        u.Name, CAST(NULL AS varchar(50)), e.personnel_code, CAST(e.jobcode AS varchar(20)), e.JobTitle,
                        GETDATE()
                    FROM hr.Employee e
                    INNER JOIN dbo.Account a ON a.EmployeeId = e.EmployeeId
                    LEFT JOIN dbo.JobRecord jr ON jr.SystemId = a.SystemId AND jr.IsPrimary = 1
                    LEFT JOIN dbo.Unit u ON u.UnitId = e.UnitId
                    WHERE (a.FirstName <> e.FirstName OR a.LastName <> e.LastName)
                      AND (@systemId = 0 OR a.SystemId = @systemId)
                END')");
        }

        private static async Task SeedReferenceAndPermissionDataAsync(accountmanagementContext db, ILogger logger)
        {
            if (await db.Units.AnyAsync())
            {
                return; // already seeded
            }

            try
            {
                var building = new Building { BuildingId = 1, Name = "Test Building", ShortName = "TB", StreetAddress = "1 Test Way", City = "Testville", State = "DE", Zip = "00000" };
                db.Buildings.Add(building);

                var unit = new Unit
                {
                    UnitId = 1,
                    Building = building,
                    Name = "Test Department",
                    ShortName = "TEST",
                    Adname = "TESTDEPT",
                    UseGenericAdgroups = true,
                    HomeDirServer = ""
                };
                db.Units.Add(unit);

                var employee = new Employee
                {
                    EmployeeId = 1,
                    FirstName = "Test",
                    MiddleName = "",
                    LastName = "Employee",
                    SuffixName = "",
                    JobTitle = "Test Job Title",
                    FsfBuildingId = "",
                    PersonnelCode = "T1",
                    Jobcode = 1,
                    EmployeeStatus = "A",
                    UnitId = unit.UnitId,
                    IsDeleted = false,
                    LastUpdate = DateTime.Now
                };
                db.Employees.Add(employee);

                var jobcode = new Jobcode { JobcodeId = 1, Description = "Test Job Code", OfficialDescription = "Test Job Code" };
                db.Jobcodes.Add(jobcode);

                var personnelCode = new PersonnelCode { Personnel_Code = "T1", Description = "Test Personnel Code" };
                db.PersonnelCodes.Add(personnelCode);

                var accountType = new AccountType { Description = "Employee", IsStaff = true };
                db.AccountTypes.Add(accountType);

                var titleGroup = new TitleGroup { Description = "Test Title Group", Active = true };
                db.TitleGroups.Add(titleGroup);

                var title = new Title { Text = "Test Title", Active = true };
                db.Titles.Add(title);

                db.TitleGroup_Titles.Add(new TitleGroup_Title { TitleGroup = titleGroup, Title = title, Active = true });

                // PermissionLevel/PermissionRole/PermissionLevelRole are inserted via raw SQL, not
                // db.Add(...): the legacy model itself has an incomplete relationship configuration
                // here (PermissionRole.HasOne(d => d.PermissionLevelRole) has no matching
                // WithOne/WithMany or FK, and PermissionLevel has no relationship config for it at
                // all), which is harmless for the read-only queries the app actually runs at runtime
                // but makes EF's change tracker throw ("principal entity in the relationship is not
                // known") the moment any of these three types are added to a SaveChanges graph. Not a
                // seeding bug - a pre-existing gap in accountmanagementContext.cs that production
                // never hits because this reference data is never inserted through EF.
                async Task<int> InsertAndGetIdAsync(FormattableString sql) =>
                    (await db.Database.SqlQuery<int>(sql).ToListAsync()).First();

                // PermissionLevelID is, oddly, NOT an identity column in the generated schema (unlike
                // PermissionRoleID/PermissionLevelRoleID, which are) - another symptom of the same
                // incomplete relationship configuration noted above confusing EF's key-generation
                // convention for this one table. Explicit IDs needed here.
                //
                // Test Admin is granted every permission string actually checked anywhere in the
                // ported Blazor app or the legacy source (grep for IsInLevel("...")) - not just the
                // two the app currently reads (Can View System Review Page/Can Impersonate) - so the
                // local test account exercises every permission-gated code path, including the
                // Nickname/AlternateName/UsernameOverride gates on Employees/Edit that aren't wired up
                // in the port yet but will read this same role once they are.
                var allPermissionLevels = new[]
                {
                    "Can View System Review Page",
                    "Can Impersonate",
                    "Can Set Nickname",
                    "Can Set Alternate Name",
                    "Can Set Username Override"
                };

                var permissionRoleId = await InsertAndGetIdAsync(
                    $"INSERT INTO dbo.PermissionRole (PermissionRole, Active) OUTPUT INSERTED.PermissionRoleID VALUES ({"Test Admin"}, 1)");

                var permissionLevelId = 1;
                foreach (var levelName in allPermissionLevels)
                {
                    var id = permissionLevelId++;
                    await db.Database.ExecuteSqlInterpolatedAsync(
                        $"INSERT INTO dbo.PermissionLevel (PermissionLevelID, PermissionLevel, Active) VALUES ({id}, {levelName}, 1)");
                    await db.Database.ExecuteSqlInterpolatedAsync(
                        $"INSERT INTO dbo.PermissionLevelRole (PermissionRoleID, PermissionLevelID, Active) VALUES ({permissionRoleId}, {id}, 1)");
                }

                // Seeded so whoever runs this locally (any Windows account) is automatically resolved
                // as a fully-permissioned test admin - CurrentUserService strips the DOMAIN\ or
                // MACHINE\ prefix the same way, so a bare username here matches either way.
                var currentWindowsUser = Environment.UserName;

                var adUser = new User
                {
                    ObjectGuid = Guid.NewGuid(),
                    SamaccountName = currentWindowsUser,
                    DistinguishedName = $"CN={currentWindowsUser},DC=test,DC=local",
                    CommonName = currentWindowsUser,
                    Name = currentWindowsUser,
                    GivenName = "Test",
                    Surname = "Admin",
                    DisplayName = "Test Admin",
                    JobTitle = "Test Admin",
                    IsDisabled = false,
                    IsDeleted = false,
                    WhenChanged = DateTime.Now,
                    WhenCreated = DateTime.Now,
                    Usnchanged = 1
                };
                // User's own parameterless constructor eagerly creates a placeholder
                // `OrganizationalUnit = new OrganizationalUnit()` with no ObjectGuid set - left in
                // place, EF tries to insert that dummy object too and fails ("primary key property
                // 'ObjectGuid' is null"). OrganizationalUnitGuid is nullable and unused here, so drop it.
                adUser.OrganizationalUnit = null!;
                db.Users.Add(adUser);

                // SystemId, despite the [DatabaseGenerated(Identity)] attribute on Account.SystemId
                // itself, is NOT actually an identity column in the generated schema: Permission's
                // explicit HasOne(Account).WithOne(Permission).HasPrincipalKey<Permission>(SystemId)
                // .HasForeignKey<Account>(SystemId) configuration makes Account.SystemId a dependent
                // FK of a one-to-one relationship (Permission is the "principal" side) - EF disables
                // independent value generation for FK properties in 1:1 relationships, so it has to be
                // supplied explicitly here rather than left to auto-generate.
                // Employee isn't linked via navigation here - Employee.EmployeeId and
                // Account.EmployeeId are configured as FKs pointing at EACH OTHER (the leftover
                // FK_Employee_Account_EmployeeId dropped above, plus the "normal" Account -> Employee
                // direction), a genuine circular relationship in the model. EF's SaveChanges dependency
                // ordering detects the cycle from the model metadata itself regardless of whether the
                // DB-level constraint exists, so the link is set as a plain int after both rows exist
                // instead (see below), breaking the cycle for save-ordering purposes.
                var account = new Account
                {
                    SystemId = 1,
                    FirstName = "Test",
                    LastName = "Admin",
                    JobTitle = "Test Admin",
                    User = adUser,
                    PrimaryUnit = unit,
                    AccountType = accountType,
                    Source = "AD",
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    CreateUser = "seed",
                    LastUpdate = DateTime.Now,
                    LastUpdateUser = "seed"
                };
                db.Accounts.Add(account);

                db.JobRecord.Add(new JobRecord
                {
                    Account = account,
                    Unit = unit,
                    Title = title,
                    TitleGroup = titleGroup,
                    IsPrimary = true,
                    IsActive = true,
                    IsPHRST = true,
                    StartDate = DateTime.Now.AddYears(-1),
                    HrPersonnelCode = personnelCode.Personnel_Code,
                    HrJobCode = jobcode.JobcodeId,
                    NeedsReview = false,
                    CreateDate = DateTime.Now,
                    CreateUser = "seed",
                    LastUpdate = DateTime.Now,
                    LastUpdateUser = "seed"
                });

                // Second account, deliberately out of sync with its live hr.EmployeeJob row, so the
                // Employees/Edit PHRST reconciliation card (ComputePhrstMismatches) has real mismatch
                // data to render against instead of an always-empty/no-op path. The JobRecord below
                // snapshots an older personnel/job code than what the EmployeeJob row now reports.
                var jobcode2 = new Jobcode { JobcodeId = 2, Description = "Mismatch Job Code", OfficialDescription = "Mismatch Job Code" };
                db.Jobcodes.Add(jobcode2);

                var personnelCode2 = new PersonnelCode { Personnel_Code = "T2", Description = "Mismatch Personnel Code" };
                db.PersonnelCodes.Add(personnelCode2);

                var hrBuilding2 = new hr_Building { FsfBuildingId = "TB2", District = "TS", WorkLocation = "TB2", Description = "Test Building 2", ShortDescription = "TB2" };
                db.Hr_Buildings.Add(hrBuilding2);

                // Saved on their own first: hr_Building.Employee is (oddly) configured as a FK back to
                // Employee via the shared FsfBuildingId column, which - combined with Employee's own
                // FKs to Jobcode/hr_Building and EmployeeJob's FK to Jobcode - forms a genuine insert-
                // order cycle if all four are added to the change tracker in one batch (Jobcode ->
                // Employee -> hr_Building -> EmployeeJob -> Jobcode). Persisting the lookup rows first
                // breaks it, same idea as the Employee/Account circular-FK workaround below.
                await db.SaveChangesAsync();

                var mismatchEmployee = new Employee
                {
                    EmployeeId = 2,
                    FirstName = "Mismatch",
                    MiddleName = "",
                    LastName = "Case",
                    SuffixName = "",
                    JobTitle = "Mismatch Job Title",
                    FsfBuildingId = hrBuilding2.FsfBuildingId,
                    PersonnelCode = personnelCode2.Personnel_Code,
                    Jobcode = jobcode2.JobcodeId,
                    EmployeeStatus = "A",
                    UnitId = unit.UnitId,
                    IsDeleted = false,
                    LastUpdate = DateTime.Now
                };
                db.Employees.Add(mismatchEmployee);

                db.EmployeeJobs.Add(new EmployeeJob
                {
                    EmployeeId = mismatchEmployee.EmployeeId,
                    JobId = 1,
                    FsfBuildingId = hrBuilding2.FsfBuildingId,
                    IsPrimary = true,
                    PersonnelCode = personnelCode2.Personnel_Code,
                    Jobcode = jobcode2.JobcodeId,
                    UnitId = unit.UnitId,
                    EffectiveDate = DateTime.Now.AddMonths(-1)
                });

                var mismatchAccount = new Account
                {
                    SystemId = 2,
                    FirstName = "Mismatch",
                    LastName = "Case",
                    JobTitle = "Mismatch Job Title",
                    PrimaryUnit = unit,
                    AccountType = accountType,
                    Source = "AD",
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    CreateUser = "seed",
                    LastUpdate = DateTime.Now,
                    LastUpdateUser = "seed"
                };
                db.Accounts.Add(mismatchAccount);

                db.JobRecord.Add(new JobRecord
                {
                    Account = mismatchAccount,
                    Unit = unit,
                    Title = title,
                    TitleGroup = titleGroup,
                    IsPrimary = true,
                    IsActive = true,
                    IsPHRST = true,
                    StartDate = DateTime.Now.AddYears(-1),
                    // Stale on purpose - the live EmployeeJob above reports "TB2"/"T2"/2.
                    HrWorkLocation = "OLD-BUILDING",
                    HrPersonnelCode = personnelCode.Personnel_Code,
                    HrJobCode = jobcode.JobcodeId,
                    NeedsReview = false,
                    CreateDate = DateTime.Now,
                    CreateUser = "seed",
                    LastUpdate = DateTime.Now,
                    LastUpdateUser = "seed"
                });

                await db.SaveChangesAsync();

                account.EmployeeId = employee.EmployeeId;
                mismatchAccount.EmployeeId = mismatchEmployee.EmployeeId;
                await db.SaveChangesAsync();

                // Permission links the now-generated Account.SystemId to the PermissionRole inserted
                // earlier - raw SQL again, consistent with (and to stay well clear of) the
                // PermissionRole/PermissionLevel relationship gap described above.
                await db.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO dbo.Permission (SystemId, PermissionRoleID, Active) VALUES ({account.SystemId}, {permissionRoleId}, 1)");

                logger.LogInformation(
                    "Test database seeded: local admin account for Windows user '{User}' with full permissions, plus a second account (SystemId 2, 'Mismatch Case') with a deliberate PHRST mismatch for exercising the Employees/Edit reconciliation card.",
                    currentWindowsUser);
            }
            catch (Exception ex)
            {
                // Seeding is a convenience, not a hard requirement - the app should still start (and
                // show empty grids) against a schema-only database if seeding fails for some reason.
                logger.LogWarning(ex, "Test database seeding failed; continuing with schema-only database.");
            }
        }
    }
}
