using AutoMapper;
using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Interfaces;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using BibliotecaApp.Domain.Interfaces.Services;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Services
{
    public class PrecoLivroAppService : IPrecoLivroAppService
    {
        private readonly IPrecoLivroDomainService _precoLivroDomainService;
        private readonly IMapper _mapper;

        public PrecoLivroAppService(IMapper mapper, IPrecoLivroDomainService precoLivroDomainService)
        {
            _mapper = mapper;
            _precoLivroDomainService = precoLivroDomainService;
        }

        public async Task<PrecoLivroResponseDto> AddAsync(PrecoLivroInsertDto dto)
        {
            var PrecoLivro = _mapper.Map<PrecoLivro>(dto);
            await _precoLivroDomainService.AddAsync(PrecoLivro);
            var responseDto = _mapper.Map<PrecoLivroResponseDto>(PrecoLivro);

            return responseDto;
        }

        public async Task<PrecoLivroResponseDto> UpdateAsync(PrecoLivroUpdateDto dto)
        {
            var PrecoLivro = _mapper.Map<PrecoLivro>(dto);
            await _precoLivroDomainService.UpdateAsync(PrecoLivro);
            var responseDto = _mapper.Map<PrecoLivroResponseDto>(PrecoLivro);

            return responseDto;
        }

        public async Task<PrecoLivroResponseDto> DeleteAsync(PrecoLivroDeleteDto dto)
        {
            var PrecoLivro = _mapper.Map<PrecoLivro>(dto);
            await _precoLivroDomainService.DeleteAsync(PrecoLivro);
            var responseDto = _mapper.Map<PrecoLivroResponseDto>(PrecoLivro);

            return responseDto;
        }

        public async Task<List<PrecoLivroResponseDto>> GetAllAsync()
        {
            var lista = await _precoLivroDomainService.GetAllAsync();
            var listaDto = _mapper.Map<List<PrecoLivroResponseDto>>(lista);

            return listaDto;
        }

        public async Task<PrecoLivroResponseDto> GetByIdAsync(int id)
        {
            var PrecoLivro = await _precoLivroDomainService.GetByIdAsync(id);
            var responseDto = _mapper.Map<PrecoLivroResponseDto>(PrecoLivro);

            return responseDto;
        }
        public async Task<List<PrecoLivro?>> GetAllByKey(int codL, TipoCompra tipoCompra, int? pageNumber=1)
        {
            var lista = await _precoLivroDomainService.GetByConditionAsync(
                pageSize: 10,
                pageNumber: pageNumber,
                predicate: x => x.LivroCodl == codL && x.TipoCompra == tipoCompra,
                orderBy: new Expression<Func<PrecoLivro, object>>[]
                    { x => x.TipoCompra==tipoCompra}
                ).ConfigureAwait(false);

            return (List<PrecoLivro?>)lista;

        }

    }
}
