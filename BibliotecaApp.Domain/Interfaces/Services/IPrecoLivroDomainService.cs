using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using BibliotecaApp.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Interfaces.Services
{
    public interface IPrecoLivroDomainService : IBaseDomainService<PrecoLivro, int>, IBaseQueryRepository<PrecoLivro>
    {

    }
}
