using System.Security.Claims;
using Domain.Entities.Enums;

namespace WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(value!);
    }

    public static Role GetRole(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.Role);
        return Enum.Parse<Role>(value!);
    }

    public static int GetDoctorId(this ClaimsPrincipal user)
    {
        if (user.GetRole() != Role.Doctor)
            throw new UnauthorizedAccessException("El usuario no es Doctor.");

        return user.GetUserId();
    }
}