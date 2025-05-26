using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Domain.Entities;

namespace EHealthBridgeAPI.Application.Abstractions.Token
{
    public interface ITokenHandler
    {
        TokenDto CreateAccessToken(int second, AppUser appUser);
        string CreateRefreshToken();
    }
}
