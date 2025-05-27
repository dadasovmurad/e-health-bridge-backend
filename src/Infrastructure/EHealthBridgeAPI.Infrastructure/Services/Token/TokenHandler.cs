using EHealthBridgeAPI.Application.Abstractions.Token;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Domain.Entities;
using Microsoft.Extensions.Configuration;
using EHealthBridgeAPI.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;

namespace EHealthBridgeAPI.Infrastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IRoleRepository _roleRepository;

        public TokenHandler(IConfiguration configuration, IRoleRepository roleRepository)
        {
            _configuration = configuration;
            _roleRepository = roleRepository;
        }

        public async Task<Application.DTOs.TokenDto> CreateAccessTokenAsync(int second, AppUser user)
        {
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            // Get user roles
            var userRoles = await _roleRepository.GetUserRolesAsync(user.Id);
            
            // Create claims
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email)
            };

            // Add role claims
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var expiration = DateTime.UtcNow.AddSeconds(second);
            JwtSecurityToken securityToken = new(
                audience: _configuration["Token:Audience"],
                issuer: _configuration["Token:Issuer"],
                expires: expiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials,
                claims: claims
            );

            JwtSecurityTokenHandler tokenHandler = new();
            var accessToken = tokenHandler.WriteToken(securityToken);
            string refreshToken = CreateRefreshToken();

            return new(accessToken, expiration, refreshToken);
        }

        public string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using RandomNumberGenerator random = RandomNumberGenerator.Create();
            random.GetBytes(number);
            return Convert.ToBase64String(number);
        }
    }
}