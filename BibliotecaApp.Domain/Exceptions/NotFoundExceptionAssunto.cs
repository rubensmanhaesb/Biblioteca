using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Exceptions
{
    public class NotFoundExceptionAssunto : BaseNotFoundException
    {
        public NotFoundExceptionAssunto(int CodAs)
                    : base($"Assunto {CodAs} não encontrado.")
        {

        }
    }
}
