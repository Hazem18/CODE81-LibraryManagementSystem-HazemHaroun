using FluentValidation;

namespace Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.ISBN).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.PublisherId).GreaterThan(0);
        RuleFor(x => x.AuthorIds).NotEmpty().WithMessage("A book must have at least one author.");
        RuleFor(x => x.PublicationYear)
            .LessThanOrEqualTo(DateTime.UtcNow.Year)
            .When(x => x.PublicationYear.HasValue);
    }
}