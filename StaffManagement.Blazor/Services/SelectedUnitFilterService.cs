namespace StaffManagement.Blazor.Services
{
    /// <summary>
    /// Scoped, per-circuit "which department/unit am I filtering by" state - replaces
    /// AppUser.SelectedUnitId (session-backed in the source). Analogous to the reference app's
    /// ContestStateService: a trivial POCO store with a change event, consumed by EmployeeFilter
    /// (the widget that sets it) and any page whose list/dashboard should be scoped to a unit
    /// (Home dashboard, Employees/Index).
    /// </summary>
    public class SelectedUnitFilterService
    {
        /// <summary>0 means "All Departments" (matches the legacy dropdown's DefaultValue = "0").</summary>
        public int SelectedUnitId { get; private set; }

        public event Action? OnChange;

        public void SetUnit(int unitId)
        {
            if (unitId != SelectedUnitId)
            {
                SelectedUnitId = unitId;
                OnChange?.Invoke();
            }
        }
    }
}
