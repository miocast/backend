using System.ComponentModel.DataAnnotations;

namespace backend.Contracts
{
    public record GetTechnicalSpecRequest(string? Search, [Required] int Size, [Required] int Page);
}
