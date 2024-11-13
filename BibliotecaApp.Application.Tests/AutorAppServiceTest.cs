using AutoMapper;
using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Mappings;
using BibliotecaApp.Aplication.Services;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Interfaces.Services;
using BibliotecaApp.Domain.Services;
using BibliotecaApp.Infra.Data.Context;
using BibliotecaApp.Infra.Data.Repositories;
using BibliotecaApp.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentValidation;

namespace BibliotecaApp.Aplication.Test.Services
{
    public class AutorAppServiceTest
    {
        private readonly IMapper _mapper;
        private readonly AutorAppService _autorAppService;
        private readonly DataContext _context;
        private readonly AutorDomainService _AutorDomainService;

        public AutorAppServiceTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _context = new DataContext(options, new LoggerFactory().CreateLogger<DataContext>());
            var AutorRepository = new AutorRepository(_context);
            var unitOfWork = new UnitOfWork(_context);
            _AutorDomainService = new AutorDomainService(unitOfWork);

            var config = new MapperConfiguration(cfg => cfg.AddProfile(new AutorMapping()));
            _mapper = config.CreateMapper();
            _autorAppService = new AutorAppService(_mapper, _AutorDomainService);
        }

        private AutorInsertDto GenerateAutorInsertDto() => new AutorInsertDto
        {
            Nome = "Nome Teste"
        };

        private AutorUpdateDto GenerateAutorUpdateDto(int codAu) => new AutorUpdateDto
        {
            CodAu = codAu,
            Nome= "Nome Atualizada"
        };

        private AutorDeleteDto GenerateAutorDeleteDto(int codAu) => new AutorDeleteDto
        {
            CodAu = codAu
        };

        [Fact(DisplayName = "Adicionar Autor com sucesso")]
        public async Task AddAsync_ShouldAddAutor_WhenValid()
        {
            var AutorInsertDto = GenerateAutorInsertDto();
            var result = await _autorAppService.AddAsync(AutorInsertDto);

            result.Should().NotBeNull();
            result.Nome.Should().Be(AutorInsertDto.Nome);
            result.CodAu.Should().BeGreaterThan(0);
        }

        [Fact(DisplayName = "Atualizar Autor com sucesso")]
        public async Task UpdateAsync_ShouldUpdateAutor_WhenValid()
        {
            var AutorInsertDto = GenerateAutorInsertDto();
            var addedAutor = await _autorAppService.AddAsync(AutorInsertDto);
            var AutorUpdateDto = GenerateAutorUpdateDto(addedAutor.CodAu);

            var result = await _autorAppService.UpdateAsync(AutorUpdateDto);

            result.Should().NotBeNull();
            result.CodAu.Should().Be(addedAutor.CodAu);
            result.Nome.Should().Be(AutorUpdateDto.Nome);
        }

        [Fact(DisplayName = "Excluir Autor com sucesso")]
        public async Task DeleteAsync_ShouldDeleteAutor_WhenAutorExists()
        {
            var AutorInsertDto = GenerateAutorInsertDto();
            var addedAutor = await _autorAppService.AddAsync(AutorInsertDto);
            var AutorDeleteDto = GenerateAutorDeleteDto(addedAutor.CodAu);

            var result = await _autorAppService.DeleteAsync(AutorDeleteDto);

            result.Should().NotBeNull();
            result.CodAu.Should().Be(AutorDeleteDto.CodAu);
        }

        [Fact(DisplayName = "Obter Autor por ID com sucesso")]
        public async Task GetByIdAsync_ShouldReturnAutor_WhenAutorExists()
        {
            var AutorInsertDto = GenerateAutorInsertDto();
            var addedAutor = await _autorAppService.AddAsync(AutorInsertDto);

            var result = await _autorAppService.GetByIdAsync(addedAutor.CodAu);

            result.Should().NotBeNull();
            result.CodAu.Should().Be(addedAutor.CodAu);
            result.Nome.Should().Be(addedAutor.Nome);
        }

        [Fact(DisplayName = "Obter todos os Autors com sucesso")]
        public async Task GetAllAsync_ShouldReturnAllAutors_WhenAutorsExist()
        {
            await _autorAppService.AddAsync(GenerateAutorInsertDto());
            await _autorAppService.AddAsync(GenerateAutorInsertDto());

            var result = await _autorAppService.GetAllAsync();

            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(1);
        }

        // Tests that should fail due to validation issues

        [Fact(DisplayName = "Adicionar Autor deve falhar quando o nome estiver vazio")]
        public async Task AddAsync_ShouldThrowValidationException_WhenNomeIsEmpty()
        {
            var invalidAutorInsertDto = new AutorInsertDto { Nome = "" };

            Func<Task> act = async () => await _autorAppService.AddAsync(invalidAutorInsertDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*O nome do autor é obrigatório.*");
        }

        [Fact(DisplayName = "Atualizar Autor deve falhar quando o nome estiver vazia")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenNomeIsEmpty()
        {
            var AutorInsertDto = GenerateAutorInsertDto();
            var addedAutor = await _autorAppService.AddAsync(AutorInsertDto);

            var invalidAutorUpdateDto = new AutorUpdateDto
            {
                CodAu = addedAutor.CodAu,
                Nome = ""
            };

            Func<Task> act = async () => await _autorAppService.UpdateAsync(invalidAutorUpdateDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*O nome do autor é obrigatório.*");
        }

        [Fact(DisplayName = "Atualizar Autor deve falhar quando o código do Autor não existe")]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenAutorDoesNotExist()
        {
            var invalidAutorUpdateDto = new AutorUpdateDto
            {
                CodAu = 9999,
                Nome = "Nome Atualizado"
            };

            Func<Task> act = async () => await _autorAppService.UpdateAsync(invalidAutorUpdateDto);

            await act.Should().ThrowAsync<NotFoundExceptionAutor>()
                .WithMessage($"Autor {invalidAutorUpdateDto.CodAu} não encontrado.");
        }

        [Fact(DisplayName = "Excluir Autor deve falhar quando o código do Autor não existe")]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenAutorDoesNotExist()
        {
            var invalidAutorDeleteDto = new AutorDeleteDto { CodAu = 9999 };

            Func<Task> act = async () => await _autorAppService.DeleteAsync(invalidAutorDeleteDto);

            await act.Should().ThrowAsync<NotFoundExceptionAutor>()
                .WithMessage($"Autor {invalidAutorDeleteDto.CodAu} não encontrado.");
        }
    }
}
