using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using Orbita.Domain.Enums;

namespace Orbita.Contracts.Commands.Projects;

public sealed record CreateProjectDraftCommand(
    string Client,
    string ProjectName,
    string? Reference,
    DraftOrigin Origin,
    string Content) : ICommand<Result<Guid>>;

public sealed record InviteDraftCoAuthorCommand(
    Guid DraftId,
    Guid CoAuthorUserId,
    int ExpectedVersion) : ICommand<Result>;

public sealed record RemoveDraftCoAuthorCommand(
    Guid DraftId,
    int ExpectedVersion) : ICommand<Result>;

public sealed record UpdateProjectDraftContentCommand(
    Guid DraftId,
    string Content,
    int ExpectedVersion) : ICommand<Result>;

public sealed record DeleteProjectDraftCommand(
    Guid DraftId,
    int ExpectedVersion) : ICommand<Result>;
