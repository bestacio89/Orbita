


using Orbita.Domain.Entities;

using Orbita.Common.Mapping.Abstractions;
using Orbita.Common.Mediator.Handlers;
using Orbita.Common.Mediator.Results;
using Orbita.Contracts.DTOs;
using Orbita.Contracts.Persistence;
using Orbita.Contracts.Queries.Books;

namespace Orbita.Application.Books.Queries;
public sealed class GetBookByTitleQueryHandler
    : IQueryHandler<GetBookByTitleQuery, Result<IEnumerable<BookDto>>>
{
    private readonly IBookRepository _bookRepository;
    private readonly IOrbitaMapper _mapper;

    public GetBookByTitleQueryHandler(IBookRepository bookRepository, IOrbitaMapper mapper)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<BookDto>>> Handle(
        GetBookByTitleQuery request,
        CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetBooksByAuthorAsync(request.Title, cancellationToken);

        if (books is null || !books.Any())
            return "No books found with the given title".ToFailure<IEnumerable<BookDto>>();

        return _mapper.Map<IEnumerable<Book>,IEnumerable<BookDto>>(books).ToResult();
    }
}
