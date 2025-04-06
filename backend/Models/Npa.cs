using System.ComponentModel.DataAnnotations;

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
}
