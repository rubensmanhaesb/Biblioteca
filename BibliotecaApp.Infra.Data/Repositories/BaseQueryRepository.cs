using BibliotecaApp.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Infra.Data.Repositories
{
    internal class BaseQueryRepository<T> : IBaseQueryRepository<T>, IDisposable where T : class
    {
        private readonly DbContext _dbContext;

        public BaseQueryRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<T?>> GetByConditionAsync(
            int? pageSize = null,
            int? pageNumber = null,
            Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, object>>[] orderBy = null,
            bool isAscending = true,
            Expression<Func<T, object>>[] includes = null,
            CancellationToken cancellationToken = default)
        {

            IQueryable<T> query = _dbContext.Set<T>();

            var totalRecords = await query.CountAsync();

            query = ApplyPredicate(query, predicate);
            query = ApplyPagination(query: query, pageNumber: pageNumber, pageSize: pageSize);
            query = ApplyIncludes(query, includes);
            query = ApplyOrdering(query, orderBy, isAscending);

            var lista = await query.ToListAsync(cancellationToken);

            return lista;

        }

        #region rotinas usada em GetByConditionAsync
        private IQueryable<T> ApplyPagination(IQueryable<T> query, int? pageNumber = null, int? pageSize = null)
        {
            if ((pageNumber != null) && (pageSize != null))
                if (pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((int)((pageNumber - 1) * pageSize)).Take((int)pageSize);
                }
            return query;
        }

        private IQueryable<T> ApplyPredicate(IQueryable<T> query, Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                return query.Where(predicate);
            }
            return query;
        }
        private IQueryable<T> ApplyIncludes(IQueryable<T> query, Expression<Func<T, object>>[] includes)
        {
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);//EAGER LOADING
                }
            }
            return query;
        }
        private IQueryable<T> ApplyOrdering(IQueryable<T> query, Expression<Func<T, object>>[] orderBy, bool isAscending)
        {
            if (orderBy != null && orderBy.Any())
            {
                if (isAscending)
                {
                    query = query.OrderBy(orderBy.First());

                    foreach (var order in orderBy.Skip(1))
                    {
                        query = ((IOrderedQueryable<T>)query).ThenBy(order);
                    }
                }
                else
                {
                    query = query.OrderByDescending(orderBy.First());

                    foreach (var order in orderBy.Skip(1))
                    {
                        query = ((IOrderedQueryable<T>)query).ThenByDescending(order);
                    }
                }
            }
            return query;
        }
        #endregion

        public void Dispose()
        {
            _dbContext?.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
