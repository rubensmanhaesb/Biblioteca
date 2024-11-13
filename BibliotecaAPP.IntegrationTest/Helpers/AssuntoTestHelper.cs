using BibliotecaApp.Domain.Entities;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaAPP.IntegrationTest.Helpers
{
    public static  class AssuntoTestHelper
    {
        public static Assunto GenerateValidAssunto()
        {
            return new Faker<Assunto>("pt_BR")
                .RuleFor(a => a.Descricao, f => f.Lorem.Letter(20))
                .Generate();
        }
    }
}
