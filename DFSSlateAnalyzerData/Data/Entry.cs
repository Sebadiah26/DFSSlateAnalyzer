using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DFSSlateAnalyzerData.Data
{
    public class Entry 
    {
       
        public int ID { get; set; }
        public System.Int64 EntryID { get; set; }
        public string? Name { get; set; }
        public decimal? TimeRemaining { get; set; }
        public string? Lineup { get; set; }
        public decimal? Points { get; set; }
        public virtual List<EntryMember>? EntryMembers { get; set; } = new List<EntryMember>();
        [ForeignKey("ContestID")]
        public virtual Contest? Contest { get; set; }
       
        public System.Int64 ContestID { get; set; }
        public int Rank { get; set; }



    }
}
 