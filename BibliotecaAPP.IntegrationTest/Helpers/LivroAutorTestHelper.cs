using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces.Repositories;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaAPP.IntegrationTest.Helpers
{
    public static class LivroAutorTestHelper
    {

        public static async Task<LivroAutor> GenerateValidLivroAutor(IUnitOfWork unitOfWork)
        {

            var livro = LivroTestHelper.GenerateValidLivro();
            await unitOfWork.LivroRepository.Add(livro);

            var autor = AutorTestHelper.GenerateValidAutor();
            await unitOfWork.AutorRepository.Add(autor);

            return new LivroAutor
            {
                LivroCodl = livro.Codl,
                AutorCodAu = autor.CodAu
            };
        }

    }
}
