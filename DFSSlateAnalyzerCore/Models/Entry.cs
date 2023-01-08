namespace DFSSlateAnalyzerCore.Models
{
    public class Entry : BaseEntity
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? TimeRemaining { get; set; }
        public string? Lineup { get; set; }
        public decimal? Points { get; set; }
        public virtual List<EntryMember> EntryMembers { get; set; }


        public int Rank { get; set; }


        public Entry()
        {
            EntryMembers = new List<EntryMember>();
        }

    }
}
