using BibliotecaApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Entities
{
    public class PrecoLivro
    {
        public int Codp { get; set; } 
        public int LivroCodl { get; set; } 
        public TipoCompra TipoCompra { get; set; } 
        public decimal Valor { get; set; }

        #region Relacionamentos
        public Livro Livro { get; set; }
        #endregion
    }

}
