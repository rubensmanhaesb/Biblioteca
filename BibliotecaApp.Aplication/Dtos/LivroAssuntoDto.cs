using BibliotecaApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Aplication.Dtos
{
    public  class LivroAssuntoDto
    {
        public int LivroCodl { get; set; }
        public int AssuntoCodAs { get; set; }

        public LivroAssuntoPkDto Pk()
        {
            return new LivroAssuntoPkDto()
            {
                LivroCodl = LivroCodl,
                AssuntoCodAs = AssuntoCodAs
            };
        }
    }
    public class LivroAssuntoPkDto
    {
        public int LivroCodl { get; set; }
        public int AssuntoCodAs { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is LivroAssuntoPkDto pk)
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
