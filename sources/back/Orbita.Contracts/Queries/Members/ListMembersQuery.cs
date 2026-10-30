// Application/Members/Queries/ListMembersQuery.cs
using Orbita.Common.Mediator.Messages;
using Orbita.Common.Mediator.Results;
using Orbita.Contracts.DTOs;

namespace Orbita.Contracts.Queries.Members;

public sealed record ListMembersQuery
    : IQuery<Result<IReadOnlyCollection<MemberDto>>>;
