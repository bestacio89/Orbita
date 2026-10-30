namespace Orbita.Contracts.DTOs.Projects;

public sealed record CurrentUserDto(
    int ContractVersion,
    Guid UserId,
    Guid TenantId,
    Guid ObjectId,
    string DisplayName,
    string Email,
    IReadOnlyCollection<string> GlobalRoles,
    IReadOnlyCollection<string> Capabilities);
