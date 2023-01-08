using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
#nullable disable

namespace StaffManagement.Data
{
    /// <summary>
    /// Represents the database session , with declarations for dbSets and the overriding of OnModelCreating to detail 
    /// database relationships and properties
    /// </summary>
    public partial class accountmanagementContext : DbContext
    {
        /// <summary>
        /// Add sql log to debug output
        /// </summary>
        public static readonly ILoggerFactory _loggerFactory
            = LoggerFactory.Create(builder => { builder.AddDebug(); });


        /// <summary>
        /// Soundex function will broaden a search to match strings that sound similar rather than partially-spelled exactly the same
        /// (using recommended method for calling a sql function on the server. )
        /// </summary>
        /// <param name="soundex"></param>
        /// <returns></returns>
        public string Soundex(string soundex)
            => throw new NotSupportedException();


        public accountmanagementContext(DbContextOptions<accountmanagementContext> options)
            : base(options)
        {


        }

        #region DbSets
        /// <summary>
        /// Tables, Stored Procedures, and Views can be represented as DbSets with a corresponding class in "Models/Data" that
        /// defines the fields, etc. along with overridden OnModelCreating
        /// </summary>
        // Tables, SPs, and Views
        public virtual DbSet<Account> Accounts { get; set; }

        public virtual DbSet<AccountType> AccountTypes { get; set; }
        public virtual DbSet<AccountUnit> AccountUnits { get; set; }

        public virtual DbSet<Action> Actions { get; set; }

        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<hr_Building> Hr_Buildings { get; set; }
        public virtual DbSet<sis_Building> Sis_Buildings { get; set; }
        public virtual DbSet<BuildingSource> BuildingSources { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<ContractAdditionalUnit> ContractAdditionalUnits { get; set; }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeLatestJob> EmployeeLatestJob { get; set; }

        public virtual DbSet<Export_Employee> Export_Employees { get; set; }
        public virtual DbSet<EmployeeAdditionalUnit> EmployeeAdditionalUnits { get; set; }
        public virtual DbSet<EmployeeJob> EmployeeJobs { get; set; }
        public virtual DbSet<EmployeeJobSource> EmployeeJobSources { get; set; }
        public virtual DbSet<EmployeeSource> EmployeeSources { get; set; }
        public virtual DbSet<ExchangeEmail> ExchangeEmails { get; set; }
        public virtual DbSet<FieldDatum> FieldData { get; set; }
        public virtual DbSet<FieldGroup> FieldGroups { get; set; }
        public virtual DbSet<FieldProperty> FieldProperties { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupAssignment> GroupAssignments { get; set; }
        public virtual DbSet<GroupAssignmentSource> GroupAssignmentSources { get; set; }

        public virtual DbSet<GroupExclusionList> GroupExclusionLists { get; set; }
        public virtual DbSet<GroupMembership> GroupMemberships { get; set; }
        public virtual DbSet<GroupMembershipRule> GroupMembershipRules { get; set; }
        public virtual DbSet<JobTitle> JobTitles { get; set; }


        public virtual DbSet<Jobcode> Jobcodes { get; set; }
        public virtual DbSet<JobcodeSource> JobcodeSources { get; set; }
        public virtual DbSet<JobRecord> JobRecord { get; set; }

        public virtual DbSet<JobRecord_Ordered> JobRecord_Ordered { get; set; }


        public virtual DbSet<OrganizationalUnit> OrganizationalUnits { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<PermissionRole> PermissionRole { get; set; }
        public virtual DbSet<PermissionLevelRole> PermissionLevelRole { get; set; }
        public virtual DbSet<PermissionLevel> PermissionLevel { get; set; }

        public virtual DbSet<PersonnelCode> PersonnelCodes { get; set; }
        public virtual DbSet<PersonnelcodeSource> PersonnelcodeSources { get; set; }
        public virtual DbSet<Phone> Phones { get; set; }
        public virtual DbSet<Export_StaffBuilding> Export_StaffBuildings { get; set; }
        public virtual DbSet<Sis_StaffBuilding> Sis_StaffBuildings { get; set; }
        //public virtual DbSet<SiteText> SiteText { get; set; }

        public virtual DbSet<StaffBuildingSource> StaffBuildingSources { get; set; }
        public virtual DbSet<StaffSource> StaffSources { get; set; }

        public virtual DbSet<StaffUnit> StaffUnits { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentAccount> StudentAccounts { get; set; }
        public virtual DbSet<StudentBuilding> StudentBuildings { get; set; }
        public virtual DbSet<StudentSource> StudentSources { get; set; }

        public virtual DbSet<Title> Titles { get; set; }
        public virtual DbSet<TitleGroup> TitleGroups { get; set; }
        public virtual DbSet<TitleGroup_Title> TitleGroup_Titles { get; set; }

        public virtual DbSet<Unit> Units { get; set; }

        public virtual DbSet<UnitBuilding> UnitBuildings { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Contractor_staff> staff { get; set; }
        public virtual DbSet<Sis_Staff> Sis_Staff { get; set; }

        //public virtual DbSet<StaffManagementView> StaffManagementView { get; set; }
        //public virtual DbSet<StaffManagementView_Permission> StaffManagementView_Permission { get; set; }
        //public virtual DbSet<StaffManagementView_Unit> StaffManagementView_Unit { get; set; }

        //public virtual DbSet<NeedsReview> NeedsReview { get; set; }

        public virtual DbSet<AuditLogView> AuditLogView { get; set; }

        public virtual DbSet<StaffGroupAssociation> StaffGroupAssociations { get; set; }

        public virtual DbSet<StaffGroup> StaffGroups { get; set; }



        // Stored Procedure Classes
        // public virtual DbSet<Select_StaffManagementView_Employees> Select_StaffManagementView_Employees { get; set; }


        public virtual DbSet<Select_Employee_AssignedGroups> Select_Employee_AssignedGroups { get; set; }

        public virtual DbSet<Select_Employee_StaffGroups> Select_Employee_StaffGroups { get; set; }


        public virtual DbSet<Select_PHRST_Mismatched_Employees> Select_PHRST_Mismatched_Employees { get; set; }

        public virtual DbSet<Select_TitleInfo_ByTitleGroup> Select_TitleInfo_ByTitleGroup { get; set; }


        // public virtual DbSet<Select_Available_JobTitles> Select_Available_JobTitles { get; set; }

        //public virtual DbSet<EmployeeJobTitleAssignmentInfo> EmployeeJobTitleAssignmentInfo { get; set; }

        public virtual DbSet<Select_StaffGroupAssociation_Employees> Select_StaffGroupAssociation_Employees { get; set; }
        public virtual DbSet<Select_Group_Employees> Select_Group_Employees { get; set; }

        public virtual DbSet<Select_AuditLog> Select_AuditLog { get; set; }



        #endregion


        /// <summary>
        /// Use logger factory declared as property above.  Other configurations set in Startup.cs for setting sql connection string and allowing all detail in the debug log.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseLoggerFactory(_loggerFactory);


        //See Startup.cs
        ////services.AddDbContext<accountmanagementContext>
        ////        (options => options
        ////        .UseSqlServer(Configuration.GetConnectionString(Configuration.GetConnectionString("ActiveDB"))).EnableSensitiveDataLogging() );




        #region OnModelCreating
        /// <summary>
        /// OnModelCreating allows you to override settings as each database object is created.  Includes foreign key relationships that are easier to set here rather than in the object classes.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // link sql function to Soundex method declared above
            modelBuilder.HasDbFunction(typeof(accountmanagementContext).GetMethod(nameof(Soundex), new[] { typeof(string) }))
                .HasName("Soundex");

            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            #region Account
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.SystemId)
                    .HasName("PK_dbo_Account");

                entity.ToTable("Account");

                entity.Property(e => e.AdobjectGuid).HasColumnName("ADObjectGuid");



                entity.Property(e => e.AlternateFirstName).HasMaxLength(255);

                entity.Property(e => e.AlternateLastName).HasMaxLength(255);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.ESchoolStaffId).HasColumnName("eSchoolStaffId");

                entity.Property(e => e.FirstName).HasMaxLength(255);

                entity.Property(e => e.JobTitle).HasMaxLength(255);

                entity.Property(e => e.LastName).HasMaxLength(255);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdateUser)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName).HasMaxLength(255);

                entity.Property(e => e.Nickname).HasMaxLength(255);

                entity.Property(e => e.Source)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SuffixName).HasMaxLength(255);

                entity.Property(e => e.UsernameOverride)
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.HasOne(d => d.PrimaryUnit)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.PrimaryUnitId)
                    .HasConstraintName("FK_dbo_Account_dbo_Unit");

                entity.HasOne(d => d.Permission)
                   .WithOne(p => p.Account)
                   .HasPrincipalKey<Permission>(p => p.SystemId)
                   ;

                //entity.HasOne(d => d.JobTitleAssignment)
                //  .WithOne(p => p.Account)
                //  .HasPrincipalKey<JobTitleAssignment>(p => p.SystemId)
                //  ;


                entity.HasOne(d => d.AccountUnit)
                       .WithOne(p => p.Account)
                       ;



                //entity.HasOne(e => e.Employee)
                //.WithOne(p => p.Account)
                //.HasPrincipalKey<Account>(a => a.EmployeeId)
                //.HasForeignKey<Employee>(a => a.EmployeeId);



            });

            #endregion

            modelBuilder.Entity<AccountType>(entity =>
            {
                // entity.HasNoKey();

                entity.ToTable("AccountType", "dbo");


            });



            modelBuilder.Entity<AccountUnit>(entity =>
            {
                // entity.HasNoKey();

                entity.ToView("AccountUnit", "dbo");


            });

            modelBuilder.Entity<EmployeeLatestJob>(entity =>
            {


                entity.ToView("EmployeeLatestJob", "dbo");


            });


            #region Action
            modelBuilder.Entity<Action>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK_dbo_Actions");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Secret)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);
            });
            #endregion

            #region AuditLog

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PK_dbo_AuditLog");

                entity.ToTable("AuditLog", "dbo");

                //entity.Property(e => e.Activity)
                //    .IsRequired()
                //    .IsUnicode(false);

                //entity.Property(e => e.ActivityCode)
                //    .IsRequired()
                //    .HasMaxLength(5)
                //    .IsUnicode(false)
                //    .IsFixedLength(true);

                //entity.Property(e => e.EventDate).HasColumnType("datetime");

                //entity.Property(e => e.Initiator)
                //    .IsRequired()
                //    .HasMaxLength(100)
                //    .IsUnicode(false);


            });
            #endregion

            #region Building
            modelBuilder.Entity<Building>(entity =>
            {
                entity.ToTable("Building");

                entity.Property(e => e.BuildingId).ValueGeneratedNever();

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FsfBuildingId)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("fsfBuildingId");

                entity.Property(e => e.GradeBand)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SisBuildingId).HasColumnName("sisBuildingId");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StreetAddress)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.FsfBuilding)
                    .WithMany(p => p.Buildings)
                    .HasForeignKey(d => d.FsfBuildingId)
                    .HasConstraintName("FK_dbo_Building_hr_Building");

                entity.HasOne(d => d.SisBuilding)
                    .WithMany(p => p.Buildings)
                    .HasForeignKey(d => d.SisBuildingId)
                    .HasConstraintName("FK_dbo_Building_sis_Building");
            });
            #endregion

            modelBuilder.Entity<hr_Building>(entity =>
            {
                entity.HasKey(e => e.FsfBuildingId)
                    .HasName("PK_hr_Building");

                entity.ToTable("Building", "hr");

                entity.Property(e => e.FsfBuildingId)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("fsfBuildingId");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.District)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("district")
                    .IsFixedLength(true);

                entity.Property(e => e.ShortDescription)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("short_description");

                entity.Property(e => e.WorkLocation)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("work_location")
                    .IsFixedLength(true);

                entity.HasOne(x => x.Employee)
                    .WithOne(x => x.FsfBuilding)
                    .HasForeignKey<Employee>(x => x.FsfBuildingId);






            });

            modelBuilder.Entity<sis_Building>(entity =>
            {
                entity.HasKey(e => e.SisBuildingId)
                    .HasName("PK_sis_Building");

                entity.ToTable("Building", "sis");

                entity.Property(e => e.SisBuildingId)
                    .ValueGeneratedNever()
                    .HasColumnName("sisBuildingId");
            });

            modelBuilder.Entity<BuildingSource>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("BuildingSource", "hr");

                entity.Property(e => e.Description)
                    .HasMaxLength(46)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.District)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("district")
                    .IsFixedLength(true);

                entity.Property(e => e.FsfBuildingId)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("fsfBuildingId");

                entity.Property(e => e.ShortDescription)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("short_description");

                entity.Property(e => e.WorkLocation)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("work_location")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasKey(e => new { e.ContractorId, e.ContractorJobId })
                    .HasName("PK_contractor_Contract");

                entity.ToTable("Contract", "contractor");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.IsActive).HasComputedColumnSql("(case when ([EndDate] IS NULL OR [EndDate]>=dateadd(day,(-1),getdate())) AND [StartDate]<=dateadd(day,(1),getdate()) then CONVERT([bit],(1)) else CONVERT([bit],(0)) end)", false);

                entity.Property(e => e.JobTitle)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdateUser)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");

                //  entity.HasOne(d => d.Contractor)
                //  .WithMany(p => p.Contracts)
                //   .HasForeignKey(d => d.ContractorId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //   .HasConstraintName("FK_contractor_Contract");

                entity.HasOne(d => d.PrimaryUnit)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.PrimaryUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_contractor_Contract_dbo_Unit");
            });

            modelBuilder.Entity<ContractAdditionalUnit>(entity =>
            {
                entity.HasKey(e => new { e.ContractorId, e.ContractorJobId, e.UnitId })
                    .HasName("PK_contractor_ContractAdditionalUnit");

                entity.ToTable("ContractAdditionalUnit", "contractor");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdateUser)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.ContractAdditionalUnits)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_contractor_ContractAdditionalUnit_dbo_Unit");

                entity.HasOne(d => d.Contractor)
                    .WithMany(p => p.ContractAdditionalUnits)
                    .HasForeignKey(d => new { d.ContractorId, d.ContractorJobId })
                   .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_contractor_ContractAdditionalUnit_contractor_Contract");
            });



            #region Export_Employee
            modelBuilder.Entity<Export_Employee>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Employee", "export");

                entity.Property(e => e.AdobjectGuid).HasColumnName("ADObjectGuid");

                entity.Property(e => e.FirstName).HasMaxLength(255);

                entity.Property(e => e.JobTitle).HasMaxLength(255);

                entity.Property(e => e.LastName).HasMaxLength(255);

                entity.Property(e => e.MiddleName).HasMaxLength(255);

                entity.Property(e => e.Nickname).HasMaxLength(255);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.SchoologyUserId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SuffixName).HasMaxLength(255);

                entity.Property(e => e.UsernameOverride)
                    .HasMaxLength(128)
                    .IsUnicode(false);
            });
            #endregion
            #region Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId)
                    .HasName("PK_hr_Employee");

                entity.ToTable("Employee", "hr");

                entity.Property(e => e.EmployeeId).ValueGeneratedNever();

                entity.Property(e => e.EmployeeStatus)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FsfBuildingId)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("fsfBuildingId");

                entity.Property(e => e.JobTitle)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.JobTitleManual)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Jobcode).HasColumnName("jobcode");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.MiddleName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PersonnelCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("personnel_code")
                    .IsFixedLength(true);

                entity.Property(e => e.SuffixName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);



                ;





                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK_hr_Employee_dbo_Unit");

                entity.HasOne(d => d.FsfBuilding)
                    .WithOne(p => p.Employee)
                    .HasPrincipalKey<Employee>(p => p.FsfBuildingId)
                    .HasForeignKey<hr_Building>(p => p.FsfBuildingId);

                //entity.HasOne(d => d.NeedsReview)
                //  .WithOne(p => p.Employee)
                //  .HasPrincipalKey<NeedsReview>(p => p.ReferenceId)
                //  .HasForeignKey<Employee>(p => p.EmployeeId);

                entity.HasOne(d => d.Jobcodes)
                .WithOne(p => p.Employee)
                .HasPrincipalKey<Jobcode>(p => p.JobcodeId)
                .HasForeignKey<Employee>(p => p.Jobcode);

                entity.HasOne(d => d.PersonnelCodes)
              .WithOne(p => p.Employee)
              .HasPrincipalKey<PersonnelCode>(p => p.Personnel_Code)
              .HasForeignKey<Employee>(p => p.PersonnelCode);

                //  entity.HasOne(d => d.EmployeeLatestJob)
                //      .WithOne(p => p.Employee)
                //      ;



            });
            #endregion
            #region EmployeeAdditionalUnit
            modelBuilder.Entity<EmployeeAdditionalUnit>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.UnitId })
                    .HasName("PK_hr_EmployeeAdditionalUnit");

                entity.ToTable("EmployeeAdditionalUnit", "hr");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeAdditionalUnits)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_hr_EmployeeAdditionalUnit_hr_Employee");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.EmployeeAdditionalUnits)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_hr_EmployeeAddidionalUnit_dbo_Unit");
            });
            #endregion
            #region EmployeeJob
            modelBuilder.Entity<EmployeeJob>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.JobId })
                    .HasName("PK_hr_EmployeeJob");

                entity.ToTable("EmployeeJob", "hr");

                entity.Property(e => e.FsfBuildingId)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("fsfBuildingId")
                    .IsFixedLength(true);

                entity.Property(e => e.Jobcode).HasColumnName("jobcode");

                entity.Property(e => e.PersonnelCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("personnel_code")
                    .IsFixedLength(true);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeJobs)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_hr_EmployeeJob_hr_Employee");


                entity.HasOne(d => d.Jobcodes)
                .WithOne(p => p.EmployeeJob)
                .HasPrincipalKey<EmployeeJob>(p => p.Jobcode)
                .HasForeignKey<Jobcode>(p => p.JobcodeId);

                entity.HasOne(d => d.PersonnelCodes)
              .WithOne(p => p.EmployeeJob)
              .HasPrincipalKey<EmployeeJob>(d => d.PersonnelCode)
              .HasForeignKey<PersonnelCode>(d => d.Personnel_Code)

              ;



                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.EmployeeJobs)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK_hr_EmployeeJob_dbo_Unit");
            });
            #endregion

            modelBuilder.Entity<EmployeeJobSource>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("EmployeeJobSource", "hr");

                entity.Property(e => e.FsfBuildingId)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("fsfBuildingId");

                entity.Property(e => e.Jobcode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("jobcode")
                    .IsFixedLength(true);

                entity.Property(e => e.PersonnelCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("personnel_code")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<EmployeeSource>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("EmployeeSource", "hr");

                entity.Property(e => e.EmployeeStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.FsfBuildingId)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("fsfBuildingId");

                entity.Property(e => e.Jobcode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("jobcode")
                    .IsFixedLength(true);

                entity.Property(e => e.LastName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PersonnelCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("personnel_code")
                    .IsFixedLength(true);

                entity.Property(e => e.SuffixName)
                    .HasMaxLength(5)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ExchangeEmail>(entity =>
            {
                entity.HasKey(e => new { e.ObjectGuid, e.RowNumber })
                    .HasName("PK_ad_ExchangeEmail");

                entity.ToTable("ExchangeEmail", "ad");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(case when charindex(':',[ValueRaw])>(0) then substring([ValueRaw],charindex(':',[ValueRaw])+(1),len([ValueRaw])-charindex(':',[ValueRaw])) else '' end)", false);

                entity.Property(e => e.Type)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(case when charindex(':',[ValueRaw])>(0) then substring([ValueRaw],(0),charindex(':',[ValueRaw])) else '' end)", false);

                entity.Property(e => e.ValueRaw)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FieldDatum>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("FieldData", "ad");

                entity.Property(e => e.DatabaseType).HasMaxLength(128);

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PropertyName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.StaticValue)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TableColumn)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.TableSchema)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<FieldGroup>(entity =>
            {
                entity.HasKey(e => e.GroupId)
                    .HasName("PK_ad_FieldGroup");

                entity.ToTable("FieldGroup", "ad");

                entity.HasIndex(e => e.Name, "IX_ad_FieldGroup_Name")
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.TableSchema)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<FieldProperty>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.PropertyId })
                    .HasName("PK_ad_FieldProperty");

                entity.ToTable("FieldProperty", "ad");

                entity.Property(e => e.IsEnabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsList)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PropertyName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.StaticValue)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TableColumn)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.FieldProperties)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ad_FieldProperty_ad_FieldGroup");
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.ToTable("Grade", "sis");

                entity.Property(e => e.GradeId)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.GradeLevel)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.PasswordPolicy)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.ObjectGuid)
                    .HasName("PK_ad_Group");

                entity.ToTable("Group", "ad");

                entity.Property(e => e.ObjectGuid).ValueGeneratedNever();

                entity.Property(e => e.CommonName)
                    // .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.DistinguishedName)
                    //  .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.ExtensionAttribute2)
                    .HasMaxLength(256)
                    .HasColumnName("extensionAttribute2");

                entity.Property(e => e.Mail).HasMaxLength(256);

                entity.Property(e => e.MailNickname).HasMaxLength(256);

                entity.Property(e => e.Name)
                    //  .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Notes).HasMaxLength(256);

                entity.Property(e => e.SamaccountName)
                    // .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("SAMAccountName");

                entity.Property(e => e.Usnchanged).HasColumnName("USNChanged");

                entity.Property(e => e.WhenChanged).HasColumnType("datetime");

                entity.Property(e => e.WhenCreated).HasColumnType("datetime");


                entity.HasOne(d => d.StaffGroup)
                  .WithOne(d => d.Group)
                  .HasPrincipalKey<StaffGroup>(p => p.ADObjectGuid)
                  .HasForeignKey<Group>(p => p.ObjectGuid);

                entity.HasOne(d => d.OrganizationalUnit)
                    .WithOne(d => d.Group)
                    .HasPrincipalKey<Group>(p => p.OrganizationalUnitGuid)
                    .HasForeignKey<OrganizationalUnit>(p => p.ObjectGuid)
                    ;

            });

            modelBuilder.Entity<GroupAssignment>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("GroupAssignment");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.AssignmentSourceNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.AssignmentSource)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo_GroupAssignment_dbo_GroupAssignmentSource");

                entity.HasOne(d => d.Rule)
                    .WithMany()
                    .HasForeignKey(d => d.RuleId)
                    .HasConstraintName("FK_dbo_GroupAssignment_dbo_GroupMembershipRule");

                entity.HasOne(d => d.System)
                    .WithMany()
                    .HasForeignKey(d => d.SystemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo_GroupAssignment_dbo_Account");
            });

            modelBuilder.Entity<GroupAssignmentSource>(entity =>
            {
                entity.HasKey(e => e.AssignmentSource)
                    .HasName("PK_dbo_GroupAssignmentSource");

                entity.ToTable("GroupAssignmentSource");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });


            modelBuilder.Entity<GroupExclusionList>(entity =>
            {
                entity.HasKey(e => new { e.FlaggedRuleId, e.ExludedByRuleId })
                    .HasName("PK_dbo_GroupExclusionList");

                entity.ToTable("GroupExclusionList");

                entity.HasOne(d => d.ExludedByRule)
                    .WithMany(p => p.GroupExclusionListExludedByRules)
                    .HasForeignKey(d => d.ExludedByRuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo_GroupExclusionList_dbo_GroupMembershipRule_Exclusion");

                entity.HasOne(d => d.FlaggedRule)
                    .WithMany(p => p.GroupExclusionListFlaggedRules)
                    .HasForeignKey(d => d.FlaggedRuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo_GroupExclusionList_dbo_GroupMembershipRule_Rule");
            });

            modelBuilder.Entity<GroupMembership>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.MemberId })
                    .HasName("PK_ad_GroupMembership");

                entity.ToTable("GroupMembership", "ad");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupMemberships)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ad_GroupMembership_ad_Group");
            });

            modelBuilder.Entity<GroupMembershipRule>(entity =>
            {
                entity.HasKey(e => e.RuleId)
                    .HasName("PK_dbo_GroupMembershipRule");

                entity.ToTable("GroupMembershipRule");

                entity.Property(e => e.AutomationSql)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("AutomationSQL");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DirectNameRule)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<JobTitle>(entity =>
            {
                entity.HasKey(e => e.EntryId)
                    .HasName("PK_hr_JobTitle");

                entity.ToTable("JobTitle", "hr");

                entity.HasIndex(e => new { e.Jobcode, e.PersonnelCode }, "IX_hr_JobTitle_JCPC")
                    .IsUnique();

                entity.HasIndex(e => e.PersonnelCode, "IX_hr_JobTitle_PC");

                entity.Property(e => e.Comment)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Jobcode).HasColumnName("jobcode");

                entity.Property(e => e.JobtitleText)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PersonnelCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("personnel_code")
                    .IsFixedLength(true);

                entity.Property(e => e.PersonnelCodeText)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.SortCoding)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Title).HasMaxLength(255);
            });

            //modelBuilder.Entity<JobTitleAssignment>(entity =>
            //{
            //    entity.HasOne(d => d.Account)
            //      .WithOne(j => j.JobTitleAssignment)
            //      .HasPrincipalKey<JobTitleAssignment>(p => p.SystemId)
            //      .HasForeignKey<Account>(p => p.SystemId);
            //});

            modelBuilder.Entity<Jobcode>(entity =>
            {
                entity.HasKey(e => e.JobcodeId)
                    .HasName("PK_hr_Jobcode");

                entity.ToTable("Jobcode", "hr");

                entity.Property(e => e.JobcodeId)
                    .ValueGeneratedNever()
                    .HasColumnName("Jobcode");

                entity.Property(e => e.BsdDescription)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Comment)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(isnull([BsdDescription],[OfficialDescription]))", false);

                entity.Property(e => e.OfficialDescription)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.JobRecord)
                    .WithOne(p => p.Jobcode)

                .HasForeignKey<JobRecord>(p => p.HrJobCode);
                // .HasForeignKey<Jobcode>(p => p.JobcodeId)




            });

            modelBuilder.Entity<JobcodeSource>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("JobcodeSource", "hr");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Jobcode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("jobcode")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<JobRecord>(entity =>
            {
                entity.HasKey(e => e.JobRecordId)
                    .HasName("PK_JobRecord");

                entity.ToTable("JobRecord", "dbo");

                entity.Property(e => e.HrRecordNumber).HasColumnName("HR_RecordNumber");
                entity.Property(e => e.HrWorkLocation).HasColumnName("HR_WorkLocation");
                entity.Property(e => e.HrPersonnelCode).HasColumnName("HR_PersonnelCode");
                entity.Property(e => e.HrJobCode).HasColumnName("HR_JobCode");
                entity.Property(e => e.HrPrimary).HasColumnName("HR_Primary");
                entity.Property(e => e.HrMatched).HasColumnName("HR_Matched");

                entity.HasOne(d => d.Account)
                   .WithMany(p => p.JobRecords)
               //  .HasForeignKey(d => d.SystemId)

               //.HasPrincipalKey<JobRecord>(p => p.SystemId)
               //.HasForeignKey<Account>(p => p.SystemId)
               ;


                ////  entity.HasOne(d => d.Jobcode)
                ////  .WithOne(p => p.JobRecord)

                ////  //.HasForeignKey<JobRecord>(p => p.HrJobCode)
                ////  .HasForeignKey<Jobcode>(p => p.JobcodeId)
                ////;


                entity.HasOne(d => d.Title)
                 .WithMany(p => p.JobRecords)
                 .HasForeignKey(d => d.TitleId)
                 .HasConstraintName("FK_dbo_JobRecord_dbo_Title");

                entity.HasOne(d => d.TitleGroup)
                  .WithMany(p => p.JobRecords)
                  .HasForeignKey(d => d.TitleGroupId)
                  .HasConstraintName("FK_dbo_JobRecord_dbo_TitleGroup");


                //    entity.HasOne(d => d.PersonnelCode)
                //  .WithOne(p => p.JobRecord)
                //.HasPrincipalKey<JobRecord>(d => d.HrPersonnelCode)
                //  .HasForeignKey<PersonnelCode>(p => p.Personnel_Code)
                //   ;
            });

            modelBuilder.Entity<JobRecord_Ordered>(entity =>
            {


                entity.ToView("JobRecord_Ordered", "dbo");

                entity.Property(e => e.HrRecordNumber).HasColumnName("HR_RecordNumber");
                entity.Property(e => e.HrWorkLocation).HasColumnName("HR_WorkLocation");
                entity.Property(e => e.HrPersonnelCode).HasColumnName("HR_PersonnelCode");
                entity.Property(e => e.HrJobCode).HasColumnName("HR_JobCode");
                entity.Property(e => e.HrPrimary).HasColumnName("HR_Primary");
                entity.Property(e => e.HrMatched).HasColumnName("HR_Matched");

                //entity.HasOne(d => d.Account)
                //   .WithMany(p => p.JobRecords)



                //  entity.HasOne(d => d.Jobcode)
                //  .WithOne(p => p.JobRecord)


                //  .HasForeignKey<Jobcode>(p => p.JobcodeId)
                //;


                //entity.HasOne(d => d.Title)
                // .WithMany(p => p.JobRecords)
                // .HasForeignKey(d => d.TitleId)


                //entity.HasOne(d => d.TitleGroup)
                //  .WithMany(p => p.JobRecords)
                //  .HasForeignKey(d => d.TitleGroupId)


                //    entity.HasOne(d => d.PersonnelCode)
                //  .WithOne(p => p.JobRecord)
                //.HasPrincipalKey<JobRecord>(d => d.HrPersonnelCode)
                //  .HasForeignKey<PersonnelCode>(p => p.Personnel_Code)
                //   ;
            });





            modelBuilder.Entity<OrganizationalUnit>(entity =>
            {
                entity.HasKey(e => e.ObjectGuid)
                    .HasName("PK_ad_OrganizationalUnit");

                entity.ToTable("OrganizationalUnit", "ad");

                entity.Property(e => e.ObjectGuid).ValueGeneratedNever();

                entity.Property(e => e.CanonicalName)
                    // .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.City).HasMaxLength(256);

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.DistinguishedName)
                    //  .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.OrganizationalUnitName)
                    // .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.State).HasMaxLength(256);

                entity.Property(e => e.StreetAddress).HasMaxLength(256);

                entity.Property(e => e.WhenChanged).HasColumnType("datetime");

                entity.Property(e => e.WhenCreated).HasColumnType("datetime");

                entity.Property(e => e.ZipCode).HasMaxLength(256);

                //entity.HasOne(d => d.User)
                //    .WithOne(p => p.OrganizationalUnit)
                //    .HasPrincipalKey<OrganizationalUnit>(p => p.ObjectGuid)
                //    .HasForeignKey<User>(p => p.OrganizationalUnitGuid);
                //    .HasConstraintName("FK_ad_OrganizationalUnit_ad_User");

                //entity.HasOne(d => d.ParentOrganizationalUnitGu)
                //    .WithMany(p => p.InverseParentOrganizationalUnitGu)
                //    .HasForeignKey(d => d.ParentOrganizationalUnitGuid)
                //    .HasConstraintName("FK_ad_OrganizationalUnit_ad_OrganizationalUnit");
            });





            modelBuilder.Entity<PersonnelCode>(entity =>
            {
                entity.HasKey(e => e.Personnel_Code)
                    .HasName("PK_hr_PersonnelCode");

                entity.ToTable("PersonnelCode", "hr");


                entity.Property(e => e.Description)
                    // .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                //  entity.HasOne(d => d.JobRecord)
                //    .WithOne(p => p.PersonnelCode)

                //.HasForeignKey<JobRecord>(p => p.HrPersonnelCode);


            });

            modelBuilder.Entity<PersonnelcodeSource>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("PersonnelcodeSource", "hr");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.PersonnelCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("personnel_code")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Phone>(entity =>
            {
                entity.HasKey(e => new { e.ObjectGuid, e.Type, e.RowNumber })
                    .HasName("PK_ad_Phone");

                entity.ToTable("Phone", "ad");

                entity.Property(e => e.Type)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Export_StaffBuilding>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StaffBuilding", "export");

                entity.Property(e => e.Adname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ADName");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HomeDirServer)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StreetAddress)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Sis_StaffBuilding>(entity =>
            {
                entity.HasKey(e => new { e.StaffId, e.SisBuildingId })
                    .HasName("PK_sis_StaffBuilding");

                entity.ToTable("StaffBuilding", "sis");

                entity.Property(e => e.StaffId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SisBuildingId).HasColumnName("sisBuildingId");

                entity.HasOne(d => d.SisBuilding)
                    .WithMany(p => p.StaffBuilding1s)
                    .HasForeignKey(d => d.SisBuildingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_sis_StaffBuilding_sis_Building");
            });

            //modelBuilder.Entity<SiteText>(entity =>
            //{
            //    entity.HasKey(e => e.SiteTextId)
            //        .HasName("PK_dbo_SiteText");

            //    entity.ToTable("SiteText", "dbo");


            //});

            modelBuilder.Entity<StaffBuildingSource>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StaffBuildingSource", "sis");

                entity.Property(e => e.SisBuildingId).HasColumnName("sisBuildingId");

                entity.Property(e => e.StaffId)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<StaffSource>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StaffSource", "sis");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ImsuserRecId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("IMSUserRecId");

                entity.Property(e => e.StaffId)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });



            modelBuilder.Entity<StaffUnit>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StaffUnit", "export");

                entity.Property(e => e.Adname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ADName");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HomeDirServer)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneFax)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone_Fax");

                entity.Property(e => e.PhoneGuidance)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone_Guidance");

                entity.Property(e => e.PhoneNurse)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone_Nurse");

                entity.Property(e => e.PhoneOffice)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone_Office");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StreetAddress)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student", "sis");

                entity.Property(e => e.StudentId).ValueGeneratedNever();

                entity.Property(e => e.CurrentStatus)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GradeId)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SisBuildingId).HasColumnName("sisBuildingId");

                entity.Property(e => e.StateReportId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SuffixName)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.WithdrawalDate).HasColumnType("date");

                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.GradeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_sis_Student_sis_Grade");

                entity.HasOne(d => d.SisBuilding)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.SisBuildingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_sis_Student_sis_Building");
            });

            modelBuilder.Entity<StudentAccount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentAccount", "export");

                entity.Property(e => e.AccountStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.AdobjectGuid).HasColumnName("ADObjectGuid");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Grade)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.SisBuildingId).HasColumnName("sisBuildingId");

                entity.Property(e => e.StateReportId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SuffixName)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.UsernameOverride)
                    .HasMaxLength(128)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<StudentBuilding>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentBuilding", "export");

                entity.Property(e => e.Adname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ADName");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SisBuildingMatch).HasColumnName("sis_BuildingMatch");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.StreetAddress)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<StudentSource>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentSource", "sis");

                entity.Property(e => e.CurrentStatus)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.GradeId)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.LastName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SisBuildingId).HasColumnName("sisBuildingId");

                entity.Property(e => e.StateReportId)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.SuffixName)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.WithdrawalDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Title>(entity =>
            {
                entity.HasKey(e => e.TitleId)
                    .HasName("PK_dbo_Title");

                entity.ToTable("Title", "dbo");


            });

            modelBuilder.Entity<TitleGroup>(entity =>
            {
                entity.HasKey(e => e.TitleGroupId)
                    .HasName("PK_dbo_TitleGroup");

                entity.ToTable("TitleGroup", "dbo");


            });

            modelBuilder.Entity<TitleGroup_Title>(entity =>
            {
                entity.HasKey(e => new { e.TitleGroupId, e.TitleId })
                   .HasName("PK_dbo_TitleGroup_Title");

                entity.ToTable("TitleGroup_Title", "dbo");


            });

            modelBuilder.Entity<UnitBuilding>(entity =>
            {
                entity.HasKey(e => new { e.UnitId, e.BuildingId })
                   .HasName("PK_dbo_UnitBuilding");

                entity.ToTable("UnitBuilding", "dbo");


            });


            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Unit");

                entity.Property(e => e.UnitId).ValueGeneratedNever();

                entity.Property(e => e.AdgroupName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ADGroupName")
                    .HasComputedColumnSql("(case [UseGenericADGroups] when (1) then [ADName] else 'BSD' end)", false);

                entity.Property(e => e.Adname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ADName");

                entity.Property(e => e.Comment)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.HomeDirServer)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HrJobcodeMatch)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("hr_jobcodeMatch");

                entity.Property(e => e.HrWorkLocationMatch)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("hr_WorkLocationMatch")
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneFax)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone_Fax");

                entity.Property(e => e.PhoneGuidance)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone_Guidance");

                entity.Property(e => e.PhoneNurse)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone_Nurse");

                entity.Property(e => e.PhoneOffice)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone_Office");

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SisBuildingMatch).HasColumnName("sis_BuildingMatch");

                entity.Property(e => e.UseGenericAdgroups).HasColumnName("UseGenericADGroups");

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.Units)
                    .HasForeignKey(d => d.BuildingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo_Unit_dbo_Building");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.ObjectGuid)
                    .HasName("PK_ad_User");


                entity.ToTable("User", "ad");

                entity.Property(e => e.ObjectGuid).ValueGeneratedNever();

                entity.Property(e => e.AccountExpires).HasColumnType("datetime");

                entity.Property(e => e.AdEmployeeId).HasMaxLength(256);

                entity.Property(e => e.AdEmployeeNumber).HasMaxLength(256);

                entity.Property(e => e.AdEmployeeType).HasMaxLength(256);

                entity.Property(e => e.City).HasMaxLength(256);

                entity.Property(e => e.CommonName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Company).HasMaxLength(256);

                entity.Property(e => e.Department).HasMaxLength(256);

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.DisplayName).HasMaxLength(256);

                entity.Property(e => e.DistinguishedName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.FaxNumber).HasMaxLength(256);

                entity.Property(e => e.GivenName).HasMaxLength(256);

                entity.Property(e => e.HomeDirectory).HasMaxLength(256);

                entity.Property(e => e.HomeDrive).HasMaxLength(256);

                entity.Property(e => e.Initials).HasMaxLength(256);

                entity.Property(e => e.JobTitle).HasMaxLength(256);

                entity.Property(e => e.Mail).HasMaxLength(256);

                entity.Property(e => e.MailNickname).HasMaxLength(256);

                entity.Property(e => e.MiddleName).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Office).HasMaxLength(256);

                entity.Property(e => e.OfficeNumber).HasMaxLength(256);

                entity.Property(e => e.SamaccountName)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("SAMAccountName");

                entity.Property(e => e.State).HasMaxLength(256);

                entity.Property(e => e.StreetAddress).HasMaxLength(256);

                entity.Property(e => e.Surname).HasMaxLength(256);

                entity.Property(e => e.UserPrincipalName).HasMaxLength(256);

                entity.Property(e => e.Usnchanged).HasColumnName("USNChanged");

                entity.Property(e => e.WhenChanged).HasColumnType("datetime");

                entity.Property(e => e.WhenCreated).HasColumnType("datetime");

                entity.Property(e => e.ZipCode).HasMaxLength(256);

                //entity.HasOne(d => d.ManagerGu)
                //   .WithMany(p => p.InverseManagerGu)
                //     .HasForeignKey(d => d.ManagerGuid)
                //    .HasConstraintName("FK_ad_User_ad_User_Manager");

                //entity.HasOne(d => d.OrganizationalUnit)
                //  .WithOne(d => d.User)
                //    .HasPrincipalKey<User>(p => p.OrganizationalUnitGuid)
                //    .HasForeignKey<OrganizationalUnit>(p => p.ObjectGuid);
                //   .HasConstraintName("FK_ad_User_ad_OrganizationalUnit");

                //entity.HasOne(d => d.PrimaryGroupGu)
                //  .WithMany(p => p.Users)
                //    .HasForeignKey(d => d.PrimaryGroupGuid)
                //    .HasConstraintName("FK_ad_User_ad_Group");

                entity.HasOne(d => d.Account)
                     .WithOne(d => d.User)
                     .HasPrincipalKey<User>(p => p.ObjectGuid)
                     .HasForeignKey<Account>(p => p.AdobjectGuid)

                 ;
            });



            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.PermissionID)
                    .HasName("PK_Permission");


                entity.ToTable("Permission");


                entity.HasOne(d => d.Account)
                     .WithOne(p => p.Permission)
                     .HasPrincipalKey<Permission>(p => p.SystemId)
                     .HasForeignKey<Account>(p => p.SystemId)
                 ;




            });



            modelBuilder.Entity<PermissionRole>(entity =>
            {
                entity.HasKey(e => e.PermissionRoleID)
                    .HasName("PK_PermissionRole");


                entity.ToTable("PermissionRole");
                entity.Property(e => e.PermissionRoleName).HasColumnName("PermissionRole");

                entity.HasOne(d => d.PermissionLevelRole);





            });

            modelBuilder.Entity<PermissionLevelRole>(entity =>
            {
                entity.HasKey(e => e.PermissionLevelRoleID)
                    .HasName("PK_PermissionLevelRoleID");


                entity.ToTable("PermissionLevelRole");






            });

            modelBuilder.Entity<PermissionLevel>(entity =>
            {
                entity.HasKey(e => e.PermissionLevelID)
                    .HasName("PK_PermissionLevel");


                entity.ToTable("PermissionLevel");
                entity.Property(e => e.PermissionLevelName).HasColumnName("PermissionLevel");


            });





            modelBuilder.Entity<Contractor_staff>(entity =>
            {
                entity.HasKey(e => e.ContractorId)
                    .HasName("PK_contractor_Staff");

                entity.ToTable("Staff", "contractor");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.MiddleName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Nickname)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SuffixName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sis_Staff>(entity =>
            {
                entity.HasKey(e => e.StaffId)
                    .HasName("PK_sis_Staff");

                entity.ToTable("Staff", "sis");

                entity.Property(e => e.StaffId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ImsuserRecId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("IMSUserRecId");

                //entity.HasOne(d => d.Employee)
                //    .WithOne(p => p.Sis_Staff)
                //    .HasPrincipalKey<Sis_Staff>(p => Int32.Parse(p.StaffId))
                //    .HasForeignKey<Employee>(p => p.EmployeeId)
                //;


            });



            modelBuilder.Entity<Select_StaffGroupAssociation_Employees>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Select_Group_Employees>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<AuditLogView>(entity =>
            {
                entity.ToView("AuditLogView");
                entity.HasNoKey();
            });




            modelBuilder.Entity<Select_Employee_AssignedGroups>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Select_Employee_StaffGroups>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.MatchedBy).HasColumnName("Matched By");
            });

            modelBuilder.Entity<Select_PHRST_Mismatched_Employees>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Select_TitleInfo_ByTitleGroup>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Select_AuditLog>(entity =>
            {
                entity.HasNoKey();
            });



            #region StaffGroupAssociation

            modelBuilder.Entity<StaffGroupAssociation>(entity =>
            {
                entity.HasKey(e => e.StaffGroupAssociationId)
                    .HasName("PK_dbo_StaffGroupAssociation");

                entity.ToTable("StaffGroupAssociation", "dbo");



            });

            #endregion


            #region StaffGroup

            modelBuilder.Entity<StaffGroup>(entity =>
            {
                entity.HasKey(e => e.StaffGroupId)
                    .HasName("PK_dbo_StaffGroup");

                entity.ToTable("StaffGroup", "dbo");



            });

            #endregion 



            // modelBuilder.Ignore<Select_Available_JobTitles>();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
#endregion