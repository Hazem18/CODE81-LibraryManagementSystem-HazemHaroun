using Application.Common.Dtos;
using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.SystemUsers.Queries.GetSystemUserById;

public class GetSystemUserByIdQueryHandler : IRequestHandler<GetSystemUserByIdQuery, SystemUserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    public GetSystemUserByIdQueryHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task<SystemUserDto> Handle(GetSystemUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user is null) throw new NotFoundException(nameof(ApplicationUser), request.Id);

        var roles = await _userManager.GetRolesAsync(user);

        return new SystemUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            IsActive = user.IsActive,
            Roles = roles.ToList()
        };
    }
}
