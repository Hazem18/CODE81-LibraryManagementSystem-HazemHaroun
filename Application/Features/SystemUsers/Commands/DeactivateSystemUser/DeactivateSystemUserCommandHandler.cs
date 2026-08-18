using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.SystemUsers.Commands.DeactivateSystemUser;

// System users are never hard-deleted: BorrowingTransaction.IssuedByUserId
// references them with a Restrict delete (see BorrowingTransactionConfiguration),
// so removing a user who has ever processed a transaction would violate that FK
// anyway. Deactivating preserves audit history while revoking access - this is
// what DELETE /api/systemusers/{id} actually does.
public class DeactivateSystemUserCommandHandler : IRequestHandler<DeactivateSystemUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    public DeactivateSystemUserCommandHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task Handle(DeactivateSystemUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user is null) throw new NotFoundException(nameof(ApplicationUser), request.Id);

        user.IsActive = false;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BusinessRuleException(string.Join(" ", result.Errors.Select(e => e.Description)));
    }
}
