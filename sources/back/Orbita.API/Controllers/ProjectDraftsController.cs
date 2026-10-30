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
[Route("api/project-drafts")]
public sealed class ProjectDraftsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProjectDraftSummaryDto>>> List(CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(new ListProjectDraftsQuery(), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpGet("{draftId:guid}")]
    public async Task<ActionResult<ProjectDraftDto>> GetById(Guid draftId, CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(new GetProjectDraftByIdQuery(draftId), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateProjectDraftCommand command,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(command, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { draftId = result.Value }, result.Value)
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpPut("{draftId:guid}")]
    public async Task<ActionResult> Update(
        Guid draftId,
        [FromBody] UpdateProjectDraftContentCommand command,
        CancellationToken cancellationToken)
    {
        if (draftId != command.DraftId)
        {
            return BadRequest("Route ID and command ID must match.");
        }

        var result = await dispatcher.SendAsync(command, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpPut("{draftId:guid}/co-author")]
    public async Task<ActionResult> InviteCoAuthor(
        Guid draftId,
        [FromBody] InviteDraftCoAuthorCommand command,
        CancellationToken cancellationToken)
    {
        if (draftId != command.DraftId)
        {
            return BadRequest("Route ID and command ID must match.");
        }

        var result = await dispatcher.SendAsync(command, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpDelete("{draftId:guid}/co-author")]
    public async Task<ActionResult> RemoveCoAuthor(Guid draftId, [FromQuery] int expectedVersion, CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(new RemoveDraftCoAuthorCommand(draftId, expectedVersion), cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : ApiResultHttpMapper.FromError(result.Error);
    }

    [HttpDelete("{draftId:guid}")]
    public async Task<ActionResult> Delete(
        Guid draftId,
        [FromQuery] int expectedVersion,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(new DeleteProjectDraftCommand(draftId, expectedVersion), cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : ApiResultHttpMapper.FromError(result.Error);
    }
}
