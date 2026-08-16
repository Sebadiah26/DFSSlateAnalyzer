namespace StaffManagement.Core.Models
{
    /// <summary>Ported from Models/TitleGroupModel.cs.</summary>
    public class TitleGroupModel
    {
        public int TitleGroupId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool Active { get; set; }
        public bool Selected { get; set; }
    }
}
