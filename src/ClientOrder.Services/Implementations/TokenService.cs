using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ClientOrder.Services.Dtos;
using ClientOrder.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ClientOrder.Services.Implementations;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public TokenDto GenerateToken(UserDto userDto)
    {
        var keySettings = configuration["JwtSettings:Key"];
        var expirationSettings = configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");

        if (string.IsNullOrWhiteSpace(keySettings) || expirationSettings <= 0)
        {
            throw new ArgumentException("Complete the JwtSettings configuration.");
        }

        var keyBytes = Convert.FromBase64String(keySettings);
        var key = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expirationSettings);

        var claims = new List<Claim>
        {
            new("sub", userDto.Id.ToString()),
            new("name", userDto.FullName),
            new("username", userDto.Username),
            new("role", userDto.Role)
        };

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new TokenDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expires
        };
    }
}
