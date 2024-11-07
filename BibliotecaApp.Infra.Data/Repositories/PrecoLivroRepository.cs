using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BibliotecaApp.Infra.Data.Repositories
{
    public class PrecoLivroRepository : BaseRepository<PrecoLivro, int>, IPrecoLivroRepository 
    {
        private readonly DataContext? _dataContext;

        public PrecoLivroRepository(DataContext? dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IEnumerable<PrecoLivro?>> GetByConditionAsync(int? pageSize = null, int? pageNumber = null, Expression<Func<PrecoLivro, bool>> predicate = null, Expression<Func<PrecoLivro, object>>[] orderBy = null, bool isAscending = true, Expression<Func<PrecoLivro, object>>[] includes = null, CancellationToken cancellationToken = default)
        {
            var query = new BaseQueryRepository<PrecoLivro>(_dataContext);
            
            return await query.GetByConditionAsync(pageSize: pageSize, pageNumber: pageNumber,
            predicate: predicate, orderBy: orderBy, isAscending: isAscending, includes: includes, cancellationToken).ConfigureAwait(false);

        }
    }
}
