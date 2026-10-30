using Franz.Common.Mediator.Dispatchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbita.API.Extensions;
using Orbita.Contracts.Commands.Projects;
using Orbita.Contracts.DTOs.Projects;
using Orbita.Contracts.Queries.Projects;

namespace Orbita.API.Controllers;

[ApiController]
[Authorize]
[Route("api/projects")]
public sealed class ProjectsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProjectSummaryDto>>> List(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100)
        {
            return BadRequest("Page must be positive and page size must be between 1 and 100.");
        }

        var result = await dispatcher.SendAsync(new ListProjectsQuery(search, page, pageSize), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpGet("{projectId:guid}")]
    public async Task<ActionResult<ProjectDetailsDto>> GetById(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(new GetProjectByIdQuery(projectId), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(command, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { projectId = result.Value }, result.Value)
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpPut("{projectId:guid}/dates")]
    public async Task<ActionResult> UpdateDates(
        Guid projectId,
        [FromBody] UpdateProjectDatesCommand command,
        CancellationToken cancellationToken)
    {
        if (projectId != command.ProjectId)
        {
            return BadRequest("Route ID and command ID must match.");
        }

        var result = await dispatcher.SendAsync(command, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpDelete("{projectId:guid}")]
    public async Task<ActionResult> Archive(
        Guid projectId,
        [FromQuery] int expectedVersion,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(new ArchiveProjectCommand(projectId, expectedVersion), cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpPost("{projectId:guid}/assignments")]
    public async Task<ActionResult> AssignUser(
        Guid projectId,
        [FromBody] AssignProjectUserCommand command,
        CancellationToken cancellationToken)
    {
        if (projectId != command.ProjectId)
        {
            return BadRequest("Route ID and command ID must match.");
        }

        var result = await dispatcher.SendAsync(command, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpDelete("{projectId:guid}/assignments/{userId:guid}")]
    public async Task<ActionResult> RemoveAssignment(
        Guid projectId,
        Guid userId,
        [FromQuery] int expectedVersion,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(
            new RemoveProjectAssignmentCommand(projectId, userId, expectedVersion),
            cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResultHttpMapper.FromError(result.Error);
    }
}
