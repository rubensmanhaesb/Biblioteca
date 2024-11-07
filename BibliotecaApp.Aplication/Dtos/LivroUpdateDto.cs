using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Dtos
{
    public class LivroUpdateDto
    {
        public int Codl { get; set; }
        public string? Titulo { get; set; }
        public string? Editora { get; set; }
        public int? Edicao { get; set; }
        public string? AnoPublicacao { get; set; }
    }
}