namespace DFSSlateAnalyzerCore.Models
{
    public class EntryMemberModel : BaseEntity
    {

        public int EntryId { get; set; }

        public int LineupSlot { get; set; }
        public string? Position { get; set; }
        public string? EntryMemberPlayerName { get; set; }
    }
}
