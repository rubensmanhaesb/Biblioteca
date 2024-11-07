using AutoMapper;
using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Domain.Entities;


namespace BibliotecaApp.Aplication.Mappings
{
    public class AssuntoMapping : Profile
    {
        public AssuntoMapping()
        {
            CreateMap<Assunto, AssuntoUpdateDto>().ReverseMap();
            CreateMap<Assunto, AssuntoDeleteDto>().ReverseMap();
            CreateMap<Assunto, AssuntoInsertDto>().ReverseMap();

            CreateMap<Assunto, AssuntoResponseDto>().ReverseMap();
        }
    }
}
