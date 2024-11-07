using BibliotecaApp.Aplication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Interfaces
{
    public interface IAssuntoAppService : IBaseQueryAppService<AssuntoResponseDto, int>,
        IBaseInsertAppService<AssuntoInsertDto, AssuntoResponseDto>,
        IBaseUpdateAppService<AssuntoUpdateDto, AssuntoResponseDto>,
        IBaseDeleteAppService<AssuntoDeleteDto, AssuntoResponseDto>
    {

    }
}
