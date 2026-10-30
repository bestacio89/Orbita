#nullable enable

using Franz.Common.Mediator.Errors;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using Orbita.Application.Identity;
using Orbita.Contracts.Authorization;
using Orbita.Contracts.DTOs.Projects;
using Orbita.Contracts.Persistence;
using Orbita.Contracts.Queries.Projects;
using Orbita.Domain.Entities;

namespace Orbita.Application.Projects.Queries;

public sealed class GetCurrentUserQueryHandler(
    CurrentActorResolver actorResolver,
    IUserRepository users,
    ICurrentUserContext currentUser,
    TimeProvider timeProvider)
    : IQueryHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
{
    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (user is null)
        {
            return new Error("Unauthorized", "A validated Entra identity is required.").ToFailure<CurrentUserDto>();
        }

        user.RecordLogin(timeProvider.GetUtcNow());
        await users.UpdateAsync(user, cancellationToken);

        return new CurrentUserDto(
            CurrentUserContract.Version,
            user.Id,
            user.TenantId,
            user.ObjectId,
            user.DisplayName,
            user.Email ?? string.Empty,
            currentUser.GlobalRoles,
            GlobalRoleCapabilityMatrix.Resolve(currentUser.GlobalRoles)).ToResult();
    }
}
