using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Entities
{
    public class LivroAutor
    {
        public int LivroCodl { get; set; }
        public int AutorCodAu { get; set; }
        
        #region Relacionamentos
        public Livro Livro { get; set; }
        public Autor Autor { get; set; }
        #endregion
        public LivroAutorPk Pk => new LivroAutorPk
        {
            LivroCodl = LivroCodl,
            AutorCodAu = AutorCodAu
        };
    }

    public class LivroAutorPk
    {
        public int LivroCodl { get; set; }
        public int AutorCodAu { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is LivroAutorPk pk)
            {
                return LivroCodl == pk.LivroCodl && AutorCodAu == pk.AutorCodAu;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LivroCodl, AutorCodAu);
        }
    }

}
