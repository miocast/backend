using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class TechnicalSpec
    {
        public TechnicalSpec(string userId, string name, string link)
        {
            UserId = userId;
            Name = name;
            Link = link;
        }

        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
    }
}
