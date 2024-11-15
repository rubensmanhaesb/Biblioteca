using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApp.API.Tests.Validations
{
    public class ValidationError
    {
        public string Field { get; set; }
        public string ErrorMessage { get; set; }
        public string Severity { get; set; }
    }
}
