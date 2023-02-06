using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DFSSlateAnalyzerData.Data
{
    public class Player 
    {
        
        public string? PlayerName { get; set; }
        public string? Salary { get; set; }
        public decimal? Points { get; set; }
        public decimal? ProjectedPoints { get; set; }
        public string? Position { get; set; }

        // public string Player { get; set; }

        public string? RosterPosition { get; set; }

        public string? Drafted { get; set; }

        public string? FPTS { get; set; }
        [ForeignKey("ContestID")]
        public virtual Contest? Contest { get; set; }

        public System.Int64 ContestID { get; set; }

        public int? PlayerID { get; set; }

        public decimal? BMProjection { get; set; }  
        public string? OPP { get; set; } 

    }
}
