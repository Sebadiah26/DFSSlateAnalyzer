using StaffManagement.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StaffManagement.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {

        Task<List<Select_Employee_StaffGroups>> GetEmployeeStaffGroups(int systemid);
        //   Task<List<Select_StaffManagementView_Employees>> GetEmployees(int viewid, int unitid, int jobcode, int permissionroleid);
        //   Task<EmployeeJobTitleAssignmentInfo> GetEmployeeJobTitleAssignmentInfo(int systemid);

        Task<List<Select_Employee_AssignedGroups>> GetEmployeeGroups(int systemid);











    }
}
