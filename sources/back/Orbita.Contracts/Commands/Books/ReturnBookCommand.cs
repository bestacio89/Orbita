using Orbita.Common.Mediator.Messages;
using Orbita.Common.Mediator.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbita.Contracts.Commands.Books
{
    public sealed record ReturnBookCommand(int MemberId, int BookId) : ICommand<Result>;

 
}
