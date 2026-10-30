// Application/Members/Commands/DeleteMemberCommandHandler.cs
using Franz.Common.EntityFramework.Repositories;
using Franz.Common.Mediator.Errors;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using Orbita.Contracts.Commands.Members;
using Orbita.Domain.Entities;
using Orbita.Persistence;

namespace Orbita.Application.Members.Commands;

public sealed class DeleteMemberCommandHandler
    : ICommandHandler<DeleteMemberCommand, Result>
{
    private readonly EntityRepository<ApplicationDbContext, Member, int> _memberRepository;

    public DeleteMemberCommandHandler(EntityRepository<ApplicationDbContext, Member, int> memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<Result> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        if (member is null)
            return Error.NotFound("Member", request.MemberId).ToFailure<Result>();

        await _memberRepository.DeleteAsync(member);
        
        return Result.Success();
    }
}
