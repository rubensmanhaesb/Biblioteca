using BibliotecaApp.Aplication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Interfaces
{
    public interface IBaseAppService<TDtoResponse, TDto, Tkey>
    {
        Task<TDtoResponse> AddAsync(TDto dto);

        Task<TDtoResponse> DeleteAsync(TDto dto);
        
        Task<TDtoResponse> UpdateAsync(TDto dto);

        Task<List<TDtoResponse>>? GetAllAsync();
        Task<TDtoResponse>? GetByIdAsync(Tkey id);
    }
}
