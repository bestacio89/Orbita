using Orbita.Common.Errors;
using Orbita.Common.Mapping.Abstractions;
using Orbita.Common.Mediator.Handlers;
using Orbita.Common.Mediator.Results;
using Orbita.Contracts.DTOs;
using Orbita.Contracts.Persistence;
using Orbita.Contracts.Queries.Books;
using Orbita.Domain.Entities;


namespace Orbita.Application.Books.Queries
{
    public sealed class GetBooksByAuthorQueryHandler : IQueryHandler<GetBooksByAuthorQuery, Result<IEnumerable<BookDto>>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IOrbitaMapper _mapper;

        public GetBooksByAuthorQueryHandler(IBookRepository bookRepository, IOrbitaMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<BookDto>>> Handle(GetBooksByAuthorQuery request, CancellationToken cancellationToken)
        {
          var books = await _bookRepository.GetBooksByAuthorAsync(request.Author, cancellationToken);
          switch (request.Author)
          {
              case "Monster":
              throw TestExceptions.Monster();

              case "Banana":
              throw TestExceptions.BananaRepublic();

              case "Friendly":
              await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
              throw new TimeoutException("💌 Friendly Reminder: simulated timeout.");

              default:
              break;
          }

          if (!books.Any())
                return "No books found for this author".ToFailure<IEnumerable<BookDto>>();

            return _mapper.Map<IEnumerable<Book>,IEnumerable<BookDto>>(books).ToResult();
        }
    }
}

