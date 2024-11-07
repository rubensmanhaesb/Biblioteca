using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Entities
{
    public class Assunto
    {
        public int CodAs { get; set; }
        public string? Descricao { get; set; }

        #region Relacionamentos
        public List<LivroAssunto> Livros { get; set; } = new List<LivroAssunto>();
        #endregion
    }
}
