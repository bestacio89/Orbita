using Orbita.Domain.Enums;

namespace Orbita.Contracts.DTOs.Projects;

public sealed record ProjectDraftDto(
    Guid Id,
    string Client,
    string ProjectName,
    string? Reference,
    DraftOrigin Origin,
    string? Source,
    string Content,
    Guid AuthorUserId,
    Guid? CoAuthorUserId,
    DateTimeOffset CreatedAt,
    DateTimeOffset ModifiedAt,
    Guid? ModifiedByUserId,
    int Version,
    bool IsAuthor);
