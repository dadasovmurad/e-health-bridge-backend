using Core.Results;
using EHealthBridgeAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Application.Abstractions.Services
{
    public interface IUserManager
    {
        Task<IDataResult<int>> RegisterUserAsync(AppUser user, string rawPassword);
        Task<IDataResult<AppUser>?> AuthenticateUserAsync(string username, string rawPassword);
    }
}
