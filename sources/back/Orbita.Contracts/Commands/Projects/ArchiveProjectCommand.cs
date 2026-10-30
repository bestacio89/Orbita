using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace Orbita.Contracts.Commands.Projects;

public sealed record ArchiveProjectCommand(
    Guid ProjectId,
    int ExpectedVersion) : ICommand<Result>;
