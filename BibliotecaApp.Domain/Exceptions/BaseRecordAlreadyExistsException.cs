using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Exceptions
{
    public class BaseRecordAlreadyExistsException : Exception
    {
        public BaseRecordAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
