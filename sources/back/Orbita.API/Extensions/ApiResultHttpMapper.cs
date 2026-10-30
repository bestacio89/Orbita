using Franz.Common.Mediator.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Orbita.API.Extensions;

/// <summary>Maps application result codes to stable HTTP response semantics.</summary>
internal static class ApiResultHttpMapper
{
    public static ActionResult FromError(Error? error) =>
        error?.Code switch
        {
            Error.Codes.NotFound => new NotFoundObjectResult(error.Message),
            Error.Codes.Conflict => new ConflictObjectResult(error.Message),
            Error.Codes.Validation => new BadRequestObjectResult(error.Message),
            "Unauthorized" => new UnauthorizedObjectResult(error.Message),
            "Forbidden" => new ForbidResult(),
            _ => new BadRequestObjectResult(error?.Message ?? "The request could not be processed.")
        };
}
