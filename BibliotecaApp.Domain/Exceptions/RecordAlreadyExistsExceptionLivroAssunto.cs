using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Exceptions
{
    public class RecordAlreadyExistsExceptionLivroAssunto : BaseRecordAlreadyExistsException
    {
        public RecordAlreadyExistsExceptionLivroAssunto(int CodLi, int codAu)
                            : base($"Livro {CodLi} do assunto {codAu}  já existente.")
        {

        }
    }
}
