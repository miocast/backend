
using Microsoft.AspNetCore.Identity;
using System.Numerics;

namespace backend.Models
{
    public class User : IdentityUser
    {
        public string? Initials { get; set; }
    }
}
