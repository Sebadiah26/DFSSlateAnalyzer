
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffManagement.Data;
using StaffManagement.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StaffManagement.Repositories.Interfaces
{
    public interface ISystemReviewRepository
    {

        void RemoveTitleGroupTitle(int titlegroupid, int titleid);

        Task<List<TitleGroupModel>> GetTitleGroups();

        StaffGroup GetStaffGroup(Guid objectGuid);

        Task<SelectList> GetEmployeesForStaffGroupAssociation(int staffgroupassociationid);

        Task<SelectList> GetEmployeesForGroupMembership(Guid ObjectGuid);

        PagedList.Core.IPagedList<AuditLogViewModel> GetAuditLog(string sortOrder, string searchString, int pageNumber, int pageSize);

        Task<List<TitleModel>> GetAllTitles(int titlegroupid);
        List<dynamic> GetActiveDirectoryGroups();
        string EditTitleStatus(int selectedtitleid, bool status);

        string EditTitle(int selectedtitleid, string newtitle);
        void AddTitleGroupTitle(int titlegroupid, int titleid);
        int AddTitle(string newTitleText);










    }
}
