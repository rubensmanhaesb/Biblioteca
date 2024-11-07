using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAssuntoRepository? AssuntoRepository { get; }
        IAutorRepository? AutorRepository { get; }
        ILivroAssuntoRepository? LivroAssuntoRepository { get; }
        ILivroAutorRepository? LivroAutorRepository { get; }
        ILivroRepository? LivroRepository { get; }
        IPrecoLivroRepository? PrecoLivroRepository { get; }
        DbContext DataContext { get; }
        Task SaveChanges();
    }
}
