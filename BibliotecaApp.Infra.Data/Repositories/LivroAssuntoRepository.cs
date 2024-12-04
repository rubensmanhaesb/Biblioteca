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
    public class LivroAssuntoRepository : BaseRepository<LivroAssunto, LivroAssuntoPk>, ILivroAssuntoRepository
    {
        private readonly DataContext? _dataContext;

        public LivroAssuntoRepository(DataContext? dataContext) : base(dataContext) 
        {
            _dataContext = dataContext;
        }

        public async Task<IEnumerable<LivroAssunto?>> GetByConditionAsync(int? pageSize = null, int? pageNumber = null, Expression<Func<LivroAssunto, bool>> predicate = null, Expression<Func<LivroAssunto, object>>[] orderBy = null, bool isAscending = true, Expression<Func<LivroAssunto, object>>[] includes = null, CancellationToken cancellationToken = default)
        {
            var query = new BaseQueryRepository<LivroAssunto>(_dataContext);

            return await query.GetByConditionAsync(pageSize: pageSize, pageNumber: pageNumber,
                 predicate: predicate, orderBy: orderBy, isAscending: isAscending, includes: includes, cancellationToken).ConfigureAwait(false);
        }

        public override async Task<LivroAssunto?> GetById(LivroAssuntoPk id)
        {
            return await _dataContext!.Set<LivroAssunto>()
                .FirstOrDefaultAsync(la => la.LivroCodl == id.LivroCodl && la.AssuntoCodAs == id.AssuntoCodAs);

        }
    }
}