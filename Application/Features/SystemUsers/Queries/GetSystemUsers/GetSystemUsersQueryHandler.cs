using Application.Common.Dtos;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SystemUsers.Queries.GetSystemUsers;

public class GetSystemUsersQueryHandler : IRequestHandler<GetSystemUsersQuery, List<SystemUserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    public GetSystemUsersQueryHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task<List<SystemUserDto>> Handle(GetSystemUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.OrderBy(u => u.FullName).ToListAsync(cancellationToken);

        var result = new List<SystemUserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new SystemUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                IsActive = user.IsActive,
                Roles = roles.ToList()
            });
        }

        return result;
    }
}
