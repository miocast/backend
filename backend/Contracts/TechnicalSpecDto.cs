using backend.Enums;

namespace backend.Contracts
{
    public record TechnicalSpecDto(Guid Id, string UserId, string Name, string Link, string Description, DateTime LastUpdate, string Category, TechStatus Status);
}
