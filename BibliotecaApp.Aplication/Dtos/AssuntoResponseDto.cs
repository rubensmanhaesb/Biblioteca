using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Dtos
{
    public class AssuntoResponseDto
    {
        public int CodAs { get; set; }
        public string? Descricao { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is AssuntoResponseDto other)
            {
                return CodAs == other.CodAs &&
                       string.Equals(Descricao, other.Descricao, StringComparison.Ordinal);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CodAs, Descricao);
        }
    }
}
