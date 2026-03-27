using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastTransfers.Application.Common;
using FastTransfers.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FastTransfers.Infrastructure.Identity;

public class JwtService : IJwtService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryMinutes;

    public JwtService(IConfiguration config)
    {
        _secret        = config["Jwt:Secret"]   ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
        _issuer        = config["Jwt:Issuer"]   ?? "FastTransfers";
        _audience      = config["Jwt:Audience"] ?? "FastTransfers";
        _expiryMinutes = int.TryParse(config["Jwt:ExpiryMinutes"], out var exp) ? exp : 60;
    }

    public string GenerateToken(User user)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name,  user.Name),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer:             _issuer,
            audience:           _audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
