using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using Orbita.Contracts.DTOs.Projects;

namespace Orbita.Contracts.Queries.Projects;

public sealed record GetProjectByIdQuery(Guid ProjectId)
    : IQuery<Result<ProjectDetailsDto>>;
