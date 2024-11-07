using BibliotecaApp.Infra.Data.Repositories;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces.Repositories;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using BibliotecaApp.Infra.Data.Context;

namespace BibliotecaApp.Infra.Data.Test
{
    public class AutorRepositoryTest : IDisposable
    {
        private readonly DataContext _context;
        private readonly AutorRepository _autorRepository;

        public AutorRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _context = new DataContext(options);
            _autorRepository = new AutorRepository(_context);
        }

        [Fact(DisplayName = "Adiciona autor no repositório")]
        public async Task CreateAutor_ShouldAddNewAutor()
        {
            // Arrange
            var faker = new Faker<Autor>()
                .RuleFor(a => a.CodAu, f => f.Random.Int())
                .RuleFor(a => a.Nome, f => f.Person.FullName);

            var newAutor = faker.Generate();

            // Act
            await _autorRepository.Add(newAutor);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _autorRepository.GetById(newAutor.CodAu);
            Assert.NotNull(result);
            Assert.Equal(newAutor.CodAu, result.CodAu);
            Assert.Equal(newAutor.Nome, result.Nome);
        }

        [Fact(DisplayName = "Obtem um único autor com base no ID")]
        public async Task GetAutorById_ShouldReturnAutor_WhenAutorExists()
        {
            // Arrange
            var faker = new Faker<Autor>()
                .RuleFor(a => a.CodAu, f => f.Random.Int())
                .RuleFor(a => a.Nome, f => f.Person.FullName);

            var newAutor = faker.Generate();
            await _autorRepository.Add(newAutor);
            await _context.SaveChangesAsync();

            // Act
            var result = await _autorRepository.GetById(newAutor.CodAu);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newAutor.CodAu, result.CodAu);
            Assert.Equal(newAutor.Nome, result.Nome);
        }

        [Fact(DisplayName = "Atualiza o autor no repositório")]
        public async Task UpdateAutor_ShouldModifyExistingAutor()
        {
            // Arrange
            var faker = new Faker<Autor>()
                .RuleFor(a => a.CodAu, f => f.Random.Int())
                .RuleFor(a => a.Nome, f => f.Person.FullName);

            var newAutor = faker.Generate();
            await _autorRepository.Add(newAutor);
            await _context.SaveChangesAsync();

            var updatedAutor = new Autor
            {
                CodAu = newAutor.CodAu,
                Nome = "Updated Name"
            };

            // Act
            _context.Entry(newAutor).State = EntityState.Detached; // Detach the existing entity
            await _autorRepository.Update(updatedAutor);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _autorRepository.GetById(updatedAutor.CodAu);
            Assert.NotNull(result);
            Assert.Equal(updatedAutor.CodAu, result.CodAu);
            Assert.Equal(updatedAutor.Nome, result.Nome);
        }

        [Fact(DisplayName = "Remove o autor no repositório")]
        public async Task DeleteAutor_ShouldRemoveAutor()
        {
            // Arrange
            var faker = new Faker<Autor>()
                .RuleFor(a => a.CodAu, f => f.Random.Int())
                .RuleFor(a => a.Nome, f => f.Person.FullName);

            var newAutor = faker.Generate();
            await _autorRepository.Add(newAutor);
            await _context.SaveChangesAsync();

            // Act
            _context.Entry(newAutor).State = EntityState.Detached; // Detach the existing entity
            await _autorRepository.Delete(newAutor);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _autorRepository.GetById(newAutor.CodAu);
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
