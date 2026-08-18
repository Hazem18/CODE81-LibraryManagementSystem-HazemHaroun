using Domain.Constants;
using FluentValidation;

namespace Application.Features.SystemUsers.Commands.CreateSystemUser;

public class CreateSystemUserCommandValidator : AbstractValidator<CreateSystemUserCommand>
{
    public CreateSystemUserCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Role)
            .Must(r => r == Roles.Administrator || r == Roles.Librarian || r == Roles.Staff)
            .WithMessage($"Role must be one of: {Roles.Administrator}, {Roles.Librarian}, {Roles.Staff}.");
    }
}
