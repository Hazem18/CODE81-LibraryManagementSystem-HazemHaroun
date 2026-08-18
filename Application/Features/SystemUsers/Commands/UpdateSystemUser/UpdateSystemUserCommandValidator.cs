using Domain.Constants;
using FluentValidation;

namespace Application.Features.SystemUsers.Commands.UpdateSystemUser;

public class UpdateSystemUserCommandValidator : AbstractValidator<UpdateSystemUserCommand>
{
    public UpdateSystemUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Role)
            .Must(r => r == Roles.Administrator || r == Roles.Librarian || r == Roles.Staff)
            .WithMessage($"Role must be one of: {Roles.Administrator}, {Roles.Librarian}, {Roles.Staff}.");
    }
}
