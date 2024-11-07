using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Exceptions
{
    public class NotFoundExceptionPrecoLivro : BaseNotFoundException
    {
        public NotFoundExceptionPrecoLivro(int Codl)
                            : base($"Preço Livro {Codl} não encontrado.")
        {

        }
    }
}
