using FluentValidation;

namespace Application.Features.Books.Commands.UpdateBook;

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ISBN).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.PublisherId).GreaterThan(0);
        RuleFor(x => x.AuthorIds).NotEmpty().WithMessage("A book must have at least one author.");
    }
}