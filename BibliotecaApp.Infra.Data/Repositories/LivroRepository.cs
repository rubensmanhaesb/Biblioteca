using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Infra.Data.Repositories
{
    public class LivroRepository : BaseRepository<Livro, int>, ILivroRepository
    {
        private readonly DataContext? _dataContext;

        public LivroRepository(DataContext? dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }
    }
}
