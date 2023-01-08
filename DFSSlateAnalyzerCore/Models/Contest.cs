namespace DFSSlateAnalyzerCore.Models
{
    public class Contest : BaseEntity
    {
        public string? Name { get; set; }
        public decimal? PercentComplete { get; set; }
        public int? Size { get; set; }
        public decimal? Fee { get; set; }

        public virtual List<Entry> Entries { get; set; }
        public virtual List<Player> ContestPlayers { get; set; }

        public Contest()
        {
            Entries = new List<Entry>();
            ContestPlayers = new List<Player>();
        }

    }
}
