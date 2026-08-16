using Microsoft.EntityFrameworkCore;
using StaffManagement.Core.Repositories.Interfaces;
using StaffManagement.Core.Services;
using StaffManagement.Data;

namespace StaffManagement.Core.Repositories
{
    /// <summary>Ported from StaffManagement/Repositories/JobRecordRepository.cs +
    /// JobRecordController's direct DbContext CRUD - see IJobRecordRepository's doc comment.</summary>
    public class JobRecordRepository : BaseRepository, IJobRecordRepository
    {
        public JobRecordRepository(IDbContextFactory<accountmanagementContext> dbFactory,
                                    IAuditLoggerService log,
                                    Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache,
                                    ICurrentUserContext currentUser)
            : base(dbFactory, log, memoryCache, currentUser)
        {
        }

        public async Task<List<(string Name, int JobRecordId, int? SystemId)>> GetJobRecordsNeedingReviewAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.JobRecord
                .Include(x => x.Account).ThenInclude(x => x.Employee).ThenInclude(x => x.Unit)
                .Include(x => x.Account).ThenInclude(x => x.PrimaryUnit)
                .Include(x => x.Account).ThenInclude(x => x.Employee).ThenInclude(x => x.EmployeeJobs)
                .Include(j => j.Unit)
                .Where(x => x.NeedsReview == true)
                .Select(n => new ValueTuple<string, int, int?>(
                    n.Account.FirstName + " " + n.Account.LastName,
                    n.JobRecordId,
                    n.SystemId))
                .ToListAsync();
        }

        public async Task<int> JobRecordCountAsync(int systemId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.JobRecord.Where(e => e.SystemId == systemId).CountAsync();
        }

        public async Task<List<JobRecord>> GetJobRecordsBySystemIdAsync(int systemId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.JobRecord
                .Include(x => x.Account).ThenInclude(x => x.Employee).ThenInclude(x => x.Unit)
                .Include(x => x.Account).ThenInclude(x => x.PrimaryUnit)
                .Include(x => x.Account).ThenInclude(x => x.Employee).ThenInclude(x => x.EmployeeJobs)
                .Include(j => j.Unit)
                .Where(x => x.SystemId == systemId)
                .ToListAsync();
        }

        public async Task<JobRecord?> GetJobRecordByIdAsync(int jobRecordId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.JobRecord
                .Include(j => j.Account).ThenInclude(j => j.Employee)
                .Include(j => j.Unit)
                .FirstOrDefaultAsync(m => m.JobRecordId == jobRecordId);
        }

        public async Task<int> CreateAsync(JobRecord jobRecord)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            db.Add(jobRecord);
            await db.SaveChangesAsync();

            _log.Log((int)ActionTypes.Create, "dbo.JobRecord", jobRecord.JobRecordId.ToString(), null!);

            return jobRecord.JobRecordId;
        }

        public async Task UpdateAsync(JobRecord jobRecord)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            db.Update(jobRecord);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int jobRecordId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var jobRecord = await db.JobRecord.FindAsync(jobRecordId);
            if (jobRecord != null)
            {
                db.JobRecord.Remove(jobRecord);
                await db.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int jobRecordId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.JobRecord.AnyAsync(e => e.JobRecordId == jobRecordId);
        }
    }
}
