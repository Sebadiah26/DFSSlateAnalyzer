namespace DFSSlateAnalyzerCore.Models
{
    public class ContestModel : BaseEntity
    {
        public int ID { get; set; }
        public System.Int64 ContestID { get; set; }
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

        public ContestModel(Int64 contestID, string? name, decimal? percentComplete, int? size, decimal? fee)
        {
            ContestID= contestID;   
            Name= name; 
            PercentComplete= percentComplete;   
            Size= size; 
            Fee= fee;   

            Entries = new List<EntryModel>();
            ContestPlayers = new List<PlayerModel>();
        }

    }
}
