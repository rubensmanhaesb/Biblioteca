using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity, TKey> : IDisposable
        where TEntity : class
    {
        Task Add(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);

        Task<List<TEntity>>? GetAll();
        Task<TEntity>? GetById(TKey id);
    }
}
