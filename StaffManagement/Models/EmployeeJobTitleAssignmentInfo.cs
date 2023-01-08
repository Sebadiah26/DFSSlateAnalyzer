namespace StaffManagement.Models
{
    public enum JobTitleMatchSource : int
    {
        JobTitle_Override = 1,
        By_PersonnelCode_and_JobCode = 2,
        By_JobCode_only = 3,
        By_PersonnelCode_only = 4,
        By_JobCode_Description = 5
    }

    public class EmployeeJobTitleAssignmentInfo
    {
        private string _EffectiveJobTitle;

        public int SystemId { get; set; }
        public int EmployeeId { get; set; }
        public string MatchedJobTitle { get; set; }
        public string JobTitleManual { get; set; }
        public int MatchSourceID { get; set; }
        public string jobcode { get; set; }
        public string personnel_code { get; set; }

        public string EffectiveJobTitle
        {

            get
            {
                if (this.JobTitleManual == "")
                { _EffectiveJobTitle = this.MatchedJobTitle; }
                else
                { _EffectiveJobTitle = this.JobTitleManual; }

                return _EffectiveJobTitle;
            }
        }
    }
}



