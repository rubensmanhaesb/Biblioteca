using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Exceptions
{
    public class NotFoundExceptionLivroAutor : BaseNotFoundException
    {
        public NotFoundExceptionLivroAutor(int CodLi, int CodAu)
                            : base($"Livro Autor {CodLi} e {CodAu} não encontrado.")
        {

        }
    }
}
