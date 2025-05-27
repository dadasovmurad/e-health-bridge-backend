using EHealthBridgeAPI.Application.DTOs.Auth;
using EHealthBridgeAPI.Application.DTOs;
using Core.Results;

namespace EHealthBridgeAPI.Application.Abstractions.Services.Authentications
{
    public interface IInternalAuthentication
    {
        Task<IDataResult<LoginDto>> LoginAsync(InternalLoginRequestDto internalLoginRequestDto);
    }
}