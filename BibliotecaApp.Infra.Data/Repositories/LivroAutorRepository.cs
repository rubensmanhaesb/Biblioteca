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

    }
}
