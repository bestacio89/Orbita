using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace Orbita.Contracts.Commands.Projects;

public sealed record UpdateProjectDatesCommand(
    Guid ProjectId,
    int ExpectedVersion,
    DateOnly? StartDate,
    DateOnly? ContractualEndDate,
    DateOnly? ForecastEndDate) : ICommand<Result>;
