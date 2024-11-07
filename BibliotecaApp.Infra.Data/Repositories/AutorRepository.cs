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
    public class AutorRepository : BaseRepository<Autor, int>, IAutorRepository
    {
        private readonly DataContext? _dataContext;

        public AutorRepository(DataContext? dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }
    }
}
