using Orbita.Domain.Enums;

namespace Orbita.Contracts.DTOs.Projects;

public sealed record ProjectAssignmentDto(
    Guid UserId,
    ProjectRole Role,
    DateTimeOffset AssignedAt,
    Guid? GrantedByUserId);
