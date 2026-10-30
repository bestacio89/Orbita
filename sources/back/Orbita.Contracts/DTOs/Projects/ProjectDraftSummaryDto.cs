using Orbita.Domain.Enums;

namespace Orbita.Contracts.DTOs.Projects;

public sealed record ProjectDraftSummaryDto(
    Guid Id,
    string Client,
    string ProjectName,
    string? Reference,
    DraftOrigin Origin,
    string? Source,
    Guid AuthorUserId,
    Guid? CoAuthorUserId,
    DateTimeOffset CreatedAt,
    DateTimeOffset ModifiedAt,
    Guid? ModifiedByUserId,
    int Version,
    bool IsAuthor);
