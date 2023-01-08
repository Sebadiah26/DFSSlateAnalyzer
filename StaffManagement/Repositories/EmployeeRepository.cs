using Microsoft.EntityFrameworkCore;
using StaffManagement.Data;
using StaffManagement.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.Data.SqlClient;

namespace StaffManagement.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly accountmanagementContext _db = null;

        public EmployeeRepository(accountmanagementContext context)
        {
            _db = context;
        }



        //public async Task<List<Select_StaffManagementView_Employees>> GetEmployees(int viewid, int unitid, int jobcode, int permissionroleid)


        //{   // Ex.  var products = context.Prducts.Where(p => p.CategoryId == 1 && p.UnitsInStock < 10);
        //    //  var data = db.LenderProgram.Where(i => DbFunctions.Like(i.LenderProgramCode, "OTO%"))
        //    //.ToList();


        //    var employees = new List<Select_StaffManagementView_Employees>();

        //    var param_viewid = new SqlParameter("@viewid", viewid);
        //    var param_unitid = new SqlParameter("@unitid", unitid);
        //    var param_jobcode = new SqlParameter("@jobcode", jobcode);
        //    var param_permissionroleid = new SqlParameter("@permissionroleid", permissionroleid);

        //    IQueryable<Select_StaffManagementView_Employees> query =
        //        _db.Select_StaffManagementView_Employees
        //        .FromSqlInterpolated($"EXEC Select_StaffManagementView_Employees {viewid}, {unitid}, {jobcode}, {permissionroleid}");


        //   employees = await query.ToListAsync();




        //    return employees;


        //}

        public async Task<List<Select_Employee_AssignedGroups>> GetEmployeeGroups(int systemid)

        {


            var employees = new List<Select_Employee_AssignedGroups>();

            var param_viewid = new SqlParameter("@systemid", systemid);


            IQueryable<Select_Employee_AssignedGroups> query =
                _db.Select_Employee_AssignedGroups
                .FromSqlInterpolated($"EXEC Select_Employee_AssignedGroups {systemid}");
            employees = await query.ToListAsync();



            return employees;


        }


        public async Task<List<Select_Employee_StaffGroups>> GetEmployeeStaffGroups(int systemid)

        {


            var employees = new List<Select_Employee_StaffGroups>();

            var param_viewid = new SqlParameter("@systemid", systemid);


            IQueryable<Select_Employee_StaffGroups> query =
                _db.Select_Employee_StaffGroups
                .FromSqlInterpolated($"EXEC Select_Employee_StaffGroups {systemid}");
            employees = await query.ToListAsync();



            return employees;


        }

        //public async Task<EmployeeJobTitleAssignmentInfo> GetEmployeeJobTitleAssignmentInfo(int systemid)


        //{

        //    var data = new EmployeeJobTitleAssignmentInfo();


        //    IQueryable<EmployeeJobTitleAssignmentInfo> query = _db.EmployeeJobTitleAssignmentInfo;
        //                            ;
        //    query = query.Where(x => x.SystemId.Equals(systemid));

        //    var querydata = await query.FirstOrDefaultAsync();

        //    data = querydata;

        //    return data;


        //}
    }
}
