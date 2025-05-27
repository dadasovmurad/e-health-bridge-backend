using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Domain.Entities;

namespace EHealthBridgeAPI.Application.Abstractions.Token
{
    public interface ITokenHandler
    {
        Task<TokenDto> CreateAccessTokenAsync(int second, AppUser appUser);
        string CreateRefreshToken();
    }
}
