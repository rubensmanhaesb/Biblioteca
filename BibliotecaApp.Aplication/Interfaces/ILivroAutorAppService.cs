using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Interfaces
{
    public interface ILivroAutorAppService : IBaseQueryAppService<LivroAutorResponseDto, LivroAutorPkDto>,
        IBaseInsertAppService<LivroAutorDto, LivroAutorResponseDto>,
        IBaseDeleteAppService<LivroAutorDto, LivroAutorResponseDto>
    {
    }
}
