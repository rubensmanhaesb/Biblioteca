using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Services;
using BibliotecaApp.Infra.Data.Context;
using BibliotecaApp.Infra.Data.Repositories;
using FluentValidation;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using ValidationException = FluentValidation.ValidationException;
using Microsoft.Extensions.Logging;
using BibliotecaAPP.IntegrationTest.Helpers;

namespace BibliotecaAPP.IntegrationTest.Helpers
{
    public static class LivroAssuntoTestHelper
    {
        public static async Task<LivroAssunto> GenerateValidLivroAssunto(IUnitOfWork unitOfWork)
        {
            var livro = LivroTestHelper.GenerateValidLivro();
            await unitOfWork.LivroRepository!.Add(livro);

            var assunto = AssuntoTestHelper.GenerateValidAssunto();
            await unitOfWork.AssuntoRepository!.Add(assunto);

            return new LivroAssunto
            {
                LivroCodl = livro.Codl,
                AssuntoCodAs = assunto.CodAs
            };
        }
    }
}
