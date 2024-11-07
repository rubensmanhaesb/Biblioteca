using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Interfaces
{
    public interface IBaseUpdateAppService<TDtoUpdate, TDtoResponse>
    {
        Task<TDtoResponse> UpdateAsync(TDtoUpdate dto);
    }
}
