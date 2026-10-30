// Application/Members/Commands/UpdateMemberCommand.cs
using Orbita.Common.Mediator.Messages;
using Orbita.Common.Mediator.Results;

namespace Orbita.Contracts.Commands.Members;

public sealed record UpdateMemberCommand(int MemberId, string FullName, string Email)
    : ICommand<Result>;
