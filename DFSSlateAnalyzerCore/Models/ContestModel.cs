namespace DFSSlateAnalyzerCore.Models
{
    public class ContestModel : BaseEntity
    {
        public int ContestID { get; set; }
        public string? Name { get; set; }
        public decimal? PercentComplete { get; set; }
        public int? Size { get; set; }
        public decimal? Fee { get; set; }

        public virtual List<EntryModel> Entries { get; set; }
        public virtual List<PlayerModel> ContestPlayers { get; set; }

        public ContestModel()
        {
            Entries = new List<EntryModel>();
            ContestPlayers = new List<PlayerModel>();
        }

    }
}
