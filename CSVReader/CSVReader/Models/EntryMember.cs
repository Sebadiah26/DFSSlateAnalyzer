#nullable disable

namespace CSVReader.Models
{
    public partial class EntryMember
    {
        public EntryMember()
        {
        }


        public string EntryId { get; set; }

        public int LineupSlot { get; set; }
        public string Position { get; set; }
        public string Player { get; set; }


    }
}
