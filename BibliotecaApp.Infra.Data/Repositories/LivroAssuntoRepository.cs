using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override async Task<LivroAssunto?> GetById(LivroAssuntoPk id)
        {
            return await _dataContext!.Set<LivroAssunto>()
                .FirstOrDefaultAsync(la => la.LivroCodl == id.LivroCodl && la.AssuntoCodAs == id.AssuntoCodAs);

        }
    }
}