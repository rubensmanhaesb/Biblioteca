using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Interfaces.Services
{
    public interface IBaseDomainService<TEntity, TKey> : IDisposable
        where TEntity : class
    {
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);

        Task<List<TEntity>>? GetAllAsync();
        Task<TEntity>? GetByIdAsync(TKey id);
    }
}
