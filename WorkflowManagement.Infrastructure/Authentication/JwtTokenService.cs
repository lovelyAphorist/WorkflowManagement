using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkflowManagement.Application.Users.Dtos;
using WorkflowManagement.Application.Users.Services;

namespace WorkflowManagement.Infrastructure.Authentication
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtOptions _options;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public TokenResult GenerateToken(Guid userId, string email, string displayName)
        {
            var expiresAtUtc =
                DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),

                new(JwtRegisteredClaimNames.Email, email),

                new("displayName", displayName),

                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return new TokenResult
            {
                Token = new JwtSecurityTokenHandler()
                    .WriteToken(token),

                ExpiresAtUtc = expiresAtUtc
            };
        }
    }
}