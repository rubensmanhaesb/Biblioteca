using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Dtos
{
    public class LivroResponseDto
    {
        public int Codl { get; set; }
        public string? Titulo { get; set; }
        public string? Editora { get; set; }
        public int? Edicao { get; set; }
        public string? AnoPublicacao { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is LivroResponseDto other)
            {
                return Codl == other.Codl &&
                       Titulo == other.Titulo &&
                       Editora == other.Editora &&
                       Edicao == other.Edicao &&
                       AnoPublicacao == other.AnoPublicacao;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Codl, Titulo, Editora, Edicao, AnoPublicacao);
        }
    }
}
