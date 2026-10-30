using Franz.Common.Mediator.Dispatchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbita.API.Extensions;
using Orbita.Contracts.DTOs.Projects;
using Orbita.Contracts.Queries.Projects;

namespace Orbita.API.Controllers;

[ApiController]
[Authorize]
[Route("api/me")]
public sealed class CurrentUserController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CurrentUserDto>> Get(CancellationToken cancellationToken)
    {
        var result = await dispatcher.SendAsync(new GetCurrentUserQuery(), cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResultHttpMapper.FromError(result.Error);
    }
}
