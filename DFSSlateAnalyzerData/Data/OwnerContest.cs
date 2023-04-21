using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DFSSlateAnalyzerData.Data
{
    public class OwnerContest 
    {
        
        public int OwnerId { get; set; }
        public System.Int64 ContestID { get; set; }

        public int OwnerContestCount { get; set; }

        public virtual List<Entry>? ContestEntries { get; set; }
    }
}
