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
using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Domain.Interfaces.Services;

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
                //.RuleFor(l => l.Codl, f => f.Random.Int(1, 10000)) // é identity não deve ser gerado
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

            var result = await _livroDomainService.AddAsync(newLivro);

            result.Should().NotBeNull();
            result.Titulo.Should().Be(newLivro.Titulo);
            result.Editora.Should().Be(newLivro.Editora);
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando informado o codL")]
        public async Task AddAsync_ShouldThrowValidationException_WhenVodeAuIsNotEmpty()
        {
            var newAutor = GenerateValidLivro();
            newAutor.Codl = 1; // Campo obrigatório estar vazio

            Func<Task> act = async () => await _livroDomainService.AddAsync(newAutor);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("Código do livro não deve ser informado na inclusão."));
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando título estiver vazio")]
        public async Task AddAsync_ShouldThrowValidationException_WhenTituloIsEmpty()
        {
            var newLivro = GenerateValidLivro();
            newLivro.Titulo = ""; // Campo obrigatório

            Func<Task> act = async () => await _livroDomainService.AddAsync(newLivro);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("O título do livro é obrigatório."));
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando editora estiver vazia")]
        public async Task AddAsync_ShouldThrowValidationException_WhenEditoraIsEmpty()
        {
            var newLivro = GenerateValidLivro();
            newLivro.Editora = ""; // Campo obrigatório

            Func<Task> act = async () => await _livroDomainService.AddAsync(newLivro);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("A Editora do livro é obrigatória."));
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando edição for zero ou negativa")]
        public async Task AddAsync_ShouldThrowValidationException_WhenEdicaoIsInvalid()
        {
            var newLivro = GenerateValidLivro();
            newLivro.Edicao = 0; // Valor inválido

            Func<Task> act = async () => await _livroDomainService.AddAsync(newLivro);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("A Edição não pode ser zero ou negativa."));
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando ano de publicação estiver vazio")]
        public async Task AddAsync_ShouldThrowValidationException_WhenAnoPublicacaoIsEmpty()
        {
            var newLivro = GenerateValidLivro();
            newLivro.AnoPublicacao = ""; // Campo obrigatório

            Func<Task> act = async () => await _livroDomainService.AddAsync(newLivro);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("A Publicação do livro é obrigatória."));
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando ano de publicação tiver tamanho incorreto")]
        public async Task AddAsync_ShouldThrowValidationException_WhenAnoPublicacaoIsIncorrectLength()
        {
            var newLivro = GenerateValidLivro();
            newLivro.AnoPublicacao = "123"; // Tamanho inválido

            Func<Task> act = async () => await _livroDomainService.AddAsync(newLivro);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("A Publicação deve ter exatamente 4 caracteres."));
        }

        [Fact(DisplayName = "Atualizar livro com sucesso")]
        public async Task UpdateAsync_ShouldUpdateLivro_WhenValid()
        {

            var livro = GenerateValidLivro();

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
        [Fact(DisplayName = "Atualizar livro deve falhar quando livro não encontrado")]
        public async Task UpdateAsync_ShouldThrowLivroNotFoundException_WhenLivroNotFound()
        {

            var livro = GenerateValidLivro();
            livro.Codl = new Random().Next(1, 10000);

            Func<Task> act = async () => await _livroDomainService.UpdateAsync(livro);


            await act.Should().ThrowAsync<NotFoundExceptionLivro>()
                .WithMessage($"Livro {livro.Codl} não encontrado.");
        }



        [Fact(DisplayName = "Atualizar Livro deve falhar quando título estiver vazio")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenTituloIsEmpty()
        {
            var livro = GenerateValidLivro();
            var addedLivro = await _livroDomainService.AddAsync(livro);

            addedLivro.Titulo = ""; // Campo obrigatório inválido

            Func<Task> act = async () => await _livroDomainService.UpdateAsync(addedLivro);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("O título do livro é obrigatório."));
        }

        [Fact(DisplayName = "Atualizar Livro deve falhar quando editora estiver vazia")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenEditoraIsEmpty()
        {
            var livro = GenerateValidLivro();
            var addedLivro = await _livroDomainService.AddAsync(livro);

            addedLivro.Editora = ""; // Campo obrigatório inválido

            Func<Task> act = async () => await _livroDomainService.UpdateAsync(addedLivro);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("A Editora do livro é obrigatória."));
        }

        [Fact(DisplayName = "Atualizar Livro deve falhar quando edição for zero ou negativa")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenEdicaoIsInvalid()
        {
            var livro = GenerateValidLivro();
            var addedLivro = await _livroDomainService.AddAsync(livro);

            addedLivro.Edicao = 0; // Valor inválido

            Func<Task> act = async () => await _livroDomainService.UpdateAsync(addedLivro);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("A Edição não pode ser zero ou negativa."));
        }

        [Fact(DisplayName = "Atualizar Livro deve falhar quando ano de publicação estiver vazio")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenAnoPublicacaoIsEmpty()
        {
            var livro = GenerateValidLivro();
            var addedLivro = await _livroDomainService.AddAsync(livro);

            addedLivro.AnoPublicacao = ""; // Campo obrigatório inválido

            Func<Task> act = async () => await _livroDomainService.UpdateAsync(addedLivro);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("A Publicação do livro é obrigatória."));
        }

        [Fact(DisplayName = "Atualizar Livro deve falhar quando ano de publicação tiver tamanho incorreto")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenAnoPublicacaoIsIncorrectLength()
        {
            var livro = GenerateValidLivro();
            var addedLivro = await _livroDomainService.AddAsync(livro);

            addedLivro.AnoPublicacao = "123"; // Tamanho inválido

            Func<Task> act = async () => await _livroDomainService.UpdateAsync(addedLivro);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("A Publicação deve ter exatamente 4 caracteres."));
        }

        [Fact(DisplayName = "Excluir livro deve falhar quando livro não encontrado")]
        public async Task DeleteAsync_ShouldThrowLivroNotFoundException_WhenLivroNotFound()
        {

            var livro = GenerateValidLivro();


            Func<Task> act = async () => await _livroDomainService.DeleteAsync(livro);


            await act.Should().ThrowAsync<NotFoundExceptionLivro>()
                .WithMessage($"Livro {livro.Codl} não encontrado.");
        }

        [Fact(DisplayName = "Exclui de Livro com sucesso")]
        public async Task DeleteAsync_ShouldDeletePrecoLivro_WhenPrecoLivroExists()
        {
            // Arrange: Adiciona um registro válido
            var livro = GenerateValidLivro();
            await _livroDomainService.AddAsync(livro);

            // Act: Exclui o registro recém-adicionado
            var result = await _livroDomainService.DeleteAsync(livro);

            // Assert: Verifica se o registro foi excluído corretamente
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(livro);
        }

        [Fact(DisplayName = "Consultar Livro por ID deve retornar livro existente")]
        public async Task GetByIdAsync_ShouldReturnLivro_WhenLivroExists()
        {
            var livro = GenerateValidLivro();

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
            var livro1 = GenerateValidLivro();

            var addedLivro = await _livroDomainService.AddAsync(livro1);


            await AddAsync_ShouldAddLivro_WhenValid(); 


            var result = await _livroDomainService.GetAllAsync();


            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThanOrEqualTo(1);
        }
    }
}
