using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Infra.Data.Context;
using BibliotecaApp.Infra.Data.Repositories;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BibliotecaApp.Infra.Data.Test
{
    public class PrecoLivroRepositoryTest : IDisposable
    {
        private readonly DataContext _context;
        private readonly PrecoLivroRepository _precoLivroRepository;
        private readonly Faker<PrecoLivro> _faker;

        public PrecoLivroRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _context = new DataContext(options);
            _precoLivroRepository = new PrecoLivroRepository(_context);

            _faker = new Faker<PrecoLivro>()
                .RuleFor(p => p.Codp, f => f.Random.Int())
                .RuleFor(p => p.LivroCodl, f => f.Random.Int())
                .RuleFor(p => p.Valor, f => f.Finance.Amount());
        }

        private async Task<PrecoLivro> CreateAndAddPrecoLivroAsync()
        {
            var precoLivro = _faker.Generate();
            await _precoLivroRepository.Add(precoLivro);
            await _context.SaveChangesAsync();
            return precoLivro;
        }

        [Fact(DisplayName = "Adiciona preço de livro no repositório")]
        public async Task CreatePrecoLivro_ShouldAddNewPrecoLivro()
        {
            // Arrange
            var newPrecoLivro = _faker.Generate();

            // Act
            await _precoLivroRepository.Add(newPrecoLivro);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _precoLivroRepository.GetById(newPrecoLivro.Codp);
            Assert.NotNull(result);
            Assert.Equal(newPrecoLivro.Codp, result.Codp);
            Assert.Equal(newPrecoLivro.Valor, result.Valor);
        }

        [Fact(DisplayName = "Obtem um único preço de livro com base no ID")]
        public async Task GetPrecoLivroById_ShouldReturnPrecoLivro_WhenPrecoLivroExists()
        {
            // Arrange
            var precoLivro = await CreateAndAddPrecoLivroAsync();

            // Act
            var result = await _precoLivroRepository.GetById(precoLivro.Codp);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(precoLivro.Codp, result.Codp);
            Assert.Equal(precoLivro.Valor, result.Valor);
        }

        [Fact(DisplayName = "Atualiza o preço de livro no repositório")]
        public async Task UpdatePrecoLivro_ShouldModifyExistingPrecoLivro()
        {
            // Arrange
            var precoLivro = await CreateAndAddPrecoLivroAsync();
            _context.Entry(precoLivro).State = EntityState.Detached;

            var updatedPrecoLivro = new PrecoLivro
            {
                Codp = precoLivro.Codp,
                LivroCodl = precoLivro.LivroCodl,
                Valor = precoLivro.Valor + 5 // Alterando o preço para teste
            };

            // Act
            await _precoLivroRepository.Update(updatedPrecoLivro);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _precoLivroRepository.GetById(updatedPrecoLivro.Codp);
            Assert.NotNull(result);
            Assert.Equal(updatedPrecoLivro.Codp, result.Codp);
            Assert.Equal(updatedPrecoLivro.Valor, result.Valor);
        }

        [Fact(DisplayName = "Remove o preço de livro no repositório")]
        public async Task DeletePrecoLivro_ShouldRemovePrecoLivro()
        {
            // Arrange
            var precoLivro = await CreateAndAddPrecoLivroAsync();
            _context.Entry(precoLivro).State = EntityState.Detached;

            // Act
            await _precoLivroRepository.Delete(precoLivro);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _precoLivroRepository.GetById(precoLivro.Codp);
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
