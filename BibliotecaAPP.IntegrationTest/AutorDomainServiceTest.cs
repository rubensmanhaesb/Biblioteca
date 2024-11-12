using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Services;
using BibliotecaApp.Infra.Data.Context;
using BibliotecaApp.Infra.Data.Repositories;
using FluentValidation;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
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
            return new Autor
            {
                Nome = "Nome Válido"
            };
        }

        [Fact(DisplayName = "Adicionar Autor com sucesso")]
        public async Task AddAsync_ShouldAddAutor_WhenValid()
        {
            var newAutor = GenerateValidAutor();

            var result = await _autorDomainService.AddAsync(newAutor);

            result.Should().NotBeNull();
            result.Nome.Should().Be(newAutor.Nome);
        }

        [Fact(DisplayName = "Adicionar Autor deve falhar quando nome estiver vazio")]
        public async Task AddAsync_ShouldThrowValidationException_WhenNomeIsEmpty()
        {
            var newAutor = GenerateValidAutor();
            newAutor.Nome = ""; // Campo obrigatório

            Func<Task> act = async () => await _autorDomainService.AddAsync(newAutor);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("O nome do autor é obrigatório."));
        }

        [Fact(DisplayName = "Adicionar Autor deve falhar quando informado o codAu")]
        public async Task AddAsync_ShouldThrowValidationException_WhenVodeAuIsNotEmpty()
        {
            var newAutor = GenerateValidAutor();
            newAutor.CodAu= 1; // Campo obrigatório estar vazio

            Func<Task> act = async () => await _autorDomainService.AddAsync(newAutor);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("Código do autor não deve ser informado na inclusão."));
        }


        [Fact(DisplayName = "Atualizar Autor com sucesso")]
        public async Task UpdateAsync_ShouldUpdateAutor_WhenValid()
        {
            var autor = GenerateValidAutor();
            var addedAutor = await _autorDomainService.AddAsync(autor);

            addedAutor.Nome = "Nome Alterado";

            var result = await _autorDomainService.UpdateAsync(addedAutor);

            result.Should().NotBeNull();
            result.Nome.Should().Be("Nome Alterado");
        }

        [Fact(DisplayName = "Atualizar Autor deve falhar quando autor não encontrado")]
        public async Task UpdateAsync_ShouldThrowAutorNotFoundException_WhenAutorNotFound()
        {
            var autor = GenerateValidAutor();
            autor.CodAu = new Random().Next(1, 10000);

            Func<Task> act = async () => await _autorDomainService.UpdateAsync(autor);

            await act.Should().ThrowAsync<NotFoundExceptionAutor>()
                .WithMessage($"Autor {autor.CodAu} não encontrado.");
        }

        [Fact(DisplayName = "Atualizar Autor deve falhar quando nome estiver vazio")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenNomeIsEmpty()
        {
            var autor = GenerateValidAutor();
            var addedAutor = await _autorDomainService.AddAsync(autor);

            addedAutor.Nome = ""; // Campo obrigatório inválido

            Func<Task> act = async () => await _autorDomainService.UpdateAsync(addedAutor);

            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("O nome do autor é obrigatório."));
        }

        [Fact(DisplayName = "Excluir Autor deve falhar quando autor não encontrado")]
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
            await _autorDomainService.AddAsync(autor);

            var result = await _autorDomainService.DeleteAsync(autor);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(autor);
        }

        [Fact(DisplayName = "Consultar Autor por ID deve retornar autor existente")]
        public async Task GetByIdAsync_ShouldReturnAutor_WhenAutorExists()
        {
            var autor = GenerateValidAutor();

            var addedAutor = await _autorDomainService.AddAsync(autor);

            var result = await _autorDomainService.GetByIdAsync(addedAutor.CodAu);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(addedAutor);
        }

        [Fact(DisplayName = "Consultar Autor por ID deve retornar null quando autor não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenAutorNotFound()
        {
            var autorId = new Random().Next(100, 10000);

            var result = await _autorDomainService.GetByIdAsync(autorId);

            result.Should().BeNull();
        }

        [Fact(DisplayName = "Consultar todos os autores")]
        public async Task GetManyAsync_ShouldReturnAutores_WhenAutoresExist()
        {
            var autor1 = GenerateValidAutor();

            var addedAutor = await _autorDomainService.AddAsync(autor1);

            await AddAsync_ShouldAddAutor_WhenValid();

            var result = await _autorDomainService.GetAllAsync();

            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThanOrEqualTo(1);
        }
    }
}
