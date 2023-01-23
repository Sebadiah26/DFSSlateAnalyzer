using System.ComponentModel.DataAnnotations.Schema;

namespace DFSSlateAnalyzerData.Data
{
    public class SlateGame 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Contest? SlateGameContest{ get; set; }   
        public string? Hometeam { get; set; }    
        public string? VisitingTeam { get; set; }    
        public DateTime StartTime { get; set; } 


    }
}
