namespace DFSSlateAnalyzerCore.Models
{
    public class EntryMember : BaseEntity
    {

        public string? EntryId { get; set; }

        public int LineupSlot { get; set; }
        public string? Position { get; set; }
        public string? Player { get; set; }
    }
}
