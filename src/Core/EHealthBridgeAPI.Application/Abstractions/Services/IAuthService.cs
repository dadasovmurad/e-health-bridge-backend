using EHealthBridgeAPI.Application.Abstractions.Services.Authentications;
using EHealthBridgeAPI.Application.DTOs;
using Core.Results;

namespace EHealthBridgeAPI.Application.Abstractions.Services
{
    public interface IAuthService : IInternalAuthentication
    {
        Task<IResult> GeneratePasswordResetTokenAsync(string email);
        Task<IResult> ResetPasswordAsync(string token, string newPassword);
        Task<IDataResult<TokenDto>> RefreshTokenAsync(string refreshToken);
    }
}
