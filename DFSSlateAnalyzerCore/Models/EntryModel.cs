namespace DFSSlateAnalyzerCore.Models
{
    public class EntryModel : BaseEntity
    {
        public int ID { get; set; }
        public System.Int64 EntryID { get; set; }
        public string? Name { get; set; }
        public decimal? TimeRemaining { get; set; }
        public string? Lineup { get; set; }
        public decimal? Points { get; set; }
        public virtual List<EntryMemberModel> EntryMembers { get; set; }


        public int Rank { get; set; }


        public EntryModel()
        {
            EntryMembers = new List<EntryMemberModel>();
        }

    }
}
