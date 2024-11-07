using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Entities
{
    public class Autor
    {
        public int CodAu { get; set; }
        public string? Nome { get; set; }

        #region Relacionamentos
        public List<LivroAutor> Livros { get; set; } = new List<LivroAutor>();
        #endregion
    }
}
