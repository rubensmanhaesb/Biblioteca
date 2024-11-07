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
    public class PrecoLivroDomainServiceTest
    {
        private readonly Mock<IValidator<PrecoLivro>> _validatorMock;
        private readonly PrecoLivroDomainService _precoLivroDomainService;

        public PrecoLivroDomainServiceTest()
        {
            _validatorMock = new Mock<IValidator<PrecoLivro>>();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            var unitOfWork = new UnitOfWork(new DataContext(options));
            _precoLivroDomainService = new PrecoLivroDomainService(unitOfWork);
        }

        private PrecoLivro GenerateValidPrecoLivro()
        {
            return new Faker<PrecoLivro>("pt_BR")
                .RuleFor(p => p.Codp, f => f.Random.Int())
                .RuleFor(p => p.LivroCodl, f => f.Random.Int())
                .RuleFor(p => p.Valor, f => f.Finance.Amount())
                .Generate();
        }

        [Fact(DisplayName = "Adicionar Preço de Livro com sucesso")]
        public async Task AddAsync_ShouldAddPrecoLivro_WhenValid()
        {
            var newPrecoLivro = GenerateValidPrecoLivro();

            _validatorMock.Setup(v => v.ValidateAsync(newPrecoLivro, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var result = await _precoLivroDomainService.AddAsync(newPrecoLivro);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(newPrecoLivro);
        }

        [Fact(DisplayName = "Adicionar Preço de Livro deve falhar na validação")]
        public async Task AddAsync_ShouldThrowValidationException_WhenInvalid()
        {
            var precoLivro = new PrecoLivro();
            var validationErrors = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Valor", "O valor é obrigatório.")
            };

            _validatorMock.Setup(v => v.ValidateAsync(precoLivro, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

            Func<Task> act = async () => await _precoLivroDomainService.AddAsync(precoLivro);

            var exception = await act.Should().ThrowAsync<FluentValidation.ValidationException>();
            exception.Which.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("O valor é obrigatório.");
        }

        [Fact(DisplayName = "Atualizar Preço de Livro com sucesso")]
        public async Task UpdateAsync_ShouldUpdatePrecoLivro_WhenValid()
        {
            var precoLivro = GenerateValidPrecoLivro();

            _validatorMock.Setup(v => v.ValidateAsync(precoLivro, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var resultInclusao = await _precoLivroDomainService.AddAsync(precoLivro);

            resultInclusao.Valor += 5; // Alterando valor para o teste

            var result = await _precoLivroDomainService.UpdateAsync(resultInclusao);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resultInclusao);
        }

        [Fact(DisplayName = "Atualizar Preço de Livro deve falhar na validação")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenInvalid()
        {
            var precoLivro = new PrecoLivro { Codp = 1, Valor = 0 };
            var validationErrors = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Valor", "O valor deve ser maior que zero.")
            };

            _validatorMock.Setup(v => v.ValidateAsync(precoLivro, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

            Func<Task> act = async () => await _precoLivroDomainService.UpdateAsync(precoLivro);

            var exception = await act.Should().ThrowAsync<FluentValidation.ValidationException>();
            exception.Which.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("O valor deve ser maior que zero.");
        }

        [Fact(DisplayName = "Excluir Preço de Livro deve falhar quando não encontrado")]
        public async Task DeleteAsync_ShouldThrowPrecoLivroNotFoundException_WhenPrecoLivroNotFound()
        {
            var precoLivro = GenerateValidPrecoLivro();

            Func<Task> act = async () => await _precoLivroDomainService.DeleteAsync(precoLivro);

            var exception = await act.Should().ThrowAsync<NotFoundExceptionPrecoLivro>()
                .WithMessage($"Preço Livro {precoLivro.Codp} não encontrado.");
        }

        [Fact(DisplayName = "Excluir Preço de Livro com sucesso")]
        public async Task DeleteAsync_ShouldDeletePrecoLivro_WhenPrecoLivroExists()
        {
            var precoLivro = GenerateValidPrecoLivro();

            _validatorMock.Setup(v => v.ValidateAsync(precoLivro, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var resultInclusao = await _precoLivroDomainService.AddAsync(precoLivro);

            var result = await _precoLivroDomainService.DeleteAsync(resultInclusao);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(precoLivro);
        }

        [Fact(DisplayName = "Consultar Preço de Livro por ID deve retornar preço existente")]
        public async Task GetByIdAsync_ShouldReturnPrecoLivro_WhenPrecoLivroExists()
        {
            var precoLivro = GenerateValidPrecoLivro();

            _validatorMock.Setup(v => v.ValidateAsync(precoLivro, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var resultInclusao = await _precoLivroDomainService.AddAsync(precoLivro);

            var result = await _precoLivroDomainService.GetByIdAsync(precoLivro.Codp);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(precoLivro);
        }

        [Fact(DisplayName = "Consultar Preço de Livro por ID deve retornar null quando não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenPrecoLivroNotFound()
        {
            var precoLivro = new PrecoLivro { Codp = new Random().Next(100, 10000) };

            var result = await _precoLivroDomainService.GetByIdAsync(precoLivro.Codp);

            result.Should().BeNull();
        }

        [Fact(DisplayName = "Consultar todos os preços de livros")]
        public async Task GetManyAsync_ShouldReturnPrecoLivros_WhenPrecoLivrosExist()
        {
            await AddAsync_ShouldAddPrecoLivro_WhenValid();

            var result = await _precoLivroDomainService.GetAllAsync();

            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThan(0);
        }
    }
}
