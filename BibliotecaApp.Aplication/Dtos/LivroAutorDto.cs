using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Domain.Entities;
namespace BibliotecaApp.Aplication.Dtos
{
    public class LivroAutorDto
    {
        public int LivroCodl { get; set; }
        public int AutorCodAu { get; set; }

        public LivroAutorPkDto Pk()
        {     
            return new LivroAutorPkDto()
                {
                    LivroCodl = LivroCodl,
                    AutorCodAu = AutorCodAu
                };
        }
    }

    public class LivroAutorPkDto
    {
        public int LivroCodl { get; set; }
        public int AutorCodAu { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is LivroAutorPkDto pk)
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