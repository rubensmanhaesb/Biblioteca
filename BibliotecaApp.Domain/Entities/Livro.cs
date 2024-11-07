using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Entities
{
    public class Livro
    {
        public int Codl { get; set; }
        public string? Titulo { get; set; }
        public string? Editora { get; set; }
        public int? Edicao { get; set; }
        public string? AnoPublicacao { get; set; }

        #region Relacionamentos
        public List<LivroAutor> Autores { get; set; } = new List<LivroAutor>();
        public List<LivroAssunto> Assuntos { get; set; } = new List<LivroAssunto>();
        public List<PrecoLivro> Precos { get; set; } = new List<PrecoLivro>();
        #endregion
    }
}
