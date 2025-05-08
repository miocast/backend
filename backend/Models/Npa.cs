using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Primitives;

namespace backend.Models
{
    public class Npa
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string FilePath { get; set; }
    }

    public class NpaKnowlabe
    {
        public string Id { get; set; }
        public string  Title { get; set; }
        public string FilePath { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
