using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Exceptions
{
    public class RecordAlreadyExistsExceptionLivroAutor : BaseRecordAlreadyExistsException
    {
        public RecordAlreadyExistsExceptionLivroAutor(int CodLi, int codAu)
                            : base($"Livro {CodLi} do autor {codAu}  já existente.")
        {

        }
    }
}

