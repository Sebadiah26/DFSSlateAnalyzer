using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DFSSlateAnalyzerData.Data
{
    public class Owner 
    {
        [Key]
        public int OwnerId { get; set; }

        public string? Name { get; set; }

        public virtual List<OwnerContest>? ContestOwners { get; set; }
    }
}
