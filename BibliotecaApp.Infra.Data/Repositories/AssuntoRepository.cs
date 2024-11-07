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
    public class AssuntoRepository : BaseRepository<Assunto, int>, IAssuntoRepository
    {
        private readonly DataContext? _dataContext;

        public AssuntoRepository(DataContext? dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }
    }
}
