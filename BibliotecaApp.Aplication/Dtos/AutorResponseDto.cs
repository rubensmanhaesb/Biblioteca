using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Dtos
{
    public class AutorResponseDto
    {
        public int CodAu { get; set; }
        public string? Nome { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is AutorResponseDto other)
            {
                return CodAu == other.CodAu &&
                       string.Equals(Nome, other.Nome, StringComparison.Ordinal);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CodAu, Nome);
        }
    }


}
