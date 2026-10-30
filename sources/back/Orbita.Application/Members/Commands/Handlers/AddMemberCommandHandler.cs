// Application/Members/Commands/AddMemberCommandHandler.cs

using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;

using Orbita.Common.Mapping.Abstractions;
using Orbita.Common.Mediator.Handlers;
using Orbita.Common.Mediator.Results;
using Orbita.Common.EntityFramework.Repositories;
using Orbita.Persistence;
using Orbita.Contracts.Commands.Members;

namespace Orbita.Application.Members.Commands.Handlers;

public sealed class AddMemberCommandHandler
    : ICommandHandler<AddMemberCommand, Result<int>>
{
    private readonly EntityRepository<ApplicationDbContext,Member, int> _memberRepository;
    private readonly IOrbitaMapper _mapper;

    public AddMemberCommandHandler(EntityRepository<ApplicationDbContext, Member, int> memberRepository, IOrbitaMapper mapper)
    {
        _memberRepository = memberRepository;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        var member = new Member( new FullName (request.FullName), new Email(request.Email));

        await _memberRepository.AddAsync(member, cancellationToken);
        return member.Id.ToResult();
    }
}
