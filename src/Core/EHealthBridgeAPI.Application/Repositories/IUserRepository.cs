using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHealthBridgeAPI.Domain.Entities;

namespace EHealthBridgeAPI.Application.Repositories
{
    public interface IUserRepository : IGenericRepository<AppUser>
    {
        Task<AppUser?> GetByEmailOrUsernameAsync(string emailOrUsername);
    }
}
