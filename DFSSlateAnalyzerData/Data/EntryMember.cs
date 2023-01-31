using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DFSSlateAnalyzerData.Data
{
    public class EntryMember 
    {
      
        public Int64 EntryID { get; set; }
       
        public string? EntryMemberPlayerName { get; set; }
        public int LineupSlot { get; set; }
        public string? Position { get; set; }
        public Entry? Entry { get; set; }    
    }
}
