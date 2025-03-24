using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;

namespace ShoppingListApp.Core.Services
{
    //TODO: Move ITokenService to interfaces folder
    public interface ITokenService
    {
        string GenerateToken(int userId, string username, string email);
        int? ValidateTokenAndGetUserId(string token);
        bool IsTokenValid(string token);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(int userId, string username, string email)
        {
            var useJwt = !string.IsNullOrEmpty(_configuration["Jwt:Key"]);

            if (useJwt)
            {
                return GenerateJwtToken(userId, username, email);
            }
            else
            {
                // Generate a simple GUID-based token for backward compatibility
                return Guid.NewGuid().ToString("N");
            }
        }

        private string GenerateJwtToken(int userId, string username, string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim("id", userId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"] ?? "120")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int? ValidateTokenAndGetUserId(string token)
        {
            var useJwt = !string.IsNullOrEmpty(_configuration["Jwt:Key"]);

            if (useJwt)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = _configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                    return userId;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                // With the current simple token system, we can't extract the user ID from the token directly
                return null;
            }
        }

        public bool IsTokenValid(string token)
        {
            var useJwt = !string.IsNullOrEmpty(_configuration["Jwt:Key"]);

            if (useJwt)
            {
                return ValidateTokenAndGetUserId(token) != null;
            }
            else
            {
                // For now, consider any non-empty token as valid
                return !string.IsNullOrEmpty(token);
            }
        }
    }
}