using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.API.Tests.Validations
{
    public class ValidationErrorResponse
    {
        public string Message { get; set; }
        public List<ValidationError> Errors { get; set; }
    }

}
