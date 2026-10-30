using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbita.Contracts.DTOs;

using Orbita.Common.Mediator.Messages;
using Orbita.Common.Mediator.Results;
namespace Orbita.Contracts.Queries.Books
{// Queries/Books/GetBookByIdQuery.cs


    public sealed record GetBookByIdQuery(int BookId) : IQuery<Result<BookDto>>;



}
