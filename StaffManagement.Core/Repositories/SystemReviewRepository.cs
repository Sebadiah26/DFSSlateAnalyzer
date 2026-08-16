using Microsoft.EntityFrameworkCore;
using StaffManagement.Core.Models;
using StaffManagement.Core.Repositories.Interfaces;
using StaffManagement.Core.Services;
using StaffManagement.Data;

namespace StaffManagement.Core.Repositories
{
    /// <summary>Ported incrementally from StaffManagement/Repositories/SystemReviewRepository.cs -
    /// see ISystemReviewRepository's doc comment for scope.</summary>
    public class SystemReviewRepository : BaseRepository, ISystemReviewRepository
    {
        public SystemReviewRepository(IDbContextFactory<accountmanagementContext> dbFactory,
                                       IAuditLoggerService log,
                                       Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache,
                                       ICurrentUserContext currentUser)
            : base(dbFactory, log, memoryCache, currentUser)
        {
        }

        public void AddTitleGroupTitle(int titlegroupid, int titleid)
        {
            using var db = _dbFactory.CreateDbContext();

            var newTitleGroupTitle = new TitleGroup_Title()
            {
                TitleGroupId = titlegroupid,
                TitleId = titleid,
                Active = true
            };

            db.TitleGroup_Titles.Add(newTitleGroupTitle);
            db.SaveChanges();
        }

        public void RemoveTitleGroupTitle(int titlegroupid, int titleid)
        {
            using var db = _dbFactory.CreateDbContext();

            var titleGroupTitleToRemove = db.TitleGroup_Titles
                    .Where(x => x.TitleGroupId == titlegroupid && x.TitleId == titleid && x.Active == true).ToList();

            foreach (var item in titleGroupTitleToRemove)
            {
                db.TitleGroup_Titles.Remove(item);
            }
            db.SaveChanges();
        }

        public string EditTitle(int selectedtitleid, string newtitle)
        {
            using var db = _dbFactory.CreateDbContext();

            var titleToEdit = db.Titles.Find(selectedtitleid);
            string resultMessage = "No changes needed.";

            if (titleToEdit != null && titleToEdit.Text != newtitle)
            {
                titleToEdit.Text = newtitle;
                db.Update(titleToEdit);
                db.SaveChanges();
                resultMessage = "Changes saved successfully.";
            }

            return resultMessage;
        }

        public string EditTitleStatus(int selectedtitleid, bool status)
        {
            using var db = _dbFactory.CreateDbContext();

            var titleToEdit = db.Titles.Find(selectedtitleid);
            string resultMessage = "No changes needed.";

            if (titleToEdit != null && titleToEdit.Active != status)
            {
                titleToEdit.Active = status;
                db.Update(titleToEdit);
                db.SaveChanges();
                resultMessage = "Changes saved successfully.";
            }

            return resultMessage;
        }

        public int AddTitle(string newTitleText)
        {
            using var db = _dbFactory.CreateDbContext();

            var newTitle = new Title()
            {
                Text = newTitleText,
                Active = true
            };

            db.Titles.Add(newTitle);
            db.SaveChanges();

            return newTitle.TitleId;
        }

        public async Task<List<TitleModel>> GetAllTitlesAsync(int titlegroupid)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var data = await db.Select_TitleInfo_ByTitleGroup
                .FromSqlInterpolated($"EXEC Select_TitleInfo_ByTitleGroup {titlegroupid}")
                .ToListAsync();

            return data.Select(title => new TitleModel
            {
                TitleId = title.TitleId,
                Text = title.Text,
                Active = title.Active,
                Selected = title.Selected == 1,
                TitleCount = title.Count
            }).ToList();
        }

        public async Task<List<TitleGroupModel>> GetTitleGroupsAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var data = await db.TitleGroups.Where(e => e.Active == true)
                        .OrderBy(x => x.Description)
                        .ToListAsync();

            return data.Select(titlegroup => new TitleGroupModel
            {
                TitleGroupId = titlegroup.TitleGroupId,
                Description = titlegroup.Description
            }).ToList();
        }

        public async Task<List<AuditLogViewModel>> GetAuditLogAsync(string? sortOrder, string? searchString)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var querySql = await db.Select_AuditLog
                .FromSqlInterpolated($"EXEC Select_AuditLog ")
                .ToListAsync();

            var logData = querySql.Select(log => new AuditLogViewModel
            {
                LogId = log.LogId,
                LogDate = log.LogDate,
                ReferenceID = log.ReferenceID,
                ReferenceTable = log.ReferenceTable,
                ActionType = log.ActionType,
                Employee = log.Employee,
                ApplicationUser = log.ApplicationUser,
                ImpersonatedUser = log.ImpersonatedUser,
                Comment = log.Comment
            }).AsEnumerable();

            if (!string.IsNullOrEmpty(searchString))
            {
                logData = logData.Where(s => (s.ApplicationUser ?? "").Contains(searchString)
                                   || (s.Employee ?? "").Contains(searchString)
                                   || (s.Comment ?? "").Contains(searchString) || (s.ReferenceTable ?? "").Contains(searchString)
                                   || (s.ReferenceID ?? "").Contains(searchString));
            }

            logData = sortOrder switch
            {
                " LogId_Asc" => logData.OrderByDescending(s => s.LogId),
                " Date_Asc_Sort" => logData.OrderBy(s => s.LogDate),
                " Date_Desc_Sort" => logData.OrderByDescending(s => s.LogDate),
                " Emp_Asc_Sort" => logData.OrderBy(s => s.Employee),
                " Emp_Desc_Sort" => logData.OrderByDescending(s => s.Employee),
                " User_Asc_Sort" => logData.OrderBy(s => s.ApplicationUser),
                " User_Desc_Sort" => logData.OrderByDescending(s => s.ApplicationUser),
                _ => logData.OrderByDescending(s => s.LogId),
            };

            return logData.ToList();
        }
    }
}
