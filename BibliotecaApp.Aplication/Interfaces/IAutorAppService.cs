using BibliotecaApp.Aplication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Interfaces
{
    public interface IAutorAppService : IBaseQueryAppService<AutorResponseDto, int>,
        IBaseInsertAppService<AutorInsertDto, AutorResponseDto>,
        IBaseUpdateAppService<AutorUpdateDto, AutorResponseDto>,
        IBaseDeleteAppService<AutorDeleteDto, AutorResponseDto>

    {
    }
}
