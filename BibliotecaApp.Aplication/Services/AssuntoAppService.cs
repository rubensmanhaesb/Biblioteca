using AutoMapper;
using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Interfaces;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces.Services;
using BibliotecaApp.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Services
{
    public class AssuntoAppService : IAssuntoAppService
    {
        private readonly IAssuntoDomainService _assuntoDomain;
        private readonly IMapper _mapper;

        public AssuntoAppService(IMapper mapper, IAssuntoDomainService assuntoDomain)
        {
            _mapper = mapper;
            _assuntoDomain = assuntoDomain;
        }

        public async Task<AssuntoResponseDto> AddAsync(AssuntoInsertDto dto)
        {
            var assunto = _mapper.Map<Assunto>(dto);

            await _assuntoDomain.AddAsync(assunto);

            var responseDto = _mapper.Map<AssuntoResponseDto>(assunto);

            return responseDto;
        }
        public async Task<AssuntoResponseDto> UpdateAsync(AssuntoUpdateDto dto)
        {
            var assunto = _mapper.Map<Assunto>(dto);
            await _assuntoDomain.UpdateAsync(assunto);

            var responseDto = _mapper.Map<AssuntoResponseDto>(assunto);

            return responseDto;
        }

        public async Task<AssuntoResponseDto> DeleteAsync(AssuntoDeleteDto dto)
        {
            var bkassunto = await GetByIdAsync(dto.CodAs);
            var assunto = _mapper.Map<Assunto>(dto);
            await _assuntoDomain.DeleteAsync(assunto);

            var responseDto = _mapper.Map<AssuntoResponseDto>(bkassunto);

            return responseDto;
        }

        public async Task<List<AssuntoResponseDto>>? GetAllAsync()
        {
            var lista = await _assuntoDomain.GetAllAsync();
            var listaDto = _mapper.Map<List<AssuntoResponseDto>>(lista);

            return listaDto;
        }

        public async Task<AssuntoResponseDto>? GetByIdAsync(int id)
        {
            var lista = await _assuntoDomain.GetByIdAsync(id);
            var result = _mapper.Map<AssuntoResponseDto>(lista);

            return result;
        }



    }
}
