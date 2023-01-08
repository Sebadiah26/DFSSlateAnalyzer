using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StaffManagement.Data;
using StaffManagement.Models;
using StaffManagement.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.Controllers
{
    public class ContractController : Controller
    {
        private readonly accountmanagementContext _db;
        private readonly IAuditLoggerService _log = null;
        private readonly IHttpContextAccessor _httpContextAccessor = null;
        private AppUser _appUser = null;

        public ContractController(accountmanagementContext context, IAuditLoggerService log, IHttpContextAccessor contextAccessor)
        {
            _db = context;
            _log = log;
            _httpContextAccessor = contextAccessor;

            AppUser appuser = new AppUser(_httpContextAccessor);
            _appUser = appuser;


            _appUser.GetAppUserData(_httpContextAccessor.HttpContext);
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var contract = _db.Contracts
                .Include(c => c.Contractor)
                .Include(c => c.PrimaryUnit)
                .OrderBy(x => x.PrimaryUnit.Name)
                .ThenBy(x => x.JobTitle)
                .ThenBy(x => x.Contractor.LastName);

            return View(await contract.ToListAsync());
        }

        // GET: Contract/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _db.Contracts
                .Include(c => c.Contractor)
                .Include(c => c.PrimaryUnit)
                .FirstOrDefaultAsync(m => m.ContractorId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contract/Create
        public IActionResult Create()
        {
            // ViewData["ContractorId"] = new SelectList(_db.staff, "ContractorId", "FirstName");
            ViewData["PrimaryUnitId"] = new SelectList(_db.Units, "UnitId", "Name");
            return View();
        }

        // POST: Contract/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractorId,ContractorJobId,StartDate,EndDate,JobTitle,PrimaryUnitId,LastUpdate,LastUpdateUser,IsActive")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                var contractor = new Contractor_staff();

                contractor.FirstName = _httpContextAccessor.HttpContext.Request.Form["Contractor.FirstName"].ToString();
                contractor.MiddleName = _httpContextAccessor.HttpContext.Request.Form["Contractor.MiddleName"].ToString();
                contractor.LastName = _httpContextAccessor.HttpContext.Request.Form["Contractor.LastName"].ToString();
                contractor.SuffixName = _httpContextAccessor.HttpContext.Request.Form["Contractor.SuffixName"].ToString();
                contractor.Nickname = _httpContextAccessor.HttpContext.Request.Form["Contractor.Nickname"].ToString();
                contractor.LastUpdate = DateTime.Now;
                _db.Add(contractor);
                await _db.SaveChangesAsync();

                contract.ContractorId = contractor.ContractorId;
                contract.ContractorJobId = 1;
                contract.LastUpdate = DateTime.Now;
                contract.LastUpdateUser = _appUser.CurrentUser;

                _db.Add(contract);
                await _db.SaveChangesAsync();






                _log.Log((int)ActionTypes.Create, "contractor.Contract", contract.ContractorId.ToString(), null);

                return RedirectToAction(nameof(Index));
            }
            // ViewData["ContractorId"] = new SelectList(_db.staff, "ContractorId", "FirstName", contract.ContractorId);
            ViewData["PrimaryUnitId"] = new SelectList(_db.Units, "UnitId", "AdgroupName", contract.PrimaryUnitId);

            return View(contract);
        }

        // GET: Contract/Edit/5
        public async Task<IActionResult> Edit(int? contractorid, int contractorjobid)
        {
            if (contractorid == null)
            {
                return NotFound();
            }

            var contract = await _db.Contracts
                                        .Include(x => x.Contractor)
                                        .Where(x => x.ContractorId == contractorid && x.ContractorJobId == contractorjobid)
                                        .SingleOrDefaultAsync();
            if (contract == null)
            {
                return NotFound();
            }

            // ViewData["ContractorId"] = new SelectList(_db.staff, "ContractorId", "FirstName", contract.ContractorId);
            ViewData["PrimaryUnitId"] = new SelectList(_db.Units, "UnitId", "Name", contract.PrimaryUnitId);
            return View(contract);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int contractorid, int contractorjobid, Contract contract)
        {
            if (contractorid != contract.ContractorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    contract.LastUpdateUser = _appUser.CurrentUser;
                    contract.LastUpdate = DateTime.Now;


                    _db.Update(contract);
                    await _db.SaveChangesAsync();

                    _log.Log((int)ActionTypes.Edit, "contractor.Contract", contract.ContractorId.ToString(), null);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.ContractorId, contract.ContractorJobId))
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
            // ViewData["ContractorId"] = new SelectList(_db.staff, "ContractorId", "FirstName", contract.ContractorId);
            ViewData["PrimaryUnitId"] = new SelectList(_db.Units, "UnitId", "Name", contract.PrimaryUnitId);
            return View(contract);
        }

        // GET: Contract/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _db.Contracts
                .Include(c => c.Contractor)
                .Include(c => c.PrimaryUnit)
                .FirstOrDefaultAsync(m => m.ContractorId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contract/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _db.Contracts.FindAsync(id);
            _db.Contracts.Remove(contract);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id, int id2)
        {
            return _db.Contracts.Any(e => e.ContractorId == id && e.ContractorJobId == id2);
        }
    }
}
