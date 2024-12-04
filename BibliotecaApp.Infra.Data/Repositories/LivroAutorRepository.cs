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

namespace BibliotecaApp.Infra.Data.Repositories
{
    public class LivroAutorRepository : BaseRepository<LivroAutor, LivroAutorPk>, ILivroAutorRepository
    {
        private readonly DataContext? _dataContext;

        public LivroAutorRepository(DataContext? dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }

        public override async Task<LivroAutor?> GetById(LivroAutorPk id)
        {
            return await _dataContext!.Set<LivroAutor>()
                .FirstOrDefaultAsync(la => la.LivroCodl == id.LivroCodl && la.AutorCodAu == id.AutorCodAu);
        }

        public async Task<IEnumerable<LivroAutor?>> GetByConditionAsync(int? pageSize = null, int? pageNumber = null, Expression<Func<LivroAutor, bool>> predicate = null, Expression<Func<LivroAutor, object>>[] orderBy = null, bool isAscending = true, Expression<Func<LivroAutor, object>>[] includes = null, CancellationToken cancellationToken = default)
        {
            var query = new BaseQueryRepository<LivroAutor>(_dataContext);

            return await query.GetByConditionAsync(pageSize: pageSize, pageNumber: pageNumber,
                 predicate: predicate, orderBy: orderBy, isAscending: isAscending, includes: includes, cancellationToken).ConfigureAwait(false);
        }

    }
}
