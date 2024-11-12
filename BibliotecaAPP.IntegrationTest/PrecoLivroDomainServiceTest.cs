using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
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
                .RuleFor(p => p.Codp, f => f.Random.Int(min: 1))
                .RuleFor(p => p.LivroCodl, f => f.Random.Int(min: 1))
                .RuleFor(p => p.Valor, f => f.Finance.Amount(1)) // Valor sempre maior que zero
                .RuleFor(p => p.TipoCompra, f => f.PickRandom<TipoCompra>())
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

        [Fact(DisplayName = "Adicionar Preço de Livro deve falhar na validação de campos obrigatórios")]
        public async Task AddAsync_ShouldThrowValidationException_WhenFieldsAreMissing()
        {
            var precoLivro = new PrecoLivro
            {
                Valor = 10.0m // Campo LivroCodl ausente
            };
            var validationErrors = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("LivroCodl", "O código do livro é obrigatório.")
            };

            _validatorMock.Setup(v => v.ValidateAsync(precoLivro, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

            Func<Task> act = async () => await _precoLivroDomainService.AddAsync(precoLivro);

            var exception = await act.Should().ThrowAsync<FluentValidation.ValidationException>();
            exception.Which.Errors.Should().HaveCount(2);
            exception.Which.Errors.Select(e => e.ErrorMessage).Should().BeEquivalentTo(
                new[]
                {
                    "O código do livro é obrigatório.",
                    "O código do livro deve ser maior que zero."
                }
            );
        }

        [Fact(DisplayName = "Adicionar Preço de Livro deve falhar na validação de valor zero")]
        public async Task AddAsync_ShouldThrowValidationException_WhenValorIsZeroOrNegative()
        {
            var precoLivro = new PrecoLivro
            {
                LivroCodl = 1,
                Valor = 0, // Valor inválido
                TipoCompra = TipoCompra.Balcao
            };
            var validationErrors = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Valor", "O valor deve ser maior que zero.")
            };

            _validatorMock.Setup(v => v.ValidateAsync(precoLivro, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

            Func<Task> act = async () => await _precoLivroDomainService.AddAsync(precoLivro);

            var exception = await act.Should().ThrowAsync<FluentValidation.ValidationException>();
            exception.Which.Errors.Should().HaveCount(2);
            exception.Which.Errors.Select(e => e.ErrorMessage).Should().BeEquivalentTo(
                new[]
                {
                    "O valor do livro é obrigatório.",
                    "O valor deve ser maior que zero."
                }
            );
        }


        [Fact(DisplayName = "Atualizar Preço de Livro deve falhar quando tipo de compra é inválido")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenTipoCompraIsInvalid()
        {
            var precoLivro = GenerateValidPrecoLivro();
            precoLivro.TipoCompra = (TipoCompra)999; // Tipo de compra inválido

            var validationErrors = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("TipoCompra", "O tipo de compra é inválido.")
            };

            _validatorMock.Setup(v => v.ValidateAsync(precoLivro, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

            Func<Task> act = async () => await _precoLivroDomainService.UpdateAsync(precoLivro);

            var exception = await act.Should().ThrowAsync<FluentValidation.ValidationException>();
            exception.Which.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("O tipo de compra é inválido.");
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
        
        [Fact(DisplayName = "Atualizar Preço de Livro deve lançar exceção quando não encontrado")]
        public async Task UpdateAsync_ShouldThrowNotFoundExceptionPrecoLivro_WhenPrecoLivroNotFound()
        {
            // Arrange: Cria um registro com um ID inexistente
            var precoLivro = new PrecoLivro { Codp = 9999, LivroCodl = 1, Valor = 10, TipoCompra = TipoCompra.Balcao }; // ID que não existe no banco

            // Act: Tenta atualizar o registro inexistente
            Func<Task> act = async () => await _precoLivroDomainService.UpdateAsync(precoLivro);

            // Assert: Verifica se a exceção NotFoundExceptionPrecoLivro foi lançada
            await act.Should().ThrowAsync<NotFoundExceptionPrecoLivro>()
                .WithMessage($"Preço Livro {precoLivro.Codp} não encontrado.");
        }

        [Fact(DisplayName = "Excluir Preço de Livro com sucesso")]
        public async Task DeleteAsync_ShouldDeletePrecoLivro_WhenPrecoLivroExists()
        {
            // Arrange: Adiciona um registro válido
            var precoLivro = GenerateValidPrecoLivro();
            await _precoLivroDomainService.AddAsync(precoLivro);

            // Act: Exclui o registro recém-adicionado
            var result = await _precoLivroDomainService.DeleteAsync(precoLivro);

            // Assert: Verifica se o registro foi excluído corretamente
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(precoLivro);
        }

        [Fact(DisplayName = "Excluir Preço de Livro deve lançar exceção quando não encontrado")]
        public async Task DeleteAsync_ShouldThrowNotFoundExceptionPrecoLivro_WhenPrecoLivroNotFound()
        {
            // Arrange: Cria um registro com um ID inexistente
            var precoLivro = new PrecoLivro { Codp = 9999 }; // ID que não existe no banco

            // Act: Tenta excluir o registro inexistente
            Func<Task> act = async () => await _precoLivroDomainService.DeleteAsync(precoLivro);

            // Assert: Verifica se a exceção NotFoundExceptionPrecoLivro foi lançada
            await act.Should().ThrowAsync<NotFoundExceptionPrecoLivro>()
                .WithMessage($"Preço Livro {precoLivro.Codp} não encontrado.");
        }

    }
}
