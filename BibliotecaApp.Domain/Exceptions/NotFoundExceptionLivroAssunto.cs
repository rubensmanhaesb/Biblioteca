using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Exceptions
{
    public class NotFoundExceptionLivroAssunto : BaseNotFoundException
    {
        public NotFoundExceptionLivroAssunto(int CodLi, int CodAs)
                            : base($"Livro Assunto {CodLi} e {CodAs} não encontrado.")
        {

        }
    }
}
