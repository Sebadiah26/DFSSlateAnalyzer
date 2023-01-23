using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DFSSlateAnalyzerData.Data
{
    public class Owner 
    {
        [Key]
        public string? Name { get; set; }

        public virtual List<Entry>? Entries { get; set; }
    }
}
