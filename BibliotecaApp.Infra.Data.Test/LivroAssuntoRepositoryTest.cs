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
    public class LivroAssuntoRepositoryTest : IDisposable
    {
        private readonly DataContext _context;
        private readonly LivroAssuntoRepository _livroAssuntoRepository;

        public LivroAssuntoRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _context = new DataContext(options);
            _livroAssuntoRepository = new LivroAssuntoRepository(_context);
        }

        [Fact(DisplayName = "Adiciona livroassunto no repositório")]
        public async Task CreateLivroAssunto_ShouldAddNewLivroAssunto()
        {
            // Arrange
            var faker = new Faker<LivroAssunto>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AssuntoCodAs, f => f.Random.Int());

            var newLivroAssunto = faker.Generate();

            // Act
            await _livroAssuntoRepository.Add(newLivroAssunto);
            await _context.SaveChangesAsync();

            var pk = new LivroAssuntoPk
            {
                LivroCodl = newLivroAssunto.LivroCodl,
                AssuntoCodAs = newLivroAssunto.AssuntoCodAs
            };

            // Assert
            var result = await _livroAssuntoRepository.GetById(pk);
            Assert.NotNull(result);
            Assert.Equal(newLivroAssunto.LivroCodl, result.LivroCodl);
            Assert.Equal(newLivroAssunto.AssuntoCodAs, result.AssuntoCodAs);
        }

        [Fact(DisplayName = "Atualiza livroassunto no repositório")]
        public async Task UpdateLivroAssunto_ShouldModifyExistingLivroAssunto()
        {
            // Arrange
            var faker = new Faker<LivroAssunto>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AssuntoCodAs, f => f.Random.Int());

            var newLivroAssunto = faker.Generate();
            await _livroAssuntoRepository.Add(newLivroAssunto);
            await _context.SaveChangesAsync();

            // Detach the existing entity
            _context.Entry(newLivroAssunto).State = EntityState.Detached;

            var updatedLivroAssunto = new LivroAssunto
            {
                LivroCodl = newLivroAssunto.LivroCodl,
                AssuntoCodAs = newLivroAssunto.AssuntoCodAs
            };

            // Act
            await _livroAssuntoRepository.Update(updatedLivroAssunto);
            await _context.SaveChangesAsync();

            var pk = new LivroAssuntoPk
            {
                LivroCodl = updatedLivroAssunto.LivroCodl,
                AssuntoCodAs = updatedLivroAssunto.AssuntoCodAs
            };


            // Assert
            var result = await _livroAssuntoRepository.GetById(pk);
            Assert.NotNull(result);
            Assert.Equal(updatedLivroAssunto.LivroCodl, result.LivroCodl);
            Assert.Equal(updatedLivroAssunto.AssuntoCodAs, result.AssuntoCodAs);
        }

        [Fact(DisplayName = "Remove livroassunto no repositório")]
        public async Task DeleteLivroAssunto_ShouldRemoveLivroAssunto()
        {
            // Arrange
            var faker = new Faker<LivroAssunto>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AssuntoCodAs, f => f.Random.Int());

            var newLivroAssunto = faker.Generate();
            await _livroAssuntoRepository.Add(newLivroAssunto);
            await _context.SaveChangesAsync();

            // Act
            _context.Entry(newLivroAssunto).State = EntityState.Detached; // Detach the existing entity
            await _livroAssuntoRepository.Delete(newLivroAssunto);
            await _context.SaveChangesAsync();

            var pk = new LivroAssuntoPk
            {
                LivroCodl = newLivroAssunto.LivroCodl,
                AssuntoCodAs = newLivroAssunto.AssuntoCodAs
            };

            // Assert
            var result = await _livroAssuntoRepository.GetById(pk);
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
