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
    public class LivroAutorAppService : ILivroAutorAppService
    {
        private readonly ILivroAutorDomainService _livroAutorDomain;
        private readonly IMapper _mapper;

        public LivroAutorAppService(IMapper mapper, ILivroAutorDomainService livroAutorDomain)
        {
            _mapper = mapper;
            _livroAutorDomain = livroAutorDomain;
        }

        public async Task<LivroAutorResponseDto> UpdateAsync(LivroAutorDto dto)
        {
            var livroAutor = _mapper.Map<LivroAutor>(dto);
            await _livroAutorDomain.UpdateAsync(livroAutor);

            var responseDto = _mapper.Map<LivroAutorResponseDto>(livroAutor);

            return responseDto;
        }

        public async Task<LivroAutorResponseDto> DeleteAsync(LivroAutorDto dto)
        {
            var livroAutor = _mapper.Map<LivroAutor>(dto);
            await _livroAutorDomain.DeleteAsync(livroAutor);
            var responseDto = _mapper.Map<LivroAutorResponseDto>(livroAutor);

            return responseDto;
        }

        public async Task<List<LivroAutorResponseDto>>? GetAllAsync()
        {
            var lista = await _livroAutorDomain.GetAllAsync();
            var listaDto = _mapper.Map<List<LivroAutorResponseDto>>(lista);

            return listaDto;
        }

        public async Task<LivroAutorResponseDto>? GetByIdAsync(LivroAutorPkDto id)
        {

            var livroAutorPK = _mapper.Map<LivroAutorPk>(id);

            var lista = await _livroAutorDomain.GetByIdAsync(livroAutorPK);
            var result = _mapper.Map<LivroAutorResponseDto>(lista);

            return result;
        }

        public async Task<LivroAutorResponseDto> AddAsync(LivroAutorDto dto)
        {
            var livroAutor = _mapper.Map<LivroAutor>(dto);

            await _livroAutorDomain.AddAsync(livroAutor);
           
            var responseDto = _mapper.Map<LivroAutorResponseDto>(livroAutor);

            return responseDto;

        }
    }
}
