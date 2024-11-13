using BibliotecaApp.Domain.Entities;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaAPP.IntegrationTest.Helpers
{
    public static class LivroTestHelper
    {
        public static Livro GenerateValidLivro()
        {
            return new Faker<Livro>("pt_BR")
                .RuleFor(l => l.Titulo, f => f.Company.CompanyName())
                .RuleFor(l => l.Editora, f => f.Company.CompanyName())
                .RuleFor(l => l.Edicao, f => f.Random.Int(1, 10))
                .RuleFor(l => l.AnoPublicacao, f => f.Date.Past(20).Year.ToString())
                .Generate();
        }
    }
}
