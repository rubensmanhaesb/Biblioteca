using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Interfaces
{
    public interface IBaseQueryAppService<TDtoResponse, Tkey>
    {
        Task<List<TDtoResponse>>? GetAllAsync();
        Task<TDtoResponse>? GetByIdAsync(Tkey id);
    }
}
