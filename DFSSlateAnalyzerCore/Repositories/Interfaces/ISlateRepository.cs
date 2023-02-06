using DFSSlateAnalyzerCore.Models;
using DFSSlateAnalyzerData.Data;

namespace DFSSlateAnalyzerCore.Repositories.Interfaces
{
    public interface ISlateRepository
    {
        Task<Contest> UploadContest(Int64 ID);
        Task<ContestModel> GetContest(Int64 ID);
        void SaveContestToDatabase(Contest contest);
        void UploadProjections(Int64 ID);
    }
}
