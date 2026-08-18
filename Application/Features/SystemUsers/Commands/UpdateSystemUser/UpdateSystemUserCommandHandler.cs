using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.SystemUsers.Commands.UpdateSystemUser;

public class UpdateSystemUserCommandHandler : IRequestHandler<UpdateSystemUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    public UpdateSystemUserCommandHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task Handle(UpdateSystemUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user is null) throw new NotFoundException(nameof(ApplicationUser), request.Id);

        user.FullName = request.FullName;
        user.IsActive = request.IsActive;

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Count != 0)
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, request.Role);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new BusinessRuleException(string.Join(" ", result.Errors.Select(e => e.Description)));
    }
}
