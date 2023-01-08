using DFSSlateAnalyzerCore.Models;

namespace DFSSlateAnalyzerCore.Repositories.Interfaces
{
    public interface ISlateRepository
    {
        Task<Contest> LoadContest(int ID);

    }
}
