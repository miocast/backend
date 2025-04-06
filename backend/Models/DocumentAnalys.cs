using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class DocumentAnalys
    {
        // TODO: Add many to one relationship
        [Key]
        public string Id { get; set; }
        public string DocumentId { get; set; }
        public string Text { get; set; }
        public string Explanation { get; set; }
        public string Regulation { get; set; }
    }
}
