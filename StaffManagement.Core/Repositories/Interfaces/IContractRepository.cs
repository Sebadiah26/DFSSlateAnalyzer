using StaffManagement.Data;

namespace StaffManagement.Core.Repositories.Interfaces
{
    /// <summary>
    /// New interface - the legacy ContractController talked to accountmanagementContext directly
    /// instead of going through a repository (a pre-existing inconsistency flagged during planning).
    /// This normalizes it into the same repository pattern every other feature area uses, with the
    /// same operations the controller performed.
    /// </summary>
    public interface IContractRepository
    {
        Task<List<Contract>> GetAllAsync();
        Task<Contract?> GetByContractorIdAsync(int contractorId);
        Task<Contract?> GetByIdAsync(int contractorId, int contractorJobId);
        Task<int> CreateAsync(Contract contract, Contractor_staff contractor);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int contractorId);
        Task<bool> ExistsAsync(int contractorId, int contractorJobId);
    }
}
