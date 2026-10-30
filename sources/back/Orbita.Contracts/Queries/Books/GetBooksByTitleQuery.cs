// Queries/Books/GetBookByTitleQuery.cs
using Orbita.Contracts.DTOs;
using Orbita.Common.Mediator.Messages;
using Orbita.Common.Mediator.Results;

namespace Orbita.Contracts.Queries.Books
{
    public sealed record GetBookByTitleQuery(string Title) : IQuery<Result<IEnumerable<BookDto>>>;
}
