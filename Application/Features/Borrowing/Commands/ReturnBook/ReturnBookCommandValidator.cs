using FluentValidation;

namespace Application.Features.Borrowing.Commands.ReturnBook;

public class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
{
    public ReturnBookCommandValidator()
    {
        RuleFor(x => x.TransactionId).GreaterThan(0);
    }
}
