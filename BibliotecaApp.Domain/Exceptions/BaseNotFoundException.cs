using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.Domain.Exceptions
{
    public abstract class BaseNotFoundException : Exception
    {
        public BaseNotFoundException(string message) : base(message)
        {
        }
    }
}
