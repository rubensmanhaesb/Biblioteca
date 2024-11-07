using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Interfaces
{
    public interface IPrecoLivroAppService : IBaseQueryAppService<PrecoLivroResponseDto, int>,
        IBaseInsertAppService<PrecoLivroInsertDto, PrecoLivroResponseDto>,
        IBaseUpdateAppService<PrecoLivroUpdateDto, PrecoLivroResponseDto>,
        IBaseDeleteAppService<PrecoLivroDeleteDto, PrecoLivroResponseDto>
    {
        Task<List<PrecoLivro?>> GetAllByKey(int codL, TipoCompra tipoCompra, int? pageNumber = 1);
    }
}
