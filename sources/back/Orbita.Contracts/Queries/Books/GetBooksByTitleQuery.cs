// Queries/Books/GetBookByTitleQuery.cs
using Orbita.Contracts.DTOs;
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace Orbita.Contracts.Queries.Books
{
    public sealed record GetBookByTitleQuery(string Title) : IQuery<Result<IEnumerable<BookDto>>>;
}
