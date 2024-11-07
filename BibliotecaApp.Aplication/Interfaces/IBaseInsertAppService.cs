using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Interfaces
{
    public interface IBaseInsertAppService<TDtoInsert, TDtoResponse> 
    {
        Task<TDtoResponse> AddAsync(TDtoInsert dto);
    }
}
