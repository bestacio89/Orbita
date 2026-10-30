// Application/Members/Queries/GetMemberByIdQueryHandler.cs


using Orbita.Common.Business.Domain;
using Orbita.Common.EntityFramework.Repositories;
using Orbita.Common.Mapping.Abstractions;
using Orbita.Common.Mediator.Errors;
using Orbita.Common.Mediator.Handlers;
using Orbita.Common.Mediator.Results;
using Orbita.Contracts.DTOs;
using Orbita.Contracts.Queries.Members;
using Orbita.Domain.Entities;
using Orbita.Persistence;

namespace Orbita.Application.Members.Queries;

public sealed class GetMemberByIdQueryHandler
    : IQueryHandler<GetMemberByIdQuery, Result<MemberDto>>
{
    private readonly EntityRepository<ApplicationDbContext, Member, int> _memberRepository;
    private readonly IOrbitaMapper _mapper;

    public GetMemberByIdQueryHandler(EntityRepository<ApplicationDbContext, Member, int>  memberRepository, IOrbitaMapper mapper)
    {
        _memberRepository = memberRepository;
        _mapper = mapper;
    }

    public async Task<Result<MemberDto>> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId);
        if (member is null)
            return Error.NotFound("Member", request.MemberId).ToFailure<MemberDto>();

        return _mapper.Map<Member, MemberDto>(member).ToResult();
    }
}
