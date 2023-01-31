namespace DFSSlateAnalyzerCore.Models
{
    public class EntryMemberModel : BaseEntity
    {

        public System.Int64 EntryId { get; set; }

        public int LineupSlot { get; set; }
        public string? Position { get; set; }
        public string? EntryMemberPlayerName { get; set; }
    }
}
