using backend.Models;

namespace backend.Contracts
{
    public record GetTechnicalSpecResponse(List<TechnicalSpecDto> technicalSpecs);
   
}
