using Microsoft.AspNetCore.Mvc.Rendering;
using StaffManagement.Data;
using StaffManagement.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StaffManagement.Repositories.Interfaces
{
    public interface IAccountRepository
    {

        //Add
        int AddAccount(Account account, string comment);
        int AddJobRecord(JobRecord jobRecord, string comment);


        //Edit
        void EditAccount(Account account, string comment, bool ChangeNameOnly);
        void EditAccountEmployee(Account account);


        //Get specific data for an account

        PagedList.Core.IPagedList<AccountModel> GetAccounts(string firstName, string lastName, string systemId, int unitId, int titleId, int jobCode, string personnelCode, int buildingId, string sortOrder, int pageNumber, int pageSize);
        Task<List<AccountModel>> GetAccounts(string searchFirstName, string searchLastName);
        Task<AccountModel> GetAccountByID(int id, int jobRecordId);
        Task<List<JobRecordModel>> GetJobRecordsByAccount(int systemId);
        Task<List<EmployeeJobModel>> GetEmployeeJobs(int employeeId);

        Task<EmployeeModel> GetEmployeePHRSTdata(int employeeId);
        Task<SelectList> GetEmployeeJobsSelectList(int employeeId);
        Task<List<Select_PHRST_Mismatched_Employees>> GetPHRSTMismatchedRecordsByEmployee(int systemId);
        Task<SelectList> GetActiveEmployees();
        Task<List<EmployeeModel>> SearchEmployeesByName(string searchFirstName, string searchLastName);

        // Populate page data

        Task<SelectList> GetUnitsAsync();
        SelectList GetUnits();
        Task<SelectList> GetTitlesByTitleGroup(int titleGroupId);

        Task<SelectList> GetTitles();
        Task<SelectList> GetTitleGroups();
        string GetTitlebyTitleId(int titleId);
        Task<SelectList> GetPersonnelCodes();

        Task<SelectList> GetJobCodes();
        Task<SelectList> GetfsfBuildings();
        Task<SelectList> GetEmployees();


        Task<List<AccountModel>> GetContractors();
        Task<SelectList> GetBuildingsByUnit(int unitId);
        Task<SelectList> GetBuildings();

        Task<SelectList> GetAccountTypes();

        // Task<SelectList> GetAvailableJobTitles();

        List<AuditLogView> GetAuditLog(int systemId, int jobRecordId, Guid objectGuid, string name, int employeeId);

        void RefreshTitles();








    }
}
