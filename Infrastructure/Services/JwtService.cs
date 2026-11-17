using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Application.Settings;
using Application.Common;
using Domain.Entities;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        foreach (var role in user.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));

        if (user.Doctor != null)
            claims.Add(new Claim(CustomClaims.DoctorId, user.Doctor.Id.ToString()));
        if (user.Patient != null)
            claims.Add(new Claim(CustomClaims.PatientId, user.Patient.Id.ToString()));
        if (user.Admin != null)
            claims.Add(new Claim(CustomClaims.AdminId, user.Admin.Id.ToString()));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}