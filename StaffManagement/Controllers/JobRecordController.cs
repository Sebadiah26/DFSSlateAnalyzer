using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StaffManagement.Common;
using StaffManagement.Data;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using StaffManagement.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.Controllers
{
    [DisplayName("Reports")]
    [CheckPermission]
    public class JobRecordController : BaseController
    {
        private readonly IJobRecordRepository _jobrecordRepository = null;
        private readonly IAccountRepository _accountRepository = null;
        private readonly accountmanagementContext _db;
        private readonly IEmail _email = null;

        public JobRecordController(IJobRecordRepository jobrecordRepository, IAccountRepository accountRepository, accountmanagementContext context, IEmail email, IAuditLoggerService log, IHttpContextAccessor contextAccessor, URLEncrypter protector, AppUser appUser) : base(log, contextAccessor, protector, appUser)
        {
            _db = context;
            _email = email;
            _jobrecordRepository = jobrecordRepository;
            _accountRepository = accountRepository;

        }

        // GET: JobRecord
        public ActionResult Index()
        {

            return RedirectToAction("Menu");

            //var jobrecorddata = _db.JobRecord
            //              .Include(j => j.Account).ThenInclude(j => j.Employee)
            //              .Include(j => j.Unit)

            //            //  .Where(x => x.IsPHRST.Equals(true))
            //              .Where(x => x.NeedsReview.Equals(true))
            //              ;


            //return View(await jobrecorddata.ToListAsync());
        }

        // GET: JobRecord/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }



            var jobRecord = await _db.JobRecord
                .Include(j => j.Account).ThenInclude(j => j.Employee)
                .Include(j => j.Unit)
                // .FirstOrDefaultAsync(m => m.JobRecordId == id);
                .FirstOrDefaultAsync(m => m.JobRecordId == id);
            if (jobRecord == null)
            {
                return NotFound();
            }

            return View(jobRecord);
        }

        // GET: JobRecord/Create
        public IActionResult Create()
        {
            ViewData["SystemId"] = new SelectList(_db.Accounts, "SystemId", "SystemId");
            ViewData["UnitId"] = new SelectList(_db.Units, "UnitId", "AdgroupName");
            return View();
        }

        // POST: JobRecord/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobRecordId,SystemId,IsPrimary,IsActive,UnitId,StartDate,EndDate,IsPHRST,Hr_RecordNumber,HrWorkLocation,HrPersonnelCode,HrJobCode,HrPrimary,HrMatched,NeedsReview")] JobRecord jobRecord)
        {
            if (ModelState.IsValid)
            {
                _db.Add(jobRecord);
                await _db.SaveChangesAsync();

                ViewData["ResultMessage"] = "Job record successfully added.";
                _log.Log((int)ActionTypes.Create, "dbo.JobRecord", jobRecord.JobRecordId.ToString(), null);
                return RedirectToAction(nameof(Index));
            }
            ViewData["SystemId"] = new SelectList(_db.Accounts, "SystemId", "SystemId", jobRecord.SystemId);
            ViewData["UnitId"] = new SelectList(_db.Units, "UnitId", "AdgroupName", jobRecord.UnitId);
            return View(jobRecord);
        }

        // GET: JobRecord/Edit/5
        public async Task<IActionResult> Edit(int? id, int systemid)
        {
            if (id == null)
            {
                return NotFound();
            }



            var jobRecorddata = await

                          _db.JobRecord.Include(x => x.Account).ThenInclude(x => x.Employee).ThenInclude(x => x.Unit)
                             .Include(x => x.Account).ThenInclude(x => x.PrimaryUnit)
                             //  .Include(x => x.Account).ThenInclude(x => x.Title)
                             //  .Include(x => x.Account).ThenInclude(x => x.TitleGroup)
                             .Include(x => x.Account).ThenInclude(x => x.Employee).ThenInclude(x => x.EmployeeJobs)
                             .Include(j => j.Unit)
                             .Where(x => x.SystemId == systemid)
                             .ToListAsync();





            if (jobRecorddata == null)
            {
                return NotFound();
            }


            ViewData["UnitId"] = new SelectList(_db.Units, "UnitId", "AdgroupName", jobRecorddata.Where(x => x.JobRecordId == id).ToList().First().UnitId);
            return View(jobRecorddata);
        }

        // POST: JobRecord/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JobRecordId,SystemId,IsPrimary,IsActive,UnitId,StartDate,EndDate,IsPHRST,Hr_RecordNumber,HrWorkLocation,HrPersonnelCode,HrJobCode,HrPrimary,HrMatched,NeedsReview")] JobRecord jobRecord)
        {
            if (id != jobRecord.JobRecordId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(jobRecord);
                    await _db.SaveChangesAsync();
                    ViewData["ResultMessage"] = "Job record updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobRecordExists(jobRecord.JobRecordId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // ViewData["SystemId"] = new SelectList(_db.Accounts, "SystemId", "SystemId", jobRecord.SystemId);
            ViewData["UnitId"] = new SelectList(_db.Units, "UnitId", "AdgroupName", jobRecord.UnitId);
            return View(jobRecord);
        }

        // GET: JobRecord/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobRecord = await _db.JobRecord
                .Include(j => j.Account)
                .Include(j => j.Unit)
                .FirstOrDefaultAsync(m => m.JobRecordId == id);
            if (jobRecord == null)
            {
                return NotFound();
            }

            return View(jobRecord);
        }

        // POST: JobRecord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobRecord = await _db.JobRecord.FindAsync(id);
            _db.JobRecord.Remove(jobRecord);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobRecordExists(int id)
        {
            return _db.JobRecord.Any(e => e.JobRecordId == id);
        }


        public int JobRecordCount(int systemid)
        {
            return _db.JobRecord.Where(e => e.SystemId == systemid).Count();
        }

        public ViewResult SendNotices()
        {

            return View();
        }
        [HttpPost]
        public ViewResult SendNotices(string submitAction)
        {
            // Perform action based on submit button clicked




            if (submitAction == "JobRecords")
            {
                var jobrecordquery = _jobrecordRepository.GetJobRecordsNeedingReview();


                foreach (var jobrecord in jobrecordquery)
                {


                    var emailmessage = new EmailMessage()
                    {
                        Sender = "noreply@bsd.k12.de.us",
                        Receiver = "craig.kielinski@bsd.k12.de.us",
                        Subject = "Alert",
                        Content = _email.GetEmailBodyFromListItem(jobrecord, "An Employee Change Needs to be Reviewed")
                    };

                    _email.Send(emailmessage);

                };




            }



            if (submitAction == "Employees")
            { }

            // return RedirectToAction(nameof(Index));
            return View();
        }



        [DisplayName("Unlinked Employees")]
        [HttpPost]
        public async Task<ActionResult> UnlinkedEmployees(string chkJob, string chkName, string chkNew, string chkEnd)
        {
            //if (chkHandled == "on" || chkHandled == "true")
            //string submit,
            //{ ViewBag.ShowHandled = "true";
            //  TempData["ShowHandled"] = "true";
            //int needsReviewId, bool needsReview, int employeeId,
            //}

            if (chkJob == "on")
            { ViewBag.ShowJob = "true"; }

            if (chkName == "on")
            { ViewBag.ShowName = "true"; }

            if (chkNew == "on")
            { ViewBag.ShowNew = "true"; }

            if (chkEnd == "on")
            { ViewBag.ShowEnd = "true"; }



            //if (submit == "Mark Record As Handled" || submit == "Reset to Unhandled")
            //{
            //try
            //{
            //    var needsReviewToEdit = await _db.NeedsReview.FindAsync(needsReviewId);

            //    needsReviewToEdit.needsReview = !needsReview;
            //    needsReviewToEdit.Comment = form["item.NeedsReview.Comment"].ToString();
            //    needsReviewToEdit.LastUpdateUser = _appUser.CurrentUser;
            //    needsReviewToEdit.LastUpdate = DateTime.Now;

            //    _db.Update(needsReviewToEdit);
            //    _db.SaveChanges();

            //    _log.Log((int)ActionTypes.Edit, "dbo.NeedsReview", needsReviewId.ToString(), 
            //        "Employeeid " + employeeId.ToString() + " NeedsReview set to " + needsReviewToEdit.needsReview.ToString());
            //TempData["ResultMessage"] = "Changes saved successfully.";

            //return RedirectToAction("UnlinkedEmployees");
            //}
            //catch (Exception ex)
            //{
            //    // log error
            //    _log.Log((int)ActionTypes.Edit, "dbo.NeedsReview", needsReviewId.ToString(), ex.InnerException.ToString());
            //    throw;
            //}


            //}



            var employeeData = await _accountRepository.GetPHRSTMismatchedRecordsByEmployee(0);

            if (ViewBag.ShowJob != "true")
            {
                employeeData = (List<Select_PHRST_Mismatched_Employees>)employeeData.Where(x => x.Reason != "Job").ToList();
            }

            if (ViewBag.ShowName != "true")
            {
                employeeData = (List<Select_PHRST_Mismatched_Employees>)employeeData.Where(x => x.Reason != "Name").ToList();
            }

            if (ViewBag.ShowNew != "true")
            {
                employeeData = (List<Select_PHRST_Mismatched_Employees>)employeeData.Where(x => x.Reason != "New").ToList();
            }

            if (ViewBag.ShowEnd != "true")
            {
                employeeData = (List<Select_PHRST_Mismatched_Employees>)employeeData.Where(x => x.Reason != "End").ToList();
            }

            foreach (var employee in employeeData)
            {

                // Save value in session so that employeeid is not sent through URL 
                _appUser.SaveEncryptedValue(_httpContextAccessor.HttpContext, employee.Slug.ToString(), employee.EmployeeId.ToString());

            }


            return View(employeeData.ToList());

        }


        [DisplayName("Unlinked Employees")]
        public async Task<ViewResult> UnlinkedEmployees()
        {
            //ViewBag.ShowHandled = TempData["ShowHandled"];


            ViewBag.ShowJob = "true";
            ViewBag.ShowName = "true";
            ViewBag.ShowNew = "true";
            ViewBag.ShowEnd = "true";

            //var employeemodellist = await _jobrecordRepository.LoadUnlinkedEmployees();
            var employeeData = await _accountRepository.GetPHRSTMismatchedRecordsByEmployee(0);

            if (ViewBag.ShowJob != "true")
            {
                employeeData = (List<Select_PHRST_Mismatched_Employees>)employeeData.Where(x => x.Reason != "Job").ToList();
            }

            if (ViewBag.ShowName != "true")
            {
                employeeData = (List<Select_PHRST_Mismatched_Employees>)employeeData.Where(x => x.Reason != "Name").ToList();
            }

            if (ViewBag.ShowNew != "true")
            {
                employeeData = (List<Select_PHRST_Mismatched_Employees>)employeeData.Where(x => x.Reason != "New").ToList();
            }
            if (ViewBag.ShowEnd != "true")
            {
                employeeData = (List<Select_PHRST_Mismatched_Employees>)employeeData.Where(x => x.Reason != "End").ToList();
            }

            foreach (var employee in employeeData)
            {

                // Save value in session so that employeeid is not sent through URL 
                _appUser.SaveEncryptedValue(_httpContextAccessor.HttpContext, employee.Slug.ToString(), employee.EmployeeId.ToString());

            }


            return View(employeeData.ToList());
        }

        [DisplayName("Menu")]
        public IActionResult Menu()

        {


            return View(_appUser);
        }
    }
}
