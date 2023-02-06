using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DFSSlateAnalyzerData.Data
{
    public class DFSPlayer 
    {
        [Key]
        public int? PlayerID { get; set; }
        public string? BMFirstName { get; set; }
        public string? BMLastName { get; set; }
        public string? DKPlayerName { get; set; }
        public string? Team { get; set; }
        public string? RosterPosition { get; set; }


    } 
}
