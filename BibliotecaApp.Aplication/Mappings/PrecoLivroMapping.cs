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
    public class PrecoLivroMapping : Profile
    {
        public PrecoLivroMapping()
        {

            CreateMap<PrecoLivro, PrecoLivroUpdateDto>().ReverseMap();
            CreateMap<PrecoLivro, PrecoLivroDeleteDto>().ReverseMap();
            CreateMap<PrecoLivro, PrecoLivroInsertDto>().ReverseMap();

            CreateMap<PrecoLivro, PrecoLivroResponseDto>();

        }
    }
}
