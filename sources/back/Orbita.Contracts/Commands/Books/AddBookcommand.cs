// Commands/Books/AddBookCommand.cs
using Orbita.Common.Mediator;
using Orbita.Common.Mediator.Messages;
namespace Orbita.Contracts.Commands.Books;
public sealed record AddBookCommand(string Title, string Author, string Isbn, DateTime Datepublished, int Copies)
    : ICommand<int>;
