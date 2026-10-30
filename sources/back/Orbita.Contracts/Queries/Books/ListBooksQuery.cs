// Application/Books/Queries/ListBooksQuery.cs
using Orbita.Contracts.DTOs;
using Orbita.Common.Mediator.Messages;
using Orbita.Common.Mediator.Results;

namespace Orbita.Contracts.Queries.Books;

public sealed record ListBooksQuery
    : IQuery<Result<IReadOnlyCollection<BookDto>>>;
