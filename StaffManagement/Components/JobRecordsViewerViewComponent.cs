using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StaffManagement.Models;
using StaffManagement.Repositories.Interfaces;
using System.Threading.Tasks;

namespace StaffManagement.Components
{
    public class JobRecordsViewerViewComponent : BaseViewComponent
    {
        private readonly IJobRecordRepository _jobrecordRepository;



        public JobRecordsViewerViewComponent(IJobRecordRepository jobrecordRepository, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, AppUser appUser) : base(httpContextAccessor, memoryCache, appUser)
        {
            _jobrecordRepository = jobrecordRepository;





        }
        public async Task<IViewComponentResult> InvokeAsync(int systemid, int jobrecordid)
        {
            var jobrecorddata = await _jobrecordRepository.GetJobRecords(systemid, jobrecordid);


            return View(jobrecorddata);

        }
    }
}
