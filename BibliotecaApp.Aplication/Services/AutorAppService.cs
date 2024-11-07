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
    public class AutorAppService : IAutorAppService
    {
        private readonly IAutorDomainService _autorDomain;
        private readonly IMapper _mapper;

        public AutorAppService(IMapper mapper, IAutorDomainService autorDomain)
        {
            _mapper = mapper;
            _autorDomain = autorDomain;
        }

        public async Task<AutorResponseDto> AddAsync(AutorInsertDto dto)
        {
            var autor = _mapper.Map<Autor>(dto);

            await _autorDomain.AddAsync(autor);

            var responseDto = _mapper.Map<AutorResponseDto>(autor);

            return responseDto;
        }
        public async Task<AutorResponseDto> UpdateAsync(AutorUpdateDto dto)
        {
            var autor = _mapper.Map<Autor>(dto);
            await _autorDomain.UpdateAsync(autor);

            var responseDto = _mapper.Map<AutorResponseDto>(autor);

            return responseDto;
        }

        public async Task<AutorResponseDto> DeleteAsync(AutorDeleteDto dto)
        {
            var bkautor = await GetByIdAsync(dto.CodAu);
            var autor = _mapper.Map<Autor>(dto);
            await _autorDomain.DeleteAsync(autor);
            var responseDto = _mapper.Map<AutorResponseDto>(bkautor);

            return responseDto;
        }

        public async Task<List<AutorResponseDto>>? GetAllAsync()
        {
            var lista = await _autorDomain.GetAllAsync();
            var listaDto = _mapper.Map<List<AutorResponseDto>>(lista);

            return listaDto;
        }

        public async Task<AutorResponseDto>? GetByIdAsync(int id)
        {
            var lista = await _autorDomain.GetByIdAsync(id);
            var result = _mapper.Map<AutorResponseDto>(lista);

            return result;
        }



    }
}
