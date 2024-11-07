using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Exceptions
{
    public class NotFoundExceptionLivro : BaseNotFoundException
    {
        public NotFoundExceptionLivro(int CodLi)
                            : base($"Livro {CodLi} não encontrado.")
        {

        }
    }
}
