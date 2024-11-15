using BibliotecaApp.Infra.Data.Repositories;
using BibliotecaApp.Domain.Entities;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using BibliotecaApp.Infra.Data.Context;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace BibliotecaApp.Infra.Data.Test
{
    public class LivroRepositoryTest : IDisposable
    {
        private readonly DataContext _context;
        private readonly LivroRepository _livroRepository;
        private readonly ILogger<DataContext> _logger;

        public LivroRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<DataContext>();
            _context = new DataContext(options, _logger);

            _livroRepository = new LivroRepository(_context);
        }

        [Fact(DisplayName = "Adiciona livro no repositório")]
        public async Task CreateLivro_ShouldAddNewLivro()
        {
            // Arrange
            var faker = new Faker<Livro>()
                .RuleFor(l => l.Codl, f => f.Random.Int())
                .RuleFor(l => l.Titulo, f => f.Lorem.Sentence())
                .RuleFor(l => l.Editora, f => f.Company.CompanyName())
                .RuleFor(l => l.Edicao, f => f.Random.Int(1, 10))
                .RuleFor(l => l.AnoPublicacao, f => f.Date.Past(20).Year.ToString())
                .RuleFor(l => l.Autores, f => new List<LivroAutor>
                {
                    new LivroAutor
                    {
                        Autor = new Autor
                        {
                            CodAu = f.Random.Int(),
                            Nome = f.Person.FullName
                        }
                    }
                })
                .RuleFor(l => l.Assuntos, f => new List<LivroAssunto>
                {
                    new LivroAssunto
                    {
                        Assunto = new Assunto
                        {
                            CodAs = f.Random.Int(),
                            Descricao = f.Lorem.Word()
                        }
                    }
                });

            var newLivro = faker.Generate();

            // Act
            await _livroRepository.Add(newLivro);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _livroRepository.GetById(newLivro.Codl);
            Assert.NotNull(result);
            Assert.Equal(newLivro.Codl, result.Codl);
            Assert.Equal(newLivro.Titulo, result.Titulo);
            Assert.Equal(newLivro.Editora, result.Editora);
            Assert.Equal(newLivro.Edicao, result.Edicao);
            Assert.Equal(newLivro.AnoPublicacao, result.AnoPublicacao);
            Assert.Equal(newLivro.Autores.Count, result.Autores.Count);
            Assert.Equal(newLivro.Assuntos.Count, result.Assuntos.Count);
        }

        [Fact(DisplayName = "Obtem um único livro com base no ID")]
        public async Task GetLivroById_ShouldReturnLivro_WhenLivroExists()
        {
            // Arrange
            var faker = new Faker<Livro>()
                .RuleFor(l => l.Codl, f => f.Random.Int())
                .RuleFor(l => l.Titulo, f => f.Lorem.Sentence())
                .RuleFor(l => l.Editora, f => f.Company.CompanyName())
                .RuleFor(l => l.Edicao, f => f.Random.Int(1, 10))
                .RuleFor(l => l.AnoPublicacao, f => f.Date.Past(20).Year.ToString())
                .RuleFor(l => l.Autores, f => new List<LivroAutor>
                {
                    new LivroAutor
                    {
                        Autor = new Autor
                        {
                            CodAu = f.Random.Int(),
                            Nome = f.Person.FullName
                        }
                    }
                })
                .RuleFor(l => l.Assuntos, f => new List<LivroAssunto>
                {
                    new LivroAssunto
                    {
                        Assunto = new Assunto
                        {
                            CodAs = f.Random.Int(),
                            Descricao = f.Lorem.Word()
                        }
                    }
                });

            var newLivro = faker.Generate();
            await _livroRepository.Add(newLivro);
            await _context.SaveChangesAsync();

            // Act
            var result = await _livroRepository.GetById(newLivro.Codl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newLivro.Codl, result.Codl);
            Assert.Equal(newLivro.Titulo, result.Titulo);
            Assert.Equal(newLivro.Editora, result.Editora);
            Assert.Equal(newLivro.Edicao, result.Edicao);
            Assert.Equal(newLivro.AnoPublicacao, result.AnoPublicacao);
            Assert.Equal(newLivro.Autores.Count, result.Autores.Count);
            Assert.Equal(newLivro.Assuntos.Count, result.Assuntos.Count);
        }

        [Fact(DisplayName = "Atualiza o livro no repositório")]
        public async Task UpdateLivro_ShouldModifyExistingLivro()
        {
            // Arrange
            var faker = new Faker<Livro>()
                .RuleFor(l => l.Codl, f => f.Random.Int())
                .RuleFor(l => l.Titulo, f => f.Lorem.Sentence())
                .RuleFor(l => l.Editora, f => f.Company.CompanyName())
                .RuleFor(l => l.Edicao, f => f.Random.Int(1, 10))
                .RuleFor(l => l.AnoPublicacao, f => f.Date.Past(20).Year.ToString())
                .RuleFor(l => l.Autores, f => new List<LivroAutor>
                {
            new LivroAutor
            {
                Autor = new Autor
                {
                    CodAu = f.Random.Int(),
                    Nome = f.Person.FullName
                }
            }
                })
                .RuleFor(l => l.Assuntos, f => new List<LivroAssunto>
                {
            new LivroAssunto
            {
                Assunto = new Assunto
                {
                    CodAs = f.Random.Int(),
                    Descricao = f.Lorem.Word()
                }
            }
                });

            var newLivro = faker.Generate();
            await _livroRepository.Add(newLivro);
            await _context.SaveChangesAsync();

            // Detach the existing entity
            _context.Entry(newLivro).State = EntityState.Detached;

            var updatedLivro = new Livro
            {
                Codl = newLivro.Codl,
                Titulo = "Updated Title",
                Editora = "Updated Editora",
                Edicao = newLivro.Edicao + 1,
                AnoPublicacao = (int.Parse(newLivro.AnoPublicacao!) + 1).ToString()
            };

            // Act
            await _livroRepository.Update(updatedLivro);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _livroRepository.GetById(updatedLivro.Codl);
            Assert.NotNull(result);
            Assert.Equal(updatedLivro.Codl, result.Codl);
            Assert.Equal(updatedLivro.Titulo, result.Titulo);
            Assert.Equal(updatedLivro.Editora, result.Editora);
            Assert.Equal(updatedLivro.Edicao, result.Edicao);
            Assert.Equal(updatedLivro.AnoPublicacao, result.AnoPublicacao);
            Assert.Equal(updatedLivro.Autores.Count, result.Autores.Count);
            Assert.Equal(updatedLivro.Assuntos.Count, result.Assuntos.Count);
        }

        [Fact(DisplayName = "Remove o livro no repositório")]
        public async Task DeleteLivro_ShouldRemoveLivro()
        {
            // Arrange
            var faker = new Faker<Livro>()
                .RuleFor(l => l.Codl, f => f.Random.Int())
                .RuleFor(l => l.Titulo, f => f.Lorem.Sentence())
                .RuleFor(l => l.Editora, f => f.Company.CompanyName())
                .RuleFor(l => l.Edicao, f => f.Random.Int(1, 10))
                .RuleFor(l => l.AnoPublicacao, f => f.Date.Past(20).Year.ToString())
                .RuleFor(l => l.Autores, f => new List<LivroAutor>
                {
                    new LivroAutor
                    {
                        Autor = new Autor
                        {
                            CodAu = f.Random.Int(),
                            Nome = f.Person.FullName
                        }
                    }
                })
                .RuleFor(l => l.Assuntos, f => new List<LivroAssunto>
                {
                    new LivroAssunto
                    {
                        Assunto = new Assunto
                        {
                            CodAs = f.Random.Int(),
                            Descricao = f.Lorem.Word()
                        }
                    }
                });

            var newLivro = faker.Generate();
            await _livroRepository.Add(newLivro);
            await _context.SaveChangesAsync();

            // Act
            _context.Entry(newLivro).State = EntityState.Detached; // Detach the existing entity
            await _livroRepository.Delete(newLivro);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _livroRepository.GetById(newLivro.Codl);
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
