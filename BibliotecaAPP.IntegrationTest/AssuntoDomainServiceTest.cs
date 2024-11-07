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
    public class AssuntoDomainServiceTest
    {
        private readonly AssuntoDomainService _assuntoDomainService;
        private readonly IAssuntoRepository _assuntoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DataContext _context;

        public AssuntoDomainServiceTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _context = new DataContext(options);
            _assuntoRepository = new AssuntoRepository(_context);
            _unitOfWork = new UnitOfWork(_context);
            _assuntoDomainService = new AssuntoDomainService(_unitOfWork);
        }

        private Assunto GenerateValidAssunto()
        {
            return new Faker<Assunto>("pt_BR")
                .RuleFor(a => a.CodAs, f => f.Random.Int(1, 10000))
                .RuleFor(a => a.Descricao, f => f.Lorem.Letter(20))
                .Generate();
        }

        [Fact(DisplayName = "Adicionar Assunto com sucesso")]
        public async Task AddAsync_ShouldAddAssunto_WhenValid()
        {
            var newAssunto = GenerateValidAssunto();
            newAssunto.CodAs = 0;

            var result = await _assuntoDomainService.AddAsync(newAssunto);

            result.Should().NotBeNull();
            result.Descricao.Should().Be(newAssunto.Descricao);
        }

        [Fact(DisplayName = "Adicionar Assunto deve falhar na validação")]
        public async Task AddAsync_ShouldThrowValidationException_WhenInvalid()
        {
            var assunto = new Assunto { Descricao = "" };

            Func<Task> act = async () => await _assuntoDomainService.AddAsync(assunto);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("A descrição do assunto é obrigatória."));
        }

        [Fact(DisplayName = "Atualizar Assunto com sucesso")]
        public async Task UpdateAsync_ShouldUpdateAssunto_WhenValid()
        {
            var assunto = GenerateValidAssunto();
            assunto.CodAs = 0;
            var addedAssunto = await _assuntoDomainService.AddAsync(assunto);

            addedAssunto.Descricao = "Descrição Alterada";

            var result = await _assuntoDomainService.UpdateAsync(addedAssunto);

            result.Should().NotBeNull();
            result.Descricao.Should().Be("Descrição Alterada");
        }

        [Fact(DisplayName = "Atualizar Assunto deve falhar na validação")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenInvalid()
        {
            var assunto = GenerateValidAssunto();
            assunto.CodAs = 0;
            await _assuntoDomainService.AddAsync(assunto);
            assunto.Descricao = "";

            Func<Task> act = async () => await _assuntoDomainService.UpdateAsync(assunto);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("A descrição do assunto é obrigatória."));
        }

        [Fact(DisplayName = "Excluir Assunto deve falhar quando Assunto não encontrado")]
        public async Task DeleteAsync_ShouldThrowAssuntoNotFoundException_WhenAssuntoNotFound()
        {
            var assunto = GenerateValidAssunto();

            Func<Task> act = async () => await _assuntoDomainService.DeleteAsync(assunto);

            await act.Should().ThrowAsync<NotFoundExceptionAssunto>()
                .WithMessage($"Assunto {assunto.CodAs} não encontrado.");
        }

        [Fact(DisplayName = "Excluir Assunto com sucesso")]
        public async Task DeleteAsync_ShouldDeleteAssunto_WhenAssuntoExists()
        {
            var assunto = GenerateValidAssunto();
            assunto.CodAs = 0;
            var addedAssunto = await _assuntoDomainService.AddAsync(assunto);

            var result = await _assuntoDomainService.DeleteAsync(addedAssunto);

            result.Should().NotBeNull();
            result.CodAs.Should().Be(addedAssunto.CodAs);
        }

        [Fact(DisplayName = "Consultar Assunto por ID deve retornar Assunto existente")]
        public async Task GetByIdAsync_ShouldReturnAssunto_WhenAssuntoExists()
        {
            var assunto = GenerateValidAssunto();
            assunto.CodAs = 0;
            var addedAssunto = await _assuntoDomainService.AddAsync(assunto);

            var result = await _assuntoDomainService.GetByIdAsync(addedAssunto.CodAs);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(addedAssunto);
        }

        [Fact(DisplayName = "Consultar Assunto por ID deve retornar null quando Assunto não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenAssuntoNotFound()
        {
            var assuntoId = new Random().Next(100, 10000);

            var result = await _assuntoDomainService.GetByIdAsync(assuntoId);

            result.Should().BeNull();
        }

        [Fact(DisplayName = "Consultar todos os Assuntos")]
        public async Task GetManyAsync_ShouldReturnAssuntos_WhenAssuntosExist()
        {
            await AddAsync_ShouldAddAssunto_WhenValid();

            var result = await _assuntoDomainService.GetAllAsync();

            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThan(0);
        }
    }
}
