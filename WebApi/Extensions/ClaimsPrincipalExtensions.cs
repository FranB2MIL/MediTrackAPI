using System.Security.Claims;
using Domain.Entities.Enums;
using Application.Common;

namespace WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        return user.GetRequiredIntClaim(ClaimTypes.NameIdentifier);
    }


    public static int GetDoctorId(this ClaimsPrincipal user)
    {
        return user.GetRequiredIntClaim(CustomClaims.DoctorId);
    }

    public static int GetPatientId(this ClaimsPrincipal user)
    {
        return user.GetRequiredIntClaim(CustomClaims.PatientId);
    }

    public static int GetAdminId(this ClaimsPrincipal user)
    {
        return user.GetRequiredIntClaim(CustomClaims.AdminId);
    }

    private static int GetRequiredIntClaim(this ClaimsPrincipal user, string claimType)
    {
        var value = user.FindFirstValue(claimType);

        if (value == null)
            throw new UnauthorizedAccessException($"El token no contiene el claim '{claimType}'.");

        return int.Parse(value);
    }
}