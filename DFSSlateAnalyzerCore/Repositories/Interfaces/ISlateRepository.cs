using DFSSlateAnalyzerCore.Models;

namespace DFSSlateAnalyzerCore.Repositories.Interfaces
{
    public interface ISlateRepository
    {
        Task<ContestModel> LoadContest(int ID);

    }
}
