using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Interfaces
{
    public interface IBaseDeleteAppService<TDtoDelete, TDtoResponse>
    {
        Task<TDtoResponse> DeleteAsync(TDtoDelete dto);
    }
}
