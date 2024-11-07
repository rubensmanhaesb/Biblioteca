using BibliotecaApp.Infra.Data.Repositories;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces.Repositories;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using BibliotecaApp.Infra.Data.Context;

namespace BibliotecaApp.Infra.Data.Test
{
    public class AssuntoRepositoryTest : IDisposable
    {
        private readonly DataContext _context;
        private readonly AssuntoRepository _assuntoRepository;

        public AssuntoRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _context = new DataContext(options);
            _assuntoRepository = new AssuntoRepository(_context);
        }

        [Fact(DisplayName = "Adiciona assunto no repositório")]
        public async Task CreateAssunto_ShouldAddNewAssunto()
        {
            // Arrange
            var faker = new Faker<Assunto>()
                .RuleFor(a => a.CodAs, f => f.Random.Int())
                .RuleFor(a => a.Descricao, f => f.Lorem.Sentence());

            var newAssunto = faker.Generate();

            // Act
            await _assuntoRepository.Add(newAssunto);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _assuntoRepository.GetById(newAssunto.CodAs);
            Assert.NotNull(result);
            Assert.Equal(newAssunto.CodAs, result.CodAs);
            Assert.Equal(newAssunto.Descricao, result.Descricao);
        }

        [Fact(DisplayName = "Obtem um único assunto com base no ID")]
        public async Task GetAssuntoById_ShouldReturnAssunto_WhenAssuntoExists()
        {
            // Arrange
            var faker = new Faker<Assunto>()
                .RuleFor(a => a.CodAs, f => f.Random.Int())
                .RuleFor(a => a.Descricao, f => f.Lorem.Sentence());

            var newAssunto = faker.Generate();
            await _assuntoRepository.Add(newAssunto);
            await _context.SaveChangesAsync();

            // Act
            var result = await _assuntoRepository.GetById(newAssunto.CodAs);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newAssunto.CodAs, result.CodAs);
            Assert.Equal(newAssunto.Descricao, result.Descricao);
        }

        [Fact(DisplayName = "Atualiza o assunto no repositório")]
        public async Task UpdateAssunto_ShouldModifyExistingAssunto()
        {
            // Arrange
            var faker = new Faker<Assunto>()
                .RuleFor(a => a.CodAs, f => f.Random.Int())
                .RuleFor(a => a.Descricao, f => f.Lorem.Sentence());

            var newAssunto = faker.Generate();
            await _assuntoRepository.Add(newAssunto);
            await _context.SaveChangesAsync();

            var updatedAssunto = new Assunto
            {
                CodAs = newAssunto.CodAs,
                Descricao = "Updated Description"
            };

            // Act
            _context.Entry(newAssunto).State = EntityState.Detached; // Detach the existing entity
            await _assuntoRepository.Update(updatedAssunto);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _assuntoRepository.GetById(updatedAssunto.CodAs);
            Assert.NotNull(result);
            Assert.Equal(updatedAssunto.CodAs, result.CodAs);
            Assert.Equal(updatedAssunto.Descricao, result.Descricao);
        }

        [Fact(DisplayName = "Remove o assunto no repositório")]
        public async Task DeleteAssunto_ShouldRemoveAssunto()
        {
            // Arrange
            var faker = new Faker<Assunto>()
                .RuleFor(a => a.CodAs, f => f.Random.Int())
                .RuleFor(a => a.Descricao, f => f.Lorem.Sentence());

            var newAssunto = faker.Generate();
            await _assuntoRepository.Add(newAssunto);
            await _context.SaveChangesAsync();

            await _assuntoRepository.Delete(newAssunto);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _assuntoRepository.GetById(newAssunto.CodAs);
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}