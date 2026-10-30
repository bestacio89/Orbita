// Application/Members/Commands/AddMemberCommand.cs

using Orbita.Common.Mediator.Messages;
using Orbita.Common.Mediator.Results;

namespace Orbita.Contracts.Commands.Members;

public sealed record AddMemberCommand(string FullName, string Email)
    : ICommand<Result<int>>;
