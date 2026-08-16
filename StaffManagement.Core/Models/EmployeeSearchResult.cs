namespace StaffManagement.Core.Models
{
    /// <summary>
    /// Flat read-only projection backing Employees/Index (search) and Employees/Contractors, replacing
    /// the source's habit of shoehorning this query's output into a partially-populated AccountModel
    /// (see AccountRepository.GetAccounts/GetContractors) - this is a straight port of that query's
    /// SELECT shape, just as its own type instead of an anonymous-then-AccountModel round trip.
    /// </summary>
    public class EmployeeSearchResult
    {
        public int SystemId { get; set; }
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int AccountTypeId { get; set; }
        public string EmployeeType { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string TitleGroup { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public DateTime? AccountLastUpdate { get; set; }
        public string AccountLastUpdateUser { get; set; } = string.Empty;
        public DateTime? JobRecordLastUpdate { get; set; }
        public string JobRecordLastUpdateUser { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public string AlternateFirstName { get; set; } = string.Empty;
        public string AlternateLastName { get; set; } = string.Empty;
        public string UsernameOverride { get; set; } = string.Empty;
        public int PrimaryUnitId { get; set; }
        public int TitleId { get; set; }
        public int Jobcode { get; set; }
        public string PersonnelCode { get; set; } = string.Empty;
        public int BuildingId { get; set; }
        public string Source { get; set; } = string.Empty;
        public string SubstituteFor { get; set; } = string.Empty;
        public bool UserIsDisabled { get; set; }
        public bool UserIsDeleted { get; set; }
        public string? OrganizationalUnit { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? AccountExpires { get; set; }
    }

    /// <summary>Flat projection backing SearchEmployeesByName (Add/AssignEmployee's employee picker).</summary>
    public class EmployeeSearchMatch
    {
        public int EmployeeId { get; set; }
        public int SystemId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
