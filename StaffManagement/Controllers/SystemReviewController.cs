using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StaffManagement.Common;
using StaffManagement.Data;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using StaffManagement.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.Controllers
{
    [DisplayName("Admin")]
    [CheckPermission]
    public class SystemReviewController : BaseController
    {
        private readonly accountmanagementContext _context;
        private readonly ISystemReviewRepository _systemReviewRepository = null;
        private readonly IAccountRepository _accountRepository = null;
        public SystemReviewController(ISystemReviewRepository systemReviewRepository, IAccountRepository accountRepository, accountmanagementContext context, IAuditLoggerService log, IHttpContextAccessor contextAccessor, URLEncrypter protector, AppUser appUser) : base(log, contextAccessor, protector, appUser)
        {
            _context = context;
            _systemReviewRepository = systemReviewRepository;
            _accountRepository = accountRepository;
        }



        [DisplayName("Job Titles")]
        //GET
        public async Task<ViewResult> JobTitleAdmin(int selectedtitlegroupId, int selectedtitleId, string selectedtitletext, string resultMessage, string mode)


        {

            string[] accordionButton = new string[3];
            string[] ariaExpanded = new string[3];
            string[] accordionCollapse = new string[3];

            accordionButton[0] = "collapsed";
            ariaExpanded[0] = "false";
            accordionCollapse[0] = "";

            accordionButton[1] = "collapsed";
            ariaExpanded[1] = "false";
            accordionCollapse[1] = "";


            var titleadminModel = new TitleAdminModel();



            switch (mode)
            {
                case "Activate":
                    {
                        resultMessage = _systemReviewRepository.EditTitleStatus(selectedtitleId, true);

                        break;
                    }
                case "DeActivate":
                    {
                        resultMessage = _systemReviewRepository.EditTitleStatus(selectedtitleId, false);

                        break;
                    }
            }



            // loading values for dropdownlists

            titleadminModel.TitleGroups = await _systemReviewRepository.GetTitleGroups();

            if (selectedtitlegroupId != 0)
            {

                titleadminModel.SelectedTitleGroupId = selectedtitlegroupId;
                titleadminModel.AllTitles = await _systemReviewRepository.GetAllTitles(titleadminModel.SelectedTitleGroupId);

                accordionButton[0] = "";
                ariaExpanded[0] = "true";
                accordionCollapse[0] = "show";

                accordionButton[1] = "collapsed";
                ariaExpanded[1] = "false";
                accordionCollapse[1] = "";


            }



            if (selectedtitleId != 0)
            {

                titleadminModel.SelectedTitleId = selectedtitleId;
                titleadminModel.SelectedTitleText = selectedtitletext;
                titleadminModel.AllTitles = await _systemReviewRepository.GetAllTitles(titleadminModel.SelectedTitleGroupId);

                accordionButton[1] = "";
                ariaExpanded[1] = "true";
                accordionCollapse[1] = "show";

                accordionButton[0] = "collapsed";
                ariaExpanded[0] = "false";
                accordionCollapse[0] = "";


            }

            if (selectedtitleId == 0 && selectedtitlegroupId == 0)
            {
                titleadminModel.AllTitles = await _systemReviewRepository.GetAllTitles(0);
            }





            ViewBag.accordionButton = accordionButton;
            ViewBag.ariaExpanded = ariaExpanded;
            ViewBag.accordionCollapse = accordionCollapse;

            TempData["ResultMessage"] = resultMessage;


            return View(titleadminModel);

        }



        [HttpPost]
        public async Task<ActionResult> JobTitleAdmin(TitleAdminModel titleadminModel, string submitAction, string newTitleText, string editTitleText)
        {

            string resultMessage = "";

            if (submitAction == "Save Titles")
            {
                // get saved values from database
                var existingtitles = await _systemReviewRepository.GetAllTitles(titleadminModel.SelectedTitleGroupId);
                resultMessage = "No changes needed.";

                // iterate through saved values
                foreach (var existingtitle in existingtitles)
                {
                    // if selected in database but not selected on submitted form
                    if (existingtitle.Selected && titleadminModel.AllTitles.Find(x => x.TitleId == existingtitle.TitleId).Selected == false)
                    { // remove link
                        _systemReviewRepository.RemoveTitleGroupTitle(titleadminModel.SelectedTitleGroupId, existingtitle.TitleId);
                        resultMessage = "Changes saved successfully.";
                    }

                    // if not selected in database but selected on submitted form
                    if (existingtitle.Selected == false && titleadminModel.AllTitles.Find(x => x.TitleId == existingtitle.TitleId).Selected == true)
                    { // add link
                        _systemReviewRepository.AddTitleGroupTitle(titleadminModel.SelectedTitleGroupId, existingtitle.TitleId);
                        resultMessage = "Changes saved successfully.";
                    }

                }


                //bug with posting checkboxlist values requires redirect
                return RedirectToAction("JobTitleAdmin", new { SelectedTitleGroupId = titleadminModel.SelectedTitleGroupId, ResultMessage = resultMessage });
            }

            if (submitAction == "Save Title")
            {

                resultMessage = _systemReviewRepository.EditTitle(titleadminModel.SelectedTitleId, editTitleText);

                return RedirectToAction("JobTitleAdmin",
                    new { SelectedTitleId = titleadminModel.SelectedTitleId, SelectedTitleText = editTitleText, ResultMessage = resultMessage });
            }


            if (submitAction == "Save New Title")
            {
                int newtitleid = _systemReviewRepository.AddTitle(newTitleText);
                resultMessage = "Changes saved successfully.";

                return RedirectToAction("JobTitleAdmin", new { SelectedTitleId = newtitleid, SelectedTitleText = newTitleText, ResultMessage = resultMessage });
            }


            if (titleadminModel.SelectedTitleGroupId != 0)
            {
                //bug with posting checkboxlist values- need to redirect when changing titlegroupid
                return RedirectToAction("JobTitleAdmin", new { SelectedTitleGroupId = titleadminModel.SelectedTitleGroupId });



            }

            titleadminModel.TitleGroups = await _systemReviewRepository.GetTitleGroups();


            return View(titleadminModel);


        }



        [DisplayName("Group Admin")]
        // GET: SystemReview
        public async Task<IActionResult> Index()
        {
            List<dynamic> groupdatalist = _systemReviewRepository.GetActiveDirectoryGroups();
            //IPagedList<StaffGroupAssociation> staffgroupassociationdatalist =  _systemReviewRepository.GetStaffGroupAssociations();
            List<GroupModel> groupmodellist = new List<GroupModel>();
            // List<StaffGroupAssociationModel> staffgroupassociationmodellist = new List<StaffGroupAssociationModel>();

            SelectList allemployees = null;
            SelectList runninglistofemployees = null;

            SelectList remaininglistofemployees = null;


            if (groupdatalist?.Any() == true)
            {
                int currentgroupid = 0;
                int currentgrouptotal = 0;

                foreach (var group in groupdatalist)
                {
                    GroupModel groupmodel = new GroupModel();

                    groupmodel.ObjectGuid = group.ObjectGuid;
                    groupmodel.CommonName = group.CommonName;
                    // groupmodel.OrganizationalUnit.ObjectGuid = group.ObjectGuid;
                    groupmodel.OrganizationalUnit.OrganizationalUnitName = group.OrganizationalUnitName;
                    groupmodel.ActualMatches = await _systemReviewRepository.GetEmployeesForGroupMembership(groupmodel.ObjectGuid);
                    groupmodel.ActualMatchesCount = groupmodel.ActualMatches.Count();

                    StaffGroup staffgroupdata = _systemReviewRepository.GetStaffGroup(group.ObjectGuid);

                    if (staffgroupdata != null)
                    {
                        int currentstaffgroupid = 0;
                        int currentstaffgrouptotal = 0;

                        groupmodel.StaffGroup.StaffGroupId = staffgroupdata.StaffGroupId;
                        groupmodel.StaffGroup.GroupName = staffgroupdata.GroupName;
                        currentstaffgroupid = groupmodel.StaffGroup.StaffGroupId;


                        if (staffgroupdata.StaffGroupAssociations.Any() == true)
                        {


                            foreach (var staffgroupassociation in staffgroupdata.StaffGroupAssociations)
                            {
                                StaffGroupAssociationModel staffgroupassociationmodel = new StaffGroupAssociationModel();
                                //StaffGroupModel staffgroupmodel = new StaffGroupModel();

                                if (staffgroupassociation.StaffGroupId != currentstaffgroupid)
                                {
                                    staffgroupassociationmodel.GroupHeader = true;
                                    staffgroupassociationmodel.PreviousGroupCount = currentstaffgrouptotal;
                                    currentstaffgrouptotal = 0;
                                }

                                staffgroupassociationmodel.StaffGroupAssociationId = staffgroupassociation.StaffGroupAssociationId;
                                staffgroupassociationmodel.StaffGroupId = staffgroupassociation.StaffGroupId;
                                staffgroupassociationmodel.UnitId = staffgroupassociation.UnitId;
                                staffgroupassociationmodel.BuildingId = staffgroupassociation.BuildingId;
                                staffgroupassociationmodel.TitleGroupId = staffgroupassociation.TitleGroupId;
                                staffgroupassociationmodel.TitleId = staffgroupassociation.TitleId;

                                staffgroupassociationmodel.Unit.Units = await _accountRepository.GetUnitsAsync();
                                staffgroupassociationmodel.Building.AllBuildings = await _accountRepository.GetBuildings();
                                staffgroupassociationmodel.Title.AllTitles = await _accountRepository.GetTitles();
                                staffgroupassociationmodel.TitleGroup.TitleGroups = await _accountRepository.GetTitleGroups();
                                staffgroupassociationmodel.CalculatedMatches = await _systemReviewRepository.GetEmployeesForStaffGroupAssociation(staffgroupassociationmodel.StaffGroupAssociationId);
                                staffgroupassociationmodel.EmployeeCount = staffgroupassociationmodel.CalculatedMatches.Count();

                                //staffgroupassociationmodel.StaffGroup.StaffGroupId = staffgroupassociationmodel.StaffGroupId;
                                //staffgroupassociationmodel.StaffGroup.GroupName = staffgroupassociation.StaffGroup.GroupName;

                                // staffgroupassociationmodellist.Add(staffgroupassociationmodel);

                                groupmodel.StaffGroup.StaffGroupAssociations.Add(staffgroupassociationmodel);
                                groupmodel.StaffGroup.EmployeeCount += staffgroupassociationmodel.EmployeeCount;

                                runninglistofemployees = this.AddToRunningListofEmployees(runninglistofemployees, staffgroupassociationmodel);

                                //add unique names to running total dropdown of employees in groups if not staffassociationid of 283 or all params blank


                                currentstaffgroupid = staffgroupassociation.StaffGroupId;
                                currentstaffgrouptotal += staffgroupassociationmodel.EmployeeCount;
                            }
                        }

                    }


                    groupmodellist.Add(groupmodel);


                }
            }

            //  IPagedList<StaffGroupAssociationModel> pagedstaffgroupassociationmodellist = new StaticPagedList<StaffGroupAssociationModel>(staffgroupassociationmodellist,  1, 1000, 1000);
            // IPagedList<GroupModel> pagedgroupmodellist = new StaticPagedList<GroupModel>(groupmodellist, 1, 1000, 1000);

            //get all employees 

            allemployees = await _accountRepository.GetActiveEmployees();

            remaininglistofemployees = GetMissing(allemployees, runninglistofemployees);
            ViewBag.RemainingListofEmployees = remaininglistofemployees;

            //get list of employees in all but not in running count list   

            return View(groupmodellist);
        }


        public SelectList AddToRunningListofEmployees(SelectList runninglistofemployees, StaffGroupAssociationModel model)
        {
            // exclude from list if staffgroupassociation has empty criteria
            if (model.UnitId != null || model.BuildingId != null || model.TitleGroupId != null || model.TitleId == null)
            {
                foreach (var item in model.CalculatedMatches)


                    if (runninglistofemployees == null || (((List<SelectListItem>)runninglistofemployees.Items).Any(x => x.Value == item.Value) == false))
                    {
                        if (runninglistofemployees == null)
                        {
                            SelectListItem listitem = new SelectListItem() { Value = item.Value, Text = item.Text };
                            List<SelectListItem> newList = new List<SelectListItem>();
                            newList.Add(listitem);
                            runninglistofemployees = new SelectList(newList, "Value", "Text", null);
                        }
                        else
                        {
                            ((List<SelectListItem>)runninglistofemployees.Items).Add(new SelectListItem(item.Text, item.Value));
                        }
                    }

            }

            return runninglistofemployees;
        }

        public SelectList GetMissing(SelectList allemployees, SelectList runninglistofemployees)
        {
            SelectList remaininglistofemployees = null;

            foreach (var item in allemployees)
                if (((List<SelectListItem>)runninglistofemployees.Items).Any(x => x.Value == item.Value) == false)
                {
                    if (remaininglistofemployees == null)
                    {
                        SelectListItem listitem = new SelectListItem() { Value = item.Value, Text = item.Text };
                        List<SelectListItem> newList = new List<SelectListItem>();
                        newList.Add(listitem);
                        remaininglistofemployees = new SelectList(newList, "Value", "Text", null);
                    }
                    else
                    {
                        ((List<SelectListItem>)remaininglistofemployees.Items).Add(new SelectListItem(item.Text, item.Value));
                    }

                }

            return remaininglistofemployees;
        }




        // GET: SystemReview/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.ObjectGuid == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // GET: SystemReview/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SystemReview/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObjectGuid,DistinguishedName,SamaccountName,Mail,MailNickname,Name,CommonName,Notes,ManagedByGuid,OrganizationalUnitGuid,Description,IsSecurityGroup,ExtensionAttribute2,Usnchanged,WhenChanged,WhenCreated,IsDeleted")] Group @group)
        {
            if (ModelState.IsValid)
            {
                @group.ObjectGuid = Guid.NewGuid();
                _context.Add(@group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@group);
        }

        // GET: SystemReview/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            return View(@group);
        }

        // POST: SystemReview/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ObjectGuid,DistinguishedName,SamaccountName,Mail,MailNickname,Name,CommonName,Notes,ManagedByGuid,OrganizationalUnitGuid,Description,IsSecurityGroup,ExtensionAttribute2,Usnchanged,WhenChanged,WhenCreated,IsDeleted")] Group @group)
        {
            if (id != @group.ObjectGuid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists((Guid)@group.ObjectGuid))
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
            return View(@group);
        }

        // GET: SystemReview/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.ObjectGuid == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: SystemReview/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var @group = await _context.Groups.FindAsync(id);
            _context.Groups.Remove(@group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(Guid id)
        {
            return _context.Groups.Any(e => e.ObjectGuid == id);
        }

        [DisplayName("Audit Log")]
        //GET  Audit Log page
        public ViewResult AuditLog(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.LogIdSort = String.IsNullOrEmpty(sortOrder) ? " LogId_Desc" : "";
            ViewBag.DateSort = (sortOrder == " Date_Asc_Sort") ? " Date_Desc_Sort" : " Date_Asc_Sort";
            ViewBag.EmployeeSort = (sortOrder == " Emp_Asc_Sort") ? " Emp_Desc_Sort" : " Emp_Asc_Sort";
            ViewBag.UserSort = (sortOrder == " User_Asc_Sort") ? " User_Desc_Sort" : " User_Asc_Sort";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            var auditlog = _systemReviewRepository.GetAuditLog(sortOrder, searchString, pageNumber, pageSize);


            return View(auditlog);
        }


    }
}
