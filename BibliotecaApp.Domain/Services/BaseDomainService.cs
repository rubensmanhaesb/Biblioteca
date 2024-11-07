using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Services
{
    public abstract class BaseDomainService<TEntity, TKey> : IBaseDomainService<TEntity, TKey>
        where TEntity : class
    {
        private readonly IBaseRepository<TEntity, TKey> _baseRepository;

        protected BaseDomainService(IBaseRepository<TEntity, TKey> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async virtual Task AddAsync(TEntity entity)
        {
            await _baseRepository.Add(entity);
        }

        public async virtual Task UpdateAsync(TEntity entity)
        {
            await _baseRepository.Update(entity);
        }

        public async virtual Task DeleteAsync(TEntity entity)
        {
            await _baseRepository.Delete(entity);
        }

        public async virtual Task<List<TEntity>>? GetAllAsync()
        {
            return await _baseRepository.GetAll()!;
        }

        public async virtual Task<TEntity>? GetByIdAsync(TKey id)
        {
            return await _baseRepository.GetById(id)!;
        }

        public void Dispose()
        {
            _baseRepository.Dispose();
        }


    }
}
