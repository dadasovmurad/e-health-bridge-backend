using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Results;
using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Application.DTOs.Auth;

namespace EHealthBridgeAPI.Application.Abstractions.Services.Authentications
{
    public interface IInternalAuthentication
    {
        Task<IDataResult<LoginDto>> LoginAsync(InternalLoginRequestDto internalLoginRequestDto);
    }
}