using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DFSSlateAnalyzerData.Data
{
    public class DFSPlayer 
    {
        [Key]
        public int ID { get; set; }
        public int? PlayerID { get; set; }
        public string? BMFirstName { get; set; }
        public string? BMLastName { get; set; }
        public string? DKPlayerName { get; set; }
        public string? Team { get; set; }
        public string? RosterPosition { get; set; }
        public virtual Player? Player { get; set; }

    } 
}
