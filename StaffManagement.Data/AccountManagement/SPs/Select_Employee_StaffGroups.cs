#nullable disable

namespace StaffManagement.Data
{
    public partial class Select_Employee_StaffGroups
    {
        public Select_Employee_StaffGroups()
        {

        }
        public int SystemId { get; set; }
        public int StaffGroupAssociationId { get; set; }

        public int StaffGroupId { get; set; }

        public string GroupName { get; set; }


        public string MatchedBy { get; set; }



    }
}
