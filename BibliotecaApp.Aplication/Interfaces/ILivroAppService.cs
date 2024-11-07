using BibliotecaApp.Aplication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Interfaces
{
    public interface ILivroAppService : IBaseQueryAppService<LivroResponseDto, int>,
        IBaseInsertAppService<LivroInsertDto, LivroResponseDto>,
        IBaseUpdateAppService<LivroUpdateDto, LivroResponseDto>,
        IBaseDeleteAppService<LivroDeleteDto, LivroResponseDto>
    {

    }
}
