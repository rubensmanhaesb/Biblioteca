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
    public class LivroAssuntoAppService : ILivroAssuntoAppService
    {
        private readonly ILivroAssuntoDomainService _livroAssuntoDomain;
        private readonly IMapper _mapper;

        public LivroAssuntoAppService(IMapper mapper, ILivroAssuntoDomainService livroAssuntoDomain)
        {
            _mapper = mapper;
            _livroAssuntoDomain = livroAssuntoDomain;
        }

        public async Task<LivroAssuntoResponseDto> AddAsync(LivroAssuntoDto dto)
        {
            var livroAssunto = _mapper.Map<LivroAssunto>(dto);

            await _livroAssuntoDomain.AddAsync(livroAssunto);

            var responseDto = _mapper.Map<LivroAssuntoResponseDto>(livroAssunto);

            return responseDto;
        }
        public async Task<LivroAssuntoResponseDto> UpdateAsync(LivroAssuntoDto dto)
        {
            var livroAssunto = _mapper.Map<LivroAssunto>(dto);
            await _livroAssuntoDomain.UpdateAsync(livroAssunto);

            var responseDto = _mapper.Map<LivroAssuntoResponseDto>(livroAssunto);

            return responseDto;
        }

        public async Task<LivroAssuntoResponseDto> DeleteAsync(LivroAssuntoDto dto)
        {
            var livroAssunto = _mapper.Map<LivroAssunto>(dto);
            await _livroAssuntoDomain.DeleteAsync(livroAssunto);
            var responseDto = _mapper.Map<LivroAssuntoResponseDto>(livroAssunto);

            return responseDto;
        }

        public async Task<List<LivroAssuntoResponseDto>>? GetAllAsync()
        {
            var lista = await _livroAssuntoDomain.GetAllAsync();
            var listaDto = _mapper.Map<List<LivroAssuntoResponseDto>>(lista);

            return listaDto;
        }

        public async Task<LivroAssuntoResponseDto>? GetByIdAsync(LivroAssuntoPkDto id)
        {

            var livroAssuntoPK = _mapper.Map<LivroAssuntoPk>(id);

            var lista = await _livroAssuntoDomain.GetByIdAsync(livroAssuntoPK);
            var result = _mapper.Map<LivroAssuntoResponseDto>(lista);

            return result;
        }


    }
}
