using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace Orbita.Contracts.Commands.Projects;

public sealed record CreateProjectCommand(
    string Client,
    string Name,
    string? Reference,
    DateOnly StartDate,
    DateOnly ContractualEndDate,
    string? ContractType) : ICommand<Result<Guid>>;
