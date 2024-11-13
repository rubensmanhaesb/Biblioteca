using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaAPP.IntegrationTest.Helpers
{
    public static class PrecoLivroTestHelper
    {
        public static PrecoLivro GenerateValidPrecoLivro()
        {
            return new Faker<PrecoLivro>("pt_BR")
                .RuleFor(p => p.Codp, f => f.Random.Int(min: 1))
                .RuleFor(p => p.LivroCodl, f => f.Random.Int(min: 1))
                .RuleFor(p => p.Valor, f => f.Finance.Amount(1)) // Valor sempre maior que zero
                .RuleFor(p => p.TipoCompra, f => f.PickRandom<TipoCompra>())
                .Generate();
        }
    }
}
