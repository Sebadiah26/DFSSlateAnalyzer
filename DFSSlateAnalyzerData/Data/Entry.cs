using System.ComponentModel.DataAnnotations;

namespace DFSSlateAnalyzerData.Data
{
    public class Entry 
    {
        [Key]
        public int EntryID { get; set; }
        public string? Name { get; set; }
        public string? TimeRemaining { get; set; }
        public string? Lineup { get; set; }
        public decimal? Points { get; set; }
        public virtual List<EntryMember>? EntryMembers { get; set; }


        public int Rank { get; set; }


        //public Entry()
        //{
        //    EntryMembers = new List<EntryMember>();
        //}

    }
}
