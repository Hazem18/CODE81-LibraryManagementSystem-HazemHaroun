using Application.Common.Dtos;
using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.SystemUsers.Commands.CreateSystemUser;

public class CreateSystemUserCommandHandler : IRequestHandler<CreateSystemUserCommand, SystemUserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    public CreateSystemUserCommandHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task<SystemUserDto> Handle(CreateSystemUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            throw new BusinessRuleException("A system user with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new BusinessRuleException(string.Join(" ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, request.Role);

        return new SystemUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            IsActive = user.IsActive,
            Roles = new List<string> { request.Role }
        };
    }
}
