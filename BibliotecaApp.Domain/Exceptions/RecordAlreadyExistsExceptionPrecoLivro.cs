using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Exceptions
{
    public class RecordAlreadyExistsExceptionPrecoLivro : BaseRecordAlreadyExistsException
    {
        public RecordAlreadyExistsExceptionPrecoLivro(int Codp)
                            : base($"Preço {Codp} já existente.")
        {

        }

    }
}
