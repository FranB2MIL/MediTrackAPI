using System.Security.Claims;
using Domain.Entities.Enums;

namespace WebApi.Extensions;

// Con TPT la Id es la misma en todos los roles: lo que distingue al médico, paciente o admin es el claim Role.
public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (value == null)
            throw new UnauthorizedAccessException($"El token no contiene el claim '{ClaimTypes.NameIdentifier}'.");

        return int.Parse(value);
    }

    public static int GetDoctorId(this ClaimsPrincipal user) => user.GetUserIdInRole(Role.Doctor);

    public static int GetPatientId(this ClaimsPrincipal user) => user.GetUserIdInRole(Role.Patient);

    public static int GetAdminId(this ClaimsPrincipal user) => user.GetUserIdInRole(Role.Admin);

    private static int GetUserIdInRole(this ClaimsPrincipal user, Role role)
    {
        if (!user.IsInRole(role.ToString()))
            throw new UnauthorizedAccessException($"El token no tiene el rol '{role}'.");

        return user.GetUserId();
    }
}
