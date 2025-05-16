using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Results;
using EHealthBridgeAPI.Application.DTOs;

namespace EHealthBridgeAPI.Application.Abstractions.Services.Authentications
{
    public interface IInternalAuthentication
    {
        Task<IDataResult<LoginDto>> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime);
    }
}