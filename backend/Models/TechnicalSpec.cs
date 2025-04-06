using System.ComponentModel.DataAnnotations;
using backend.Enums;

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
        public string Description { get; set; } = "Desc";
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        public string Category { get; set; } = "Default";
        public TechStatus Status { get; set; } = TechStatus.InWork;
        public string Link { get; set; }
    }
}
