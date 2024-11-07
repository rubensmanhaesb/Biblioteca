using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Interfaces.Repositories
{
    public interface IBaseQueryRepository<T> where T : class
    {
        Task<IEnumerable<T?>> GetByConditionAsync(
            int? pageSize = null,
            int? pageNumber = null,
            Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, object>>[] orderBy = null,
            bool isAscending = true,
            Expression<Func<T, object>>[] includes = null,
            CancellationToken cancellationToken = default);
    }
}
