// Application/Members/Queries/GetMemberByIdQuery.cs
using Orbita.Common.Mediator.Messages;
using Orbita.Common.Mediator.Results;
using Orbita.Contracts.DTOs;

namespace Orbita.Contracts.Queries.Members;

public sealed record GetMemberByIdQuery(int MemberId)
    : IQuery<Result<MemberDto>>;
