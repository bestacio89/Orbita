using Orbita.Contracts.DTOs;
using Orbita.Common.Mediator.Messages;
using Orbita.Common.Mediator.Results;

namespace Orbita.Contracts.Queries.Books
{
    // Ensure the generic parameter matches the handler's expected response type
    public sealed record GetBooksByAuthorQuery(string Author) : IQuery<Result<IEnumerable<BookDto>>>;
}