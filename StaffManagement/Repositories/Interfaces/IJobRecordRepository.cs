
using StaffManagement.Data;
using StaffManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StaffManagement.Repositories.Interfaces
{
    public interface IJobRecordRepository
    {

        List<dynamic> GetJobRecordsNeedingReview();

        int JobRecordCount(int systemid);

        void EditJobRecord(AccountModel accountModel, string comment, bool expireJobOnly);

        int AddJobRecord(JobRecord jobrecord, string comment);

        Task<List<JobRecordModel>> GetJobRecords(int systemid, int jobrecordid);


    }
}
