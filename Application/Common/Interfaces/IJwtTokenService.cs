using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}