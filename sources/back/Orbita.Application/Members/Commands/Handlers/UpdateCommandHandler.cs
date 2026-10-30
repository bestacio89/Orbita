// Application/Members/Commands/UpdateMemberCommandHandler.cs
using Orbita.Common.EntityFramework.Repositories;
using Orbita.Common.Mediator.Errors;
using Orbita.Common.Mediator.Handlers;
using Orbita.Common.Mediator.Results;
using Orbita.Contracts.Commands.Members;
using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Persistence;


namespace Orbita.Application.Members.Commands.Handlers;

public sealed class UpdateMemberCommandHandler
    : ICommandHandler<UpdateMemberCommand, Result>
{
    private readonly EntityRepository<ApplicationDbContext, Member, int> _memberRepository;

    public UpdateMemberCommandHandler(EntityRepository<ApplicationDbContext, Member, int> memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<Result> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        if (member is null)
            return Error.NotFound("Member", request.MemberId).ToFailure<Result<Member>>();

        member.UpdateDetails(new FullName (request.FullName), new Email(request.Email));

        await _memberRepository.UpdateAsync(member, cancellationToken);

        return Result.Success();
    }
}
