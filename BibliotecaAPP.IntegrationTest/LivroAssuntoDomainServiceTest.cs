using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Services;
using BibliotecaApp.Infra.Data.Context;
using BibliotecaApp.Infra.Data.Repositories;
using BibliotecaAPP.IntegrationTest.Helpers;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BibliotecaAPP.IntegrationTest
{
    public class LivroAssuntoDomainServiceTest
    {
        private readonly Mock<IValidator<LivroAssunto>> _validatorMock;
        private readonly LivroAssuntoDomainService _livroAssuntoDomainService;
        private readonly UnitOfWork _unitOfWork;
        private readonly DataContext _dataContext;

        public LivroAssuntoDomainServiceTest()
        {
            _validatorMock = new Mock<IValidator<LivroAssunto>>();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;
            
            _dataContext = new DataContext(options, new LoggerFactory().CreateLogger<DataContext>());
            _unitOfWork = new UnitOfWork(_dataContext);

            _livroAssuntoDomainService = new LivroAssuntoDomainService(_unitOfWork, _validatorMock.Object);
                
        }

        private LivroAssunto GenerateValidLivroAssunto()
        {
            return LivroAssuntoTestHelper.GenerateValidLivroAssunto(_unitOfWork).Result;
        }

        [Fact(DisplayName = "Adicionar LivroAssunto com sucesso")]
        public async Task AddAsync_ShouldAddLivroAssunto_WhenValid()
        {
            var newLivroAssunto = GenerateValidLivroAssunto();

            _validatorMock.Setup(v => v.ValidateAsync(newLivroAssunto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var result = await _livroAssuntoDomainService.AddAsync(newLivroAssunto);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(newLivroAssunto);
        }

        [Fact(DisplayName = "Adicionar LivroAssunto deve falhar na validação")]
        public async Task AddAsync_ShouldThrowValidationException_WhenInvalid()
        {
            var newLivroAssunto = GenerateValidLivroAssunto();

            newLivroAssunto.AssuntoCodAs = new Random().Next();

            Func<Task> act = async () => await _livroAssuntoDomainService.AddAsync(newLivroAssunto);


            await act.Should().ThrowAsync<NotFoundExceptionAssunto>()
                .WithMessage($"Assunto {newLivroAssunto.AssuntoCodAs} não encontrado.");
        }

        [Fact(DisplayName = "Atualizar LivroAssunto com sucesso")]
        public async Task UpdateAsync_ShouldUpdateLivroAssunto_WhenValid()
        {
            var newLivroAssunto = GenerateValidLivroAssunto();

            _validatorMock.Setup(v => v.ValidateAsync(newLivroAssunto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var resultInclusao = await _livroAssuntoDomainService.AddAsync(newLivroAssunto);
            _unitOfWork.DataContext.Entry(newLivroAssunto).State = EntityState.Detached;
            //_dataContext.ChangeTracker.Clear();

            var updatedLivroAssunto = new LivroAssunto
            {
                LivroCodl = resultInclusao.LivroCodl,
                AssuntoCodAs = resultInclusao.AssuntoCodAs,
                Livro = resultInclusao.Livro
            };

            // _unitOfWork.DataContext.Entry(updatedLivroAssunto).State = EntityState.Detached;
            _unitOfWork.DataContext.Entry(newLivroAssunto).State = EntityState.Detached;
            _validatorMock.Setup(v => v.ValidateAsync(updatedLivroAssunto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var result = await _livroAssuntoDomainService.UpdateAsync(updatedLivroAssunto);



            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(updatedLivroAssunto);
        }

        [Fact(DisplayName = "Atualizar LivroAssunto deve falhar na validação")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenInvalid()
        {
            var livroAssunto = new LivroAssunto { AssuntoCodAs = new Random().Next() };
            var validationErrors = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("LivroCodl", "O código do livro é obrigatório.")
            };

            _validatorMock.Setup(v => v.ValidateAsync(livroAssunto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

            Func<Task> act = async () => await _livroAssuntoDomainService.UpdateAsync(livroAssunto);

            var exception = await act.Should().ThrowAsync<FluentValidation.ValidationException>();
            exception.Which.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("O código do livro é obrigatório.");
        }

        [Fact(DisplayName = "Excluir LivroAssunto deve falhar quando LivroAssunto não encontrado")]
        public async Task DeleteAsync_ShouldThrowLivroAssuntoNotFoundException_WhenLivroAssuntoNotFound()
        {
            var newLivroAssunto = GenerateValidLivroAssunto();

            Func<Task> act = async () => await _livroAssuntoDomainService.DeleteAsync(newLivroAssunto);

            var exception = await act.Should().ThrowAsync<NotFoundExceptionLivroAssunto>()
                .WithMessage($"Livro Assunto {newLivroAssunto.LivroCodl} e {newLivroAssunto.AssuntoCodAs} não encontrado.");
        }

        [Fact(DisplayName = "Excluir LivroAssunto com sucesso")]
        public async Task DeleteAsync_ShouldDeleteLivroAssunto_WhenLivroAssuntoExists()
        {
            var newLivroAssunto = GenerateValidLivroAssunto();

            _validatorMock.Setup(v => v.ValidateAsync(newLivroAssunto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var resultInclusao = await _livroAssuntoDomainService.AddAsync(newLivroAssunto);

            var result = await _livroAssuntoDomainService.DeleteAsync(resultInclusao);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(newLivroAssunto);
        }

        [Fact(DisplayName = "Consultar LivroAssunto por ID deve retornar LivroAssunto existente")]
        public async Task GetByIdAsync_ShouldReturnLivroAssunto_WhenLivroAssuntoExists()
        {
            var newLivroAssunto = GenerateValidLivroAssunto();

            _validatorMock.Setup(v => v.ValidateAsync(newLivroAssunto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var resultInclusao = await _livroAssuntoDomainService.AddAsync(newLivroAssunto);

            var result = await _livroAssuntoDomainService.GetByIdAsync(newLivroAssunto.Pk);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(newLivroAssunto);
        }

        [Fact(DisplayName = "Consultar LivroAssunto por ID deve retornar null quando LivroAssunto não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenLivroAssuntoNotFound()
        {
            var newLivroAssunto = new LivroAssunto { LivroCodl = new Random().Next(), AssuntoCodAs = new Random().Next() };

            var result = await _livroAssuntoDomainService.GetByIdAsync(newLivroAssunto.Pk);

            result.Should().BeNull();
        }

        [Fact(DisplayName = "Consultar todos os LivroAssuntos")]
        public async Task GetManyAsync_ShouldReturnLivroAssuntos_WhenLivroAssuntosExist()
        {
            await AddAsync_ShouldAddLivroAssunto_WhenValid();

            var result = await _livroAssuntoDomainService.GetAllAsync();

            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThan(0);
        }
    }
}
