using System.ComponentModel.DataAnnotations.Schema;

namespace DFSSlateAnalyzerCore.Models
{
    public class PlayerModel 
    {
        
        public string? PlayerName { get; set; }
        public string? Salary { get; set; }
        public decimal? Points { get; set; }
        public decimal? ProjectedPoints { get; set; }
        public string? Position { get; set; }

        public string? RosterPosition { get; set; }

        public string? Drafted { get; set; }

        public string? FPTS { get; set; }

    }
}
