using AutoMapper;
using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Mappings
{
    public class LivroAutorMapping : Profile
    {
        public LivroAutorMapping()
        {
 
            CreateMap<LivroAutor, LivroAutorDto>().ReverseMap();

            CreateMap<LivroAutor, LivroAutorResponseDto>();
            CreateMap<LivroAutorDto, LivroAutorResponseDto>().ReverseMap();

            CreateMap<LivroAutorPk, LivroAutorPkDto>().ReverseMap();
        }
    }
}
