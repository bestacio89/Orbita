using Orbita.Common.Business.Domain;
using Orbita.Common.Mapping.Abstractions;
using Orbita.Common.Mediator.Handlers;
using Orbita.Common.Mediator.Results;
using Orbita.Contracts.DTOs;
using Orbita.Contracts.Persistence;
using Orbita.Contracts.Queries.Books;
using Orbita.Domain.Entities;

namespace Orbita.Application.Books.Queries;

public sealed class ListBooksQueryHandler
    : IQueryHandler<ListBooksQuery, Result<IReadOnlyCollection<BookDto>>>
{
  private readonly IBookRepository _bookRepository;
  private readonly IOrbitaMapper _mapper;

    public ListBooksQueryHandler(IBookRepository bookRepository, IOrbitaMapper mapper)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<BookDto>>> Handle(ListBooksQuery request, CancellationToken cancellationToken)
    {
        var books =  await _bookRepository.GetAllAsync(cancellationToken);
        var mapped = _mapper.Map<IReadOnlyCollection<Book>, IReadOnlyCollection<BookDto>>(books);

        return mapped.ToResult();
    }
}
