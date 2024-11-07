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
    public class AutorDomainServiceTest
    {
        private readonly AutorDomainService _autorDomainService;
        private readonly IAutorRepository _autorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DataContext _context;

        public AutorDomainServiceTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _context = new DataContext(options);
            _autorRepository = new AutorRepository(_context);
            _unitOfWork = new UnitOfWork(_context);
            _autorDomainService = new AutorDomainService(_unitOfWork);
        }

        private Autor GenerateValidAutor()
        {
            return new Faker<Autor>("pt_BR")
                .RuleFor(a => a.CodAu, f => f.Random.Int(1, 10000))
                .RuleFor(a => a.Nome, f => f.Person.FullName)
                .Generate();
        }

        [Fact(DisplayName = "Adicionar Autor com sucesso")]
        public async Task AddAsync_ShouldAddAutor_WhenValid()
        {
            var newAutor = GenerateValidAutor();
            newAutor.CodAu = 0;

            var result = await _autorDomainService.AddAsync(newAutor);

            result.Should().NotBeNull();
            result.Nome.Should().Be(newAutor.Nome);
        }

        [Fact(DisplayName = "Adicionar Autor deve falhar na validação")]
        public async Task AddAsync_ShouldThrowValidationException_WhenInvalid()
        {
            var autor = new Autor { Nome = "" };

            Func<Task> act = async () => await _autorDomainService.AddAsync(autor);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("O nome do autor é obrigatório."));
        }

        [Fact(DisplayName = "Atualizar Autor com sucesso")]
        public async Task UpdateAsync_ShouldUpdateAutor_WhenValid()
        {
            var autor = GenerateValidAutor();
            autor.CodAu = 0;
            var addedAutor = await _autorDomainService.AddAsync(autor);

            addedAutor.Nome = "Nome Alterado";

            var result = await _autorDomainService.UpdateAsync(addedAutor);

            result.Should().NotBeNull();
            result.Nome.Should().Be("Nome Alterado");
        }

        [Fact(DisplayName = "Atualizar Autor deve falhar na validação")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenInvalid()
        {
            var autor = GenerateValidAutor();
            autor.CodAu = 0;
            await _autorDomainService.AddAsync(autor);
            autor.Nome = "";

            Func<Task> act = async () => await _autorDomainService.UpdateAsync(autor);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("O nome do autor é obrigatório."));
        }

        [Fact(DisplayName = "Excluir Autor deve falhar quando Autor não encontrado")]
        public async Task DeleteAsync_ShouldThrowAutorNotFoundException_WhenAutorNotFound()
        {
            var autor = GenerateValidAutor();

            Func<Task> act = async () => await _autorDomainService.DeleteAsync(autor);

            await act.Should().ThrowAsync<NotFoundExceptionAutor>()
                .WithMessage($"Autor {autor.CodAu} não encontrado.");
        }

        [Fact(DisplayName = "Excluir Autor com sucesso")]
        public async Task DeleteAsync_ShouldDeleteAutor_WhenAutorExists()
        {
            var autor = GenerateValidAutor();
            autor.CodAu = 0;
            var addedAutor = await _autorDomainService.AddAsync(autor);

            var result = await _autorDomainService.DeleteAsync(addedAutor);

            result.Should().NotBeNull();
            result.CodAu.Should().Be(addedAutor.CodAu);
        }

        [Fact(DisplayName = "Consultar Autor por ID deve retornar Autor existente")]
        public async Task GetByIdAsync_ShouldReturnAutor_WhenAutorExists()
        {
            var autor = GenerateValidAutor();
            autor.CodAu = 0;
            var addedAutor = await _autorDomainService.AddAsync(autor);

            var result = await _autorDomainService.GetByIdAsync(addedAutor.CodAu);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(addedAutor);
        }

        [Fact(DisplayName = "Consultar Autor por ID deve retornar null quando Autor não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenAutorNotFound()
        {
            var autorId = new Random().Next(100, 10000);

            var result = await _autorDomainService.GetByIdAsync(autorId);

            result.Should().BeNull();
        }

        [Fact(DisplayName = "Consultar todos os Autores")]
        public async Task GetManyAsync_ShouldReturnAutores_WhenAutoresExist()
        {
            await AddAsync_ShouldAddAutor_WhenValid();

            var result = await _autorDomainService.GetAllAsync();

            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThan(0);
        }
    }
}
