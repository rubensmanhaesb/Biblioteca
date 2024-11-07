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
    public class AutorMapping : Profile
    {
        public AutorMapping()
        {
            CreateMap<Autor, AutorUpdateDto>().ReverseMap();
            CreateMap<Autor, AutorDeleteDto>().ReverseMap();
            CreateMap<Autor, AutorInsertDto>().ReverseMap();

            CreateMap<Autor, AutorResponseDto>();
        }
    }
}
