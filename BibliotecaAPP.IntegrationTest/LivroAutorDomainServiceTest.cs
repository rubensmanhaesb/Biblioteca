using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Services;
using BibliotecaApp.Infra.Data.Context;
using BibliotecaApp.Infra.Data.Repositories;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BibliotecaAPP.IntegrationTest
{
    public class LivroAutorDomainServiceTest
    {
        private readonly Mock<IValidator<LivroAutor>> _validatorMock;
        private readonly LivroAutorDomainService _livroAutorDomainService;
        private readonly DataContext _dataContext;

        public LivroAutorDomainServiceTest()
        {
            _validatorMock = new Mock<IValidator<LivroAutor>>();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _dataContext = new DataContext(options);

            _livroAutorDomainService = new LivroAutorDomainService(
                 new UnitOfWork(_dataContext),
                _validatorMock.Object);
        }

        private LivroAutor GenerateValidLivroAutor()
        {
            return new Faker<LivroAutor>("pt_BR")
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AutorCodAu, f => f.Random.Int())
                .Generate();
        }

        [Fact(DisplayName = "Adicionar LivroAutor com sucesso")]
        public async Task AddAsync_ShouldAddLivroAutor_WhenValid()
        {
            var newLivroAutor = GenerateValidLivroAutor();

            _validatorMock.Setup(v => v.ValidateAsync(newLivroAutor, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var result = await _livroAutorDomainService.AddAsync(newLivroAutor);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(newLivroAutor);
        }

        [Fact(DisplayName = "Adicionar LivroAutor deve falhar na validação")]
        public async Task AddAsync_ShouldThrowValidationException_WhenInvalid()
        {
            var newLivroAutor = new LivroAutor { LivroCodl = new Random().Next() };
            var validationErrors = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("AutorCodAu", "O código do autor é obrigatório.")
            };

            _validatorMock.Setup(v => v.ValidateAsync(newLivroAutor, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

            Func<Task> act = async () => await _livroAutorDomainService.AddAsync(newLivroAutor);

            var exception = await act.Should().ThrowAsync<FluentValidation.ValidationException>();
            exception.Which.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("O código do autor é obrigatório.");
        }

        [Fact(DisplayName = "Atualizar LivroAutor com sucesso")]
        public async Task UpdateAsync_ShouldUpdateLivroAutor_WhenValid()
        {
            var newLivroAutor = GenerateValidLivroAutor();

            _validatorMock.Setup(v => v.ValidateAsync(newLivroAutor, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var resultInclusao = await _livroAutorDomainService.AddAsync(newLivroAutor);
            _dataContext.ChangeTracker.Clear();

            var updatedLivroAutor = new LivroAutor
            {
                LivroCodl = resultInclusao.LivroCodl,
                AutorCodAu = resultInclusao.AutorCodAu,
                Livro = resultInclusao.Livro,
                Autor = resultInclusao.Autor
            };

            _validatorMock.Setup(v => v.ValidateAsync(updatedLivroAutor, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var result = await _livroAutorDomainService.UpdateAsync(updatedLivroAutor);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(updatedLivroAutor);
        }

        [Fact(DisplayName = "Atualizar LivroAutor deve falhar na validação")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenInvalid()
        {
            var livroAutor = new LivroAutor { AutorCodAu = new Random().Next() };
            var validationErrors = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("LivroCodl", "O código do livro é obrigatório.")
            };

            _validatorMock.Setup(v => v.ValidateAsync(livroAutor, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

            Func<Task> act = async () => await _livroAutorDomainService.UpdateAsync(livroAutor);

            var exception = await act.Should().ThrowAsync<FluentValidation.ValidationException>();
            exception.Which.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("O código do livro é obrigatório.");
        }

        [Fact(DisplayName = "Excluir LivroAutor deve falhar quando LivroAutor não encontrado")]
        public async Task DeleteAsync_ShouldThrowLivroAutorNotFoundException_WhenLivroAutorNotFound()
        {
            var newLivroAutor = GenerateValidLivroAutor();

            Func<Task> act = async () => await _livroAutorDomainService.DeleteAsync(newLivroAutor);

            var exception = await act.Should().ThrowAsync<NotFoundExceptionLivroAutor>()
                .WithMessage($"Livro Autor {newLivroAutor.LivroCodl} e {newLivroAutor.AutorCodAu} não encontrado.");
        }

        [Fact(DisplayName = "Excluir LivroAutor com sucesso")]
        public async Task DeleteAsync_ShouldDeleteLivroAutor_WhenLivroAutorExists()
        {
            var newLivroAutor = GenerateValidLivroAutor();

            _validatorMock.Setup(v => v.ValidateAsync(newLivroAutor, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var resultInclusao = await _livroAutorDomainService.AddAsync(newLivroAutor);

            var result = await _livroAutorDomainService.DeleteAsync(resultInclusao);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(newLivroAutor);
        }

        [Fact(DisplayName = "Consultar LivroAutor por ID deve retornar LivroAutor existente")]
        public async Task GetByIdAsync_ShouldReturnLivroAutor_WhenLivroAutorExists()
        {
            var newLivroAutor = GenerateValidLivroAutor();

            _validatorMock.Setup(v => v.ValidateAsync(newLivroAutor, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var resultInclusao = await _livroAutorDomainService.AddAsync(newLivroAutor);

            var result = await _livroAutorDomainService.GetByIdAsync(newLivroAutor.Pk);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(newLivroAutor);
        }

        [Fact(DisplayName = "Consultar LivroAutor por ID deve retornar null quando LivroAutor não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenLivroAutorNotFound()
        {
            var newLivroAutor = new LivroAutor { LivroCodl = new Random().Next(), AutorCodAu = new Random().Next() };

            var result = await _livroAutorDomainService.GetByIdAsync(newLivroAutor.Pk);

            result.Should().BeNull();
        }

        [Fact(DisplayName = "Consultar todos os LivroAutors")]
        public async Task GetManyAsync_ShouldReturnLivroAutors_WhenLivroAutorsExist()
        {
            await AddAsync_ShouldAddLivroAutor_WhenValid();

            var result = await _livroAutorDomainService.GetAllAsync();

            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThan(0);
        }
    }
}
