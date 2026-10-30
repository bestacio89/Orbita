// Application/Members/Commands/DeleteMemberCommand.cs
using Orbita.Common.Mediator.Messages;
using Orbita.Common.Mediator.Results;

namespace Orbita.Contracts.Commands.Members;

public sealed record DeleteMemberCommand(int MemberId)
    : ICommand<Result>;
