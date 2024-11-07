using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Exceptions
{
    public class NotFoundExceptionAutor : BaseNotFoundException
    {
        public NotFoundExceptionAutor(int CodAu)
            : base($"Autor {CodAu} não encontrado.")
        {

        }

    }
}
