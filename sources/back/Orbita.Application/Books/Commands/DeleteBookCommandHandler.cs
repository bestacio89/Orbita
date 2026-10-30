using Orbita.Common.EntityFramework.Repositories;
using Orbita.Common.Mediator.Errors;
using Orbita.Common.Mediator.Handlers;
using Orbita.Common.Mediator.Results;
using Orbita.Contracts.Commands.Books;
using Orbita.Domain.Entities;
using Orbita.Persistence;


namespace Orbita.Application.Books.Commands
{
    public sealed class DeleteBookCommandHandler : ICommandHandler<DeleteBookCommand, Result>
    {
        private readonly EntityRepository<ApplicationDbContext, Book, int> _bookRepository;

        public DeleteBookCommandHandler(EntityRepository<ApplicationDbContext, Book, int> bookRepository) =>
            _bookRepository = bookRepository;

        public async Task<Result> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);
            if (book is null)
                return Result.Failure(Error.NotFound("Book", request.Id));

            await _bookRepository.DeleteAsync(book,false);
          

            return Result.Success();
        }
    }

}
