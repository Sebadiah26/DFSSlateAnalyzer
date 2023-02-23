using DFSSlateAnalyzerCore.Models;
using DFSSlateAnalyzerData.Data;

namespace DFSSlateAnalyzerCore.Repositories.Interfaces
{
    public interface ISlateRepository
    {
        Task<Contest> UploadContest(DateTime date, Int64 ID);
        Task<ContestModel> GetContest(DateTime date, Int64 ID);
        void SaveContestToDatabase(Contest contest);
        void UploadProjections(DateTime date, Int64 ID, Stream stream);
        void UploadPlayers(DateTime date, Int64 ID);
        void UploadSlate(DateTime date, Int64 ID);
    }
}
