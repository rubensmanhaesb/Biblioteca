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

namespace BibliotecaAPP.IntegrationTest
{
    public class LivroDomainServiceTest
    {
        private readonly LivroDomainService _livroDomainService;
        private readonly ILivroRepository _livroRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DataContext _context;

        public LivroDomainServiceTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _context = new DataContext(options);
            _livroRepository = new LivroRepository(_context);
            _unitOfWork = new UnitOfWork(_context);
            _livroDomainService = new LivroDomainService(_unitOfWork);
        }

        private Livro GenerateValidLivro()
        {
            return new Faker<Livro>("pt_BR")
                .RuleFor(l => l.Codl, f => f.Random.Int(1, 10000))
                .RuleFor(l => l.Titulo, f => f.Company.CompanyName())
                .RuleFor(l => l.Editora, f => f.Company.CompanyName())
                .RuleFor(l => l.Edicao, f => f.Random.Int(1, 10))
                .RuleFor(l => l.AnoPublicacao, f => f.Date.Past(20).Year.ToString())
                .Generate();
        }

        [Fact(DisplayName = "Adicionar Livro com sucesso")]
        public async Task AddAsync_ShouldAddLivro_WhenValid()
        {

            var newLivro = GenerateValidLivro();
            newLivro.Codl = 0; 

            var result = await _livroDomainService.AddAsync(newLivro);

            result.Should().NotBeNull();
            result.Titulo.Should().Be(newLivro.Titulo);
            result.Editora.Should().Be(newLivro.Editora);
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar na validação")]
        public async Task AddAsync_ShouldThrowValidationException_WhenInvalid()
        {

            var livro = new Livro { Titulo = "" }; 


            Func<Task> act = async () => await _livroDomainService.AddAsync(livro);


            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("O título do livro é obrigatório."));
        }

        [Fact(DisplayName = "Atualizar livro com sucesso")]
        public async Task UpdateAsync_ShouldUpdateLivro_WhenValid()
        {

            var livro = GenerateValidLivro();
            livro.Codl = 0; 
            var addedLivro = await _livroDomainService.AddAsync(livro);


            addedLivro.Titulo = "Titulo Alterado";
            addedLivro.Edicao = new Random().Next(1, 10);
            addedLivro.AnoPublicacao = DateTime.Now.Year.ToString();
            addedLivro.Editora = "Editora Alterada";


            var result = await _livroDomainService.UpdateAsync(addedLivro);


            result.Should().NotBeNull();
            result.Titulo.Should().Be("Titulo Alterado");
            result.Editora.Should().Be("Editora Alterada");
        }

        [Fact(DisplayName = "Atualizar livro deve falhar na validação")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenInvalid()
        {

            var livro = GenerateValidLivro();
            livro.Codl = 0;
            await _livroDomainService.AddAsync(livro);
            livro.Titulo = ""; 


            Func<Task> act = async () => await _livroDomainService.UpdateAsync(livro);


            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("O título do livro é obrigatório."));
        }

        [Fact(DisplayName = "Excluir livro deve falhar quando livro não encontrado")]
        public async Task DeleteAsync_ShouldThrowLivroNotFoundException_WhenLivroNotFound()
        {

            var livro = GenerateValidLivro();


            Func<Task> act = async () => await _livroDomainService.DeleteAsync(livro);


            await act.Should().ThrowAsync<NotFoundExceptionLivro>()
                .WithMessage($"Livro {livro.Codl} não encontrado.");
        }

        [Fact(DisplayName = "Excluir livro com sucesso")]
        public async Task DeleteAsync_ShouldDeleteLivro_WhenLivroExists()
        {

            var livro = GenerateValidLivro();
            livro.Codl = 0; 
            var addedLivro = await _livroDomainService.AddAsync(livro);


            var result = await _livroDomainService.DeleteAsync(addedLivro);


            result.Should().NotBeNull();
            result.Codl.Should().Be(addedLivro.Codl);
        }

        [Fact(DisplayName = "Consultar Livro por ID deve retornar livro existente")]
        public async Task GetByIdAsync_ShouldReturnLivro_WhenLivroExists()
        {

            var livro = GenerateValidLivro();
            livro.Codl = 0; 
            var addedLivro = await _livroDomainService.AddAsync(livro);


            var result = await _livroDomainService.GetByIdAsync(addedLivro.Codl);


            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(addedLivro);
        }

        [Fact(DisplayName = "Consultar livro por ID deve retornar null quando livro não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenLivroNotFound()
        {

            var livroId = new Random().Next(100, 10000);


            var result = await _livroDomainService.GetByIdAsync(livroId);


            result.Should().BeNull();
        }

        [Fact(DisplayName = "Consultar todos os livros")]
        public async Task GetManyAsync_ShouldReturnLivros_WhenLivrosExist()
        {

            await AddAsync_ShouldAddLivro_WhenValid(); 


            var result = await _livroDomainService.GetAllAsync();


            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThan(0);
        }
    }
}
