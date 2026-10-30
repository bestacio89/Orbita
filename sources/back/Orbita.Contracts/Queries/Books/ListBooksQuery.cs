// Application/Books/Queries/ListBooksQuery.cs
using Orbita.Contracts.DTOs;
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace Orbita.Contracts.Queries.Books;

public sealed record ListBooksQuery
    : IQuery<Result<IReadOnlyCollection<BookDto>>>;
