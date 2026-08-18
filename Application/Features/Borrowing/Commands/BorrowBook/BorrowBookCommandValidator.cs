using FluentValidation;

namespace Application.Features.Borrowing.Commands.BorrowBook;

public class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
{
    public BorrowBookCommandValidator()
    {
        RuleFor(x => x.BookId).GreaterThan(0);
        RuleFor(x => x.MemberId).GreaterThan(0);
    }
}
