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
    public class LivroMapping : Profile
    {
        public LivroMapping()
        {
            CreateMap<Livro, LivroUpdateDto>().ReverseMap();
            CreateMap<Livro, LivroDeleteDto>().ReverseMap();
            CreateMap<Livro, LivroInsertDto>().ReverseMap();

            CreateMap<Livro, LivroResponseDto>();

        }
    }
}
