#nullable disable

namespace CSVReader.Models
{
    public partial class Entry
    {
        public Entry()
        {
            EntryMembers = new List<EntryMember>();
        }

        public int Rank { get; set; }

        public string EntryId { get; set; }

        public string EntryName { get; set; }
        public string TimeRemaining { get; set; }
        public string Points { get; set; }
        public string Lineup { get; set; }

        public virtual List<EntryMember> EntryMembers { get; set; }


    }
}
