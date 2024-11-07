using BibliotecaApp.Infra.Data.Repositories;
using BibliotecaApp.Domain.Entities;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using BibliotecaApp.Infra.Data.Context;
using System.Collections.Generic;

namespace BibliotecaApp.Infra.Data.Test
{
    public class LivroAutorRepositoryTest : IDisposable
    {
        private readonly DataContext _context;
        private readonly LivroAutorRepository _livroAutorRepository;

        public LivroAutorRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _context = new DataContext(options);
            _livroAutorRepository = new LivroAutorRepository(_context);
        }

        [Fact(DisplayName = "Adiciona livroautor no repositório")]
        public async Task CreateLivroAutor_ShouldAddNewLivroAutor()
        {
            // Arrange
            var faker = new Faker<LivroAutor>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AutorCodAu, f => f.Random.Int());

            var newLivroAutor = faker.Generate();

            // Act
            await _livroAutorRepository.Add(newLivroAutor);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _livroAutorRepository.GetById(newLivroAutor.Pk);
            Assert.NotNull(result);
            Assert.Equal(newLivroAutor.LivroCodl, result.LivroCodl);
            Assert.Equal(newLivroAutor.AutorCodAu, result.AutorCodAu);
        }

        [Fact(DisplayName = "Atualiza livroautor no repositório")]
        public async Task UpdateLivroAutor_ShouldModifyExistingLivroAutor()
        {
            // Arrange
            var faker = new Faker<LivroAutor>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AutorCodAu, f => f.Random.Int());

            var newLivroAutor = faker.Generate();
            await _livroAutorRepository.Add(newLivroAutor);
            await _context.SaveChangesAsync();

            // Detach the existing entity
            _context.Entry(newLivroAutor).State = EntityState.Detached;

            var updatedLivroAutor = new LivroAutor
            {
                LivroCodl = newLivroAutor.LivroCodl,
                AutorCodAu = newLivroAutor.AutorCodAu
            };

            // Act
            await _livroAutorRepository.Update(updatedLivroAutor);
            await _context.SaveChangesAsync();
            

            // Assert
            var result = await _livroAutorRepository.GetById(updatedLivroAutor.Pk);
            Assert.NotNull(result);
            Assert.Equal(updatedLivroAutor.LivroCodl, result.LivroCodl);
            Assert.Equal(updatedLivroAutor.AutorCodAu, result.AutorCodAu);
        }

        [Fact(DisplayName = "Remove livroautor no repositório")]
        public async Task DeleteLivroAutor_ShouldRemoveLivroAutor()
        {
            // Arrange
            var faker = new Faker<LivroAutor>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AutorCodAu, f => f.Random.Int());

            var newLivroAutor = faker.Generate();
            await _livroAutorRepository.Add(newLivroAutor);
            await _context.SaveChangesAsync();

            // Act
            _context.Entry(newLivroAutor).State = EntityState.Detached; // Detach the existing entity
            await _livroAutorRepository.Delete(newLivroAutor);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _livroAutorRepository.GetById(newLivroAutor.Pk);
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
