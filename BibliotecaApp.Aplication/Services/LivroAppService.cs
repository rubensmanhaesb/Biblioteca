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
    public class LivroAppService : ILivroAppService
    {
        private readonly ILivroDomainService _livroDomain;
        private readonly IMapper _mapper;

        public LivroAppService(IMapper mapper, ILivroDomainService livroDomain)
        {
            _mapper = mapper;
            _livroDomain = livroDomain;
        }

        public async Task<LivroResponseDto> AddAsync(LivroInsertDto dto)
        {
            var livro = _mapper.Map<Livro>(dto);

            await _livroDomain.AddAsync(livro);

            var responseDto = _mapper.Map<LivroResponseDto>(livro);

            return responseDto;
        }
        public async Task<LivroResponseDto> UpdateAsync(LivroUpdateDto dto)
        {
            var livro = _mapper.Map<Livro>(dto);
            await _livroDomain.UpdateAsync(livro);

            var responseDto = _mapper.Map<LivroResponseDto>(livro);
            
            return responseDto;
        }

        public async Task<LivroResponseDto> DeleteAsync(LivroDeleteDto dto)
        {
            var bklivro = await GetByIdAsync(dto.Codl);
            var livro = _mapper.Map<Livro>(dto);
            await _livroDomain.DeleteAsync(livro);
            var responseDto = _mapper.Map<LivroResponseDto>(bklivro);
            
            return responseDto;
        }

        public async Task<List<LivroResponseDto>>? GetAllAsync()
        {
            var lista = await _livroDomain.GetAllAsync();
            var listaDto = _mapper.Map<List<LivroResponseDto>>(lista);
            
            return listaDto;
        }

        public async Task<LivroResponseDto>? GetByIdAsync(int id)
        {
            var lista = await _livroDomain.GetByIdAsync(id);
            var result = _mapper.Map<LivroResponseDto>(lista);

            return result;
        }


    }
}
