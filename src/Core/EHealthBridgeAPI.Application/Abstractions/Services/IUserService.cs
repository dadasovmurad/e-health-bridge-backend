using Core.Results;
using EHealthBridgeAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<IDataResult<IEnumerable<AppUser>>> GetAllAsync();
        Task<IDataResult<AppUser?>> GetByIdAsync(int id);
        Task<IDataResult<int>> CreateAsync(AppUser user);
        Task<Result> UpdateAsync(AppUser user);
        Task<Result> RemoveByIdAsync(int id);
    }
}
