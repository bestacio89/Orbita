// Application/Members/Commands/DeleteMemberCommand.cs
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace Orbita.Contracts.Commands.Members;

public sealed record DeleteMemberCommand(int MemberId)
    : ICommand<Result>;
