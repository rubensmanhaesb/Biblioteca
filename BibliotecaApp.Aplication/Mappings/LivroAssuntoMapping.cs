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
    public class LivroAssuntoMapping : Profile
    {
        public LivroAssuntoMapping()
        {

            CreateMap<LivroAssunto, LivroAssuntoDto>().ReverseMap();

            CreateMap<LivroAssunto, LivroAssuntoResponseDto>();
            CreateMap<LivroAssuntoDto, LivroAssuntoResponseDto>().ReverseMap();
            CreateMap<LivroAssuntoPkDto, LivroAssuntoPk>().ReverseMap(); 

        }
    }
}
