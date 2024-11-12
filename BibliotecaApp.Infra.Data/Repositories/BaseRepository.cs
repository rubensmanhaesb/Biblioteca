using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Infra.Data.Repositories
{
    public abstract class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
        where TEntity : class
    {
        private readonly DataContext _dataContext;

        protected BaseRepository(DataContext dataContext) => _dataContext = dataContext;

        public async virtual Task Add(TEntity entity)
        {
            await _dataContext.AddAsync(entity);
            await _dataContext.SaveChangesAsync();
        }

        public async virtual Task Update(TEntity entity)
        {
            _dataContext.Update(entity);
            await _dataContext.SaveChangesAsync();
        }

        public async virtual Task Delete(TEntity entity)
        {
            _dataContext.Remove(entity);
            await _dataContext.SaveChangesAsync();
        }

        public async virtual Task<List<TEntity>> GetAll()
        {
            return await _dataContext.ExecuteWithPoliciesAsync(async ctx =>
            {
                return await _dataContext.Set<TEntity>().ToListAsync();
            });
        }

        public async virtual Task<TEntity>? GetById(TKey id)
            => await _dataContext.Set<TEntity>().FindAsync(id);

        public void Dispose() => _dataContext.Dispose();
    }
}
