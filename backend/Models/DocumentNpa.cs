using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class DocumentNpa
    {
        [Key]
        public string Id { get; set; }
        public string DocumentId { get; set; }
        public string NpaText { get; set; }
        public string Source { get; set; }
        public float DistancePercent { get; set; }
    }
}
