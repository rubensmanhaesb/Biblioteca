using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Entities
{
    public class LivroAssunto
    {
        public int LivroCodl { get; set; }
        public int AssuntoCodAs { get; set; }

        #region Relacionamentos
        public Livro Livro { get; set; }
        public Assunto Assunto { get; set; }
        #endregion

        public LivroAssuntoPk Pk()
        {
            return new LivroAssuntoPk
            {
                LivroCodl = LivroCodl,
                AssuntoCodAs = AssuntoCodAs
            };
        }
    }

    public class LivroAssuntoPk
    {
        public int LivroCodl { get; set; }
        public int AssuntoCodAs { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is LivroAssuntoPk pk)
            {
                return LivroCodl == pk.LivroCodl && AssuntoCodAs == pk.AssuntoCodAs;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LivroCodl, AssuntoCodAs);
        }
    }

}