using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHealthBridgeAPI.Domain.Entities.Common;

namespace EHealthBridgeAPI.Application.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<int> InsertAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}
