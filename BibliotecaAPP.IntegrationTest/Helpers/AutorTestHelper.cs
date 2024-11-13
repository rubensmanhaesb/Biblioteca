using BibliotecaApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaAPP.IntegrationTest.Helpers
{
    public static class AutorTestHelper
    {
        public static Autor GenerateValidAutor()
        {
            return new Autor
            {
                Nome = "Nome Válido"
            };
        }
    }
}
