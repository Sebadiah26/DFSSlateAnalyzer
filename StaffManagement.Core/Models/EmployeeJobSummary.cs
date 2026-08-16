namespace StaffManagement.Core.Models
{
    /// <summary>
    /// Flat projection of one live PHRST job assignment (hr.EmployeeJob) for a given employee, backing
    /// the Employees/Edit "PHRST reconciliation" comparison card. Ports legacy EmployeeJobModel, but
    /// uses the row's own JobId as the identity/selection key instead of the source's fragile
    /// "FsfBuildingId#PersonnelCode#Jobcode#UnitId#EffectiveDate" string-split composite key.
    /// </summary>
    public class EmployeeJobSummary
    {
        public int EmployeeId { get; set; }
        public int JobId { get; set; }
        public string? FsfBuildingId { get; set; }
        public string? PersonnelCode { get; set; }
        public int Jobcode { get; set; }
        public int? UnitId { get; set; }
        public DateTime? EffectiveDate { get; set; }

        public string BuildingDescription { get; set; } = string.Empty;
        public string PersonnelCodeDescription { get; set; } = string.Empty;
        public string JobcodeDescription { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
    }
}
