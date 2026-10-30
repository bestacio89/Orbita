

// Queries/Books/Handlers/GetBookByIdQueryHandler.cs
using Franz.Common.Mediator;
using Franz.Common.Errors;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using Franz.Common.Mapping.Abstractions;
using Orbita.Domain.Entities;
using Franz.Common.Business.Domain;
using Orbita.Contracts.DTOs;
using Orbita.Contracts.Queries.Books;
using Franz.Common.Business.Repositories;

namespace Orbita.Application.Books.Queries
{
    public sealed class GetBookByIdQueryHandler
    : IQueryHandler<GetBookByIdQuery, Result<BookDto>>
    {
        private readonly IEntityRepository<Book, int> _bookRepository;
    private readonly IOrbitaMapper _mapper;

        public GetBookByIdQueryHandler(IEntityRepository<Book, int> bookRepository, IOrbitaMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<Result<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId);

            if (book is null)
                return "Book not found".ToFailure<BookDto>();

            return _mapper.Map<Book,BookDto>(book).ToResult();
        }
    }

}
