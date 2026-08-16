using Microsoft.EntityFrameworkCore;
using StaffManagement.Core.Repositories.Interfaces;
using StaffManagement.Core.Services;
using StaffManagement.Data;

namespace StaffManagement.Core.Repositories
{
    /// <summary>Ported from Controllers/ContractController.cs, normalized into the repository pattern
    /// (see IContractRepository's doc comment). ContractorId is the entity's real (single-column)
    /// primary key despite ContractorJobId looking like it might be part of a composite key - there's
    /// no Fluent API HasKey override for Contract in accountmanagementContext.OnModelCreating, so the
    /// [Key] attribute on ContractorId alone governs. ContractorJobId is always seeded to 1 on create
    /// and never changes - preserved as-is, not redesigned.</summary>
    public class ContractRepository : BaseRepository, IContractRepository
    {
        public ContractRepository(IDbContextFactory<accountmanagementContext> dbFactory,
                                   IAuditLoggerService log,
                                   Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache,
                                   ICurrentUserContext currentUser)
            : base(dbFactory, log, memoryCache, currentUser)
        {
        }

        public async Task<List<Contract>> GetAllAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.Contracts
                .Include(c => c.Contractor)
                .Include(c => c.PrimaryUnit)
                .OrderBy(x => x.PrimaryUnit.Name)
                .ThenBy(x => x.JobTitle)
                .ThenBy(x => x.Contractor.LastName)
                .ToListAsync();
        }

        public async Task<Contract?> GetByContractorIdAsync(int contractorId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.Contracts
                .Include(c => c.Contractor)
                .Include(c => c.PrimaryUnit)
                .FirstOrDefaultAsync(m => m.ContractorId == contractorId);
        }

        public async Task<Contract?> GetByIdAsync(int contractorId, int contractorJobId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.Contracts
                .Include(x => x.Contractor)
                .Where(x => x.ContractorId == contractorId && x.ContractorJobId == contractorJobId)
                .SingleOrDefaultAsync();
        }

        public async Task<int> CreateAsync(Contract contract, Contractor_staff contractor)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            contractor.LastUpdate = DateTime.Now;
            db.Add(contractor);
            await db.SaveChangesAsync();

            contract.ContractorId = contractor.ContractorId;
            contract.ContractorJobId = 1;
            contract.LastUpdate = DateTime.Now;
            contract.LastUpdateUser = _currentUser.CurrentUser;

            db.Add(contract);
            await db.SaveChangesAsync();

            _log.Log((int)ActionTypes.Create, "contractor.Contract", contract.ContractorId.ToString(), null!);

            return contract.ContractorId;
        }

        public async Task UpdateAsync(Contract contract)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            contract.LastUpdateUser = _currentUser.CurrentUser;
            contract.LastUpdate = DateTime.Now;

            db.Update(contract);
            await db.SaveChangesAsync();

            _log.Log((int)ActionTypes.Edit, "contractor.Contract", contract.ContractorId.ToString(), null!);
        }

        public async Task DeleteAsync(int contractorId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var contract = await db.Contracts.FindAsync(contractorId);
            if (contract != null)
            {
                db.Contracts.Remove(contract);
                await db.SaveChangesAsync();
            }
            // NOTE: the source's DeleteConfirmed doesn't call _log.Log either - no audit entry on
            // contract delete is existing behavior, not an omission introduced by this port.
        }

        public async Task<bool> ExistsAsync(int contractorId, int contractorJobId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.Contracts.AnyAsync(e => e.ContractorId == contractorId && e.ContractorJobId == contractorJobId);
        }
    }
}
