namespace StaffManagement.Core.Models
{
    /// <summary>Ported from Models/TitleModel.cs.</summary>
    public class TitleModel
    {
        public int TitleId { get; set; }
        public int? ParentTitleId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? PrefixText { get; set; }
        public bool Active { get; set; }
        public bool Selected { get; set; }
        public int TitleCount { get; set; }
    }
}
