using Microsoft.EntityFrameworkCore;
using StaffManagement.Core.Repositories.Interfaces;
using StaffManagement.Core.Services;
using StaffManagement.Data;

namespace StaffManagement.Core.Repositories
{
    public class GroupRepository : BaseRepository, IGroupRepository
    {
        public GroupRepository(IDbContextFactory<accountmanagementContext> dbFactory,
                                IAuditLoggerService log,
                                Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache,
                                ICurrentUserContext currentUser)
            : base(dbFactory, log, memoryCache, currentUser)
        {
        }

        public async Task<List<Group>> GetAllAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Groups.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Group?> GetByIdAsync(Guid id)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Groups.FirstOrDefaultAsync(m => m.ObjectGuid == id);
        }

        public async Task CreateAsync(Group group)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            group.ObjectGuid = Guid.NewGuid();
            db.Add(group);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Group group)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            db.Update(group);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var group = await db.Groups.FindAsync(id);
            if (group != null)
            {
                db.Groups.Remove(group);
                await db.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Groups.AnyAsync(e => e.ObjectGuid == id);
        }
    }
}
