using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using StaffManagement.Data;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using StaffManagement.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.Repositories
{
    public class SystemReviewRepository : BaseRepository, ISystemReviewRepository
    {



        public SystemReviewRepository(accountmanagementContext db, IAuditLoggerService log, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(db, log, httpContextAccessor, memoryCache, appUser)
        {



        }





        public void AddTitleGroupTitle(int titlegroupid, int titleid)
        {
            var newTitleGroupTitle = new TitleGroup_Title()
            {
                TitleGroupId = titlegroupid,
                TitleId = titleid,
                Active = true

            };



            _db.TitleGroup_Titles.Add(newTitleGroupTitle);
            _db.SaveChanges();

            // comment = "for [Id]: [" + newAccount.SystemId.ToString() + "] " + comment;

            // _log.Log((int)ActionTypes.Create, "dbo.Account", newAccount.SystemId.ToString(), comment);




        }

        public void RemoveTitleGroupTitle(int titlegroupid, int titleid)
        {


            var titleGroupTitleToRemove = _db.TitleGroup_Titles
                    .Where(x => x.TitleGroupId == titlegroupid && x.TitleId == titleid && x.Active == true).ToList();

            foreach (var item in titleGroupTitleToRemove)
            {
                _db.TitleGroup_Titles.Remove(item);
            }
            _db.SaveChanges();

            // comment = "for [Id]: [" + newAccount.SystemId.ToString() + "] " + comment;

            // _log.Log((int)ActionTypes.Create, "dbo.Account", newAccount.SystemId.ToString(), comment);




        }



        public string EditTitle(int selectedtitleid, string newtitle)
        {
            var titleToEdit = _db.Titles.Find(selectedtitleid);
            string resultMessage = "No changes needed.";

            if (titleToEdit.Text != newtitle)

            {
                titleToEdit.Text = newtitle;

                _db.Update(titleToEdit);
                _db.SaveChanges();
                resultMessage = "Changes saved successfully.";

            }


            // comment = "for [Id]: [" + newAccount.SystemId.ToString() + "] " + comment;

            // _log.Log((int)ActionTypes.Create, "dbo.Account", newAccount.SystemId.ToString(), comment);


            return resultMessage;


        }



        public string EditTitleStatus(int selectedtitleid, bool status)
        {
            var titleToEdit = _db.Titles.Find(selectedtitleid);
            string resultMessage = "No changes needed.";

            if (titleToEdit.Active != status)

            {
                titleToEdit.Active = status;

                _db.Update(titleToEdit);
                _db.SaveChanges();
                resultMessage = "Changes saved successfully.";

            }


            // comment = "for [Id]: [" + newAccount.SystemId.ToString() + "] " + comment;

            // _log.Log((int)ActionTypes.Create, "dbo.Account", newAccount.SystemId.ToString(), comment);


            return resultMessage;


        }



        public int AddTitle(string newTitleText)
        {
            var newTitle = new Title()
            {

                Text = newTitleText,
                Active = true
            };


            _db.Titles.Add(newTitle);
            _db.SaveChanges();

            return newTitle.TitleId;


        }


        //public async Task<List<StaffManagementView>> GetViews()
        //{




        //    var model = await _db.StaffManagementView
        //      .Include(view => view.StaffManagementView_Permission)
        //      .Include(view => view.StaffManagementView_Unit)

        //      // .Include(x => x.Contract)
        //      .Where(view => view.StaffManagementView_Permission.PermissionRoleID == _appUser.PermissionRoleId).ToListAsync();


        //    return model;

        //}




        public async Task<List<TitleModel>> GetAllTitles(int titlegroupid)
        {

            var titlemodellistdata = new List<TitleModel>();


            var data = new List<Select_TitleInfo_ByTitleGroup>();


            IQueryable<Select_TitleInfo_ByTitleGroup> query =
                _db.Select_TitleInfo_ByTitleGroup
                .FromSqlInterpolated($"EXEC Select_TitleInfo_ByTitleGroup {titlegroupid}");


            data = await query.ToListAsync();


            //var query = from t in _db.Titles.Where(e => e.Active == true)

            //            join tgt in _db.TitleGroup_Titles.Where(e => e.Active == true && e.TitleGroupId == titlegroupid)
            //                on t.TitleId equals tgt.TitleId into grouping
            //            from tgt in grouping.DefaultIfEmpty()

            //            join jr in _db.JobRecord
            //                on t.TitleId equals jr.TitleId into jrt
            //            from jr in jrt.DefaultIfEmpty()

            //           // group jr by t.TitleId into grouped

            //            select new
            //            { 
            //                TitleId = t.TitleId,
            //                Text = t.Text,
            //                Selected = tgt.TitleGroupId == null ? false : true ,
            //                Count = (jrt.Count() == null) ? 0 : jrt.Count()


            //            };

            //query = query.OrderBy(x => x.Text);

            //var data = await query.ToListAsync();



            if (data.Any() == true)
            {
                foreach (var title in data)
                {
                    var titleModel = new TitleModel();

                    titleModel.TitleId = title.TitleId;
                    titleModel.Text = title.Text;
                    titleModel.Active = title.Active;
                    titleModel.Selected = (title.Selected == 1) ? true : false;
                    titleModel.TitleCount = title.Count;

                    titlemodellistdata.Add(titleModel);


                }



            }


            return titlemodellistdata;
        }



        public async Task<List<TitleGroupModel>> GetTitleGroups()
        {

            var titlegroupmodellistdata = new List<TitleGroupModel>();

            var data = await _db.TitleGroups.Where(e => e.Active == true)
                        .OrderBy(x => x.Description)
                        .ToListAsync();

            if (data.Any() == true)
            {
                foreach (var titlegroup in data)
                {
                    var titlegroupModel = new TitleGroupModel();

                    titlegroupModel.TitleGroupId = titlegroup.TitleGroupId;
                    titlegroupModel.Description = titlegroup.Description;



                    titlegroupmodellistdata.Add(titlegroupModel);


                }



            }


            return titlegroupmodellistdata;
        }



        public async Task<SelectList> GetEmployeesForStaffGroupAssociation(int staffgroupassociationid)

        {

            SelectList employees = null;

            if (!_cache.TryGetValue("StaffGroupAssociations:" + staffgroupassociationid, out employees))
            {

                var employeedatalist = new List<Select_StaffGroupAssociation_Employees>();


                IQueryable<Select_StaffGroupAssociation_Employees> query =
                    _db.Select_StaffGroupAssociation_Employees
                    .FromSqlInterpolated($"EXEC Select_StaffGroupAssociation_Employees {staffgroupassociationid}");


                employees = new SelectList(await query.ToDictionaryAsync(x => x.SystemId, x => x.Name + x.Matches), "Key", "Value");

                SetCache("StaffGroupAssociations:" + staffgroupassociationid, employees);
            }

            return employees;


        }


        public async Task<SelectList> GetEmployeesForGroupMembership(Guid ObjectGuid)

        {

            SelectList employees = null;

            if (!_cache.TryGetValue("Group:" + ObjectGuid, out employees))
            {

                var employeedatalist = new List<Select_Group_Employees>();


                IQueryable<Select_Group_Employees> query =
                    _db.Select_Group_Employees
                    .FromSqlInterpolated($"EXEC Select_Group_Employees {ObjectGuid}");


                employees = new SelectList(await query.ToDictionaryAsync(x => x.SystemId
                                                            + ":" + x.ObjectGuid
                                                            + ":" + (x.TitleGroup ?? "")
                                                            + ":" + (x.Title ?? "")
                                                             + ":" + (x.Department ?? "")
                                                              + ":" + (x.Building ?? ""),
                                                              x => x.Name
                                                              + " [" + (x.Title ?? "") + "]"
                                                              + "[" + (x.Department ?? "") + "]"), "Key", "Value");

                SetCache("Group:" + ObjectGuid, employees);
            }

            return employees;


        }




        public PagedList.Core.IPagedList<StaffGroupAssociation> GetStaffGroupAssociations()

        // string sortOrder, string searchString, int pageNumber, int pageSize
        {


            IQueryable<StaffGroupAssociation> query = _db.StaffGroupAssociations
                                                        .Include(x => x.StaffGroup);


            query.OrderBy(s => s.StaffGroupId).ThenBy(s => s.StaffGroupAssociationId);

            var querydata = query.ToPagedList(1, 1000);
            // var querydata = query.ToPagedList(pageNumber, pageSize);


            return querydata;


        }



        public List<dynamic> GetActiveDirectoryGroups()

        // string sortOrder, string searchString, int pageNumber, int pageSize
        {


            //IQueryable<Group> query = _db.Groups
            //                       .Include(x => x.OrganizationalUnit)
            //                        .OrderBy(x => x.CommonName).ThenBy(x => x.DistinguishedName)
            //                          //  .Include(x => x.StaffGroup)
            //                           // .ThenInclude(x => x.StaffGroupAssociations)
            //                            ;

            var query = from g in _db.Set<Group>()

                        join ou in _db.Set<OrganizationalUnit>()
                            on g.OrganizationalUnitGuid equals ou.ObjectGuid into grouping
                        from ou in grouping.DefaultIfEmpty()


                        select new
                        {
                            ObjectGuid = g.ObjectGuid,
                            CommonName = g.CommonName,
                            OrganizationalUnitName = ou.OrganizationalUnitName

                        };



            var querydata = query.OrderBy(x => x.OrganizationalUnitName).ThenBy(x => x.CommonName).ToList<dynamic>();




            // query.OrderBy(s => s.Description);

            // var querydata = query.ToList();
            // var querydata = query.ToPagedList(pageNumber, pageSize);


            return querydata;


        }

        public StaffGroup GetStaffGroup(Guid objectGuid)

        // string sortOrder, string searchString, int pageNumber, int pageSize
        {


            IQueryable<StaffGroup> query = _db.StaffGroups

                                        .Include(x => x.StaffGroupAssociations)
                                        .Where(x => x.ADObjectGuid == objectGuid)

                                        ;


            //query.OrderBy(s => s.GroupName);

            var querydata = query.SingleOrDefault();
            // var querydata = query.ToPagedList(pageNumber, pageSize);


            return querydata;


        }


        public PagedList.Core.IPagedList<AuditLogViewModel> GetAuditLog(string sortOrder, string searchString, int pageNumber, int pageSize)


        {


            IQueryable<Select_AuditLog> q = _db.Select_AuditLog
                 .FromSqlInterpolated($"EXEC Select_AuditLog ");

            var querySQL = q.ToList();

            var logData = new List<AuditLogViewModel>();

            if (querySQL.Any() == true)
            {
                foreach (var log in querySQL)
                {
                    var logItem = new AuditLogViewModel();

                    logItem.LogId = log.LogId;
                    logItem.LogDate = log.LogDate;
                    logItem.ReferenceID = log.ReferenceID;
                    logItem.ReferenceTable = log.ReferenceTable;
                    logItem.ActionType = log.ActionType;
                    logItem.Employee = log.Employee;
                    logItem.ApplicationUser = log.ApplicationUser;
                    logItem.ImpersonatedUser = log.ImpersonatedUser;
                    logItem.Comment = log.Comment;
                    logData.Add(logItem);

                }
            }

            IEnumerable<AuditLogViewModel> query = null;

            query = logData;

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => (s.ApplicationUser ?? "").Contains(searchString)
                                   || (s.Employee ?? "").Contains(searchString) || (s.ApplicationUser ?? "").Contains(searchString)
                                   || (s.Comment ?? "").Contains(searchString) || (s.ReferenceTable ?? "").Contains(searchString)
                                   || (s.ReferenceID ?? "").Contains(searchString));
            }

            switch (sortOrder)
            {
                case " LogId_Asc":
                    query = query.OrderByDescending(s => s.LogId);
                    break;
                case " Date_Asc_Sort":
                    query = query.OrderBy(s => s.LogDate);
                    break;
                case " Date_Desc_Sort":
                    query = query.OrderByDescending(s => s.LogDate);
                    break;
                case " Emp_Asc_Sort":
                    query = query.OrderBy(s => s.Employee);
                    break;
                case " Emp_Desc_Sort":
                    query = query.OrderByDescending(s => s.Employee);
                    break;
                case " User_Asc_Sort":
                    query = query.OrderBy(s => s.ApplicationUser);
                    break;
                case " User_Desc_Sort":
                    query = query.OrderByDescending(s => s.ApplicationUser);
                    break;
                default:
                    query = query.OrderByDescending(s => s.LogId);
                    break;


            }


            IQueryable<AuditLogViewModel> logQuery = query.AsQueryable();



            return logQuery.ToPagedList(pageNumber, pageSize);
            ;


        }
    }
}
