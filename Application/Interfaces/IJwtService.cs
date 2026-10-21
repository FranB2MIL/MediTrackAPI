using Domain.Entities;
using Domain.Interfaces;

namespace Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(IAuthenticatable user);
}