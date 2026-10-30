// Queries/Members/GetMemberByNameQuery.cs

using Orbita.Common.Mediator.Messages;
using Orbita.Common.Mediator.Results;
using Orbita.Contracts.DTOs;

namespace Orbita.Contracts.Queries.Members
{
    public sealed record GetMemberByNameQuery(string FullName) : IQuery<Result<MemberDto>>;
}
