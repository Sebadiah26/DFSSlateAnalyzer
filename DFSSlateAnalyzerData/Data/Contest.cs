using System.ComponentModel.DataAnnotations.Schema;

namespace DFSSlateAnalyzerData.Data
{
    public class Contest 
    {
        public int ContestID { get; set; }
        public string? Name { get; set; }
        public decimal? PercentComplete { get; set; }
        public int? Size { get; set; }
        public decimal? Fee { get; set; }

        public virtual List<Entry>? Entries { get; set; }
        public virtual List<Player>? ContestPlayers { get; set; }

        //public Contest()
        //{
        //    Entries = new List<Entry>();
        //    ContestPlayers = new List<Player>();
        //}

    }
}
