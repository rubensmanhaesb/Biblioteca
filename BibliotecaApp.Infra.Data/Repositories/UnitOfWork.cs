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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext? _dataContext;

        public UnitOfWork(DataContext? dataContext)
            => _dataContext = dataContext;

        public IAssuntoRepository? AssuntoRepository => new AssuntoRepository(_dataContext);

        public IAutorRepository? AutorRepository => new AutorRepository(_dataContext);

        public ILivroAssuntoRepository? LivroAssuntoRepository =>  new LivroAssuntoRepository(_dataContext);

        public ILivroAutorRepository? LivroAutorRepository =>  new LivroAutorRepository(_dataContext);

        public ILivroRepository? LivroRepository =>  new LivroRepository(_dataContext);

        public IPrecoLivroRepository? PrecoLivroRepository => new PrecoLivroRepository(_dataContext);

        public DbContext DataContext => _dataContext!;

        public async Task SaveChanges()
        {
            await _dataContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dataContext.Dispose();
        }
    }
}
