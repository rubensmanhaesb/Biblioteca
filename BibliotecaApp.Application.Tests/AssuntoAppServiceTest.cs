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
    public class AssuntoAppServiceTest
    {
        private readonly IMapper _mapper;
        private readonly AssuntoAppService _assuntoAppService;
        private readonly DataContext _context;
        private readonly AssuntoDomainService _assuntoDomainService;

        public AssuntoAppServiceTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _context = new DataContext(options, new LoggerFactory().CreateLogger<DataContext>());
            var assuntoRepository = new AssuntoRepository(_context);
            var unitOfWork = new UnitOfWork(_context);
            _assuntoDomainService = new AssuntoDomainService(unitOfWork);

            var config = new MapperConfiguration(cfg => cfg.AddProfile(new AssuntoMapping()));
            _mapper = config.CreateMapper();
            _assuntoAppService = new AssuntoAppService(_mapper, _assuntoDomainService);
        }

        private AssuntoInsertDto GenerateAssuntoInsertDto() => new AssuntoInsertDto
        {
            Descricao = "Descrição Teste"
        };

        private AssuntoUpdateDto GenerateAssuntoUpdateDto(int codAs) => new AssuntoUpdateDto
        {
            CodAs = codAs,
            Descricao = "Descrição Atualizada"
        };

        private AssuntoDeleteDto GenerateAssuntoDeleteDto(int codAs) => new AssuntoDeleteDto
        {
            CodAs = codAs
        };

        [Fact(DisplayName = "Adicionar Assunto com sucesso")]
        public async Task AddAsync_ShouldAddAssunto_WhenValid()
        {
            var assuntoInsertDto = GenerateAssuntoInsertDto();
            var result = await _assuntoAppService.AddAsync(assuntoInsertDto);

            result.Should().NotBeNull();
            result.Descricao.Should().Be(assuntoInsertDto.Descricao);
            result.CodAs.Should().BeGreaterThan(0);
        }

        [Fact(DisplayName = "Atualizar Assunto com sucesso")]
        public async Task UpdateAsync_ShouldUpdateAssunto_WhenValid()
        {
            var assuntoInsertDto = GenerateAssuntoInsertDto();
            var addedAssunto = await _assuntoAppService.AddAsync(assuntoInsertDto);
            var assuntoUpdateDto = GenerateAssuntoUpdateDto(addedAssunto.CodAs);

            var result = await _assuntoAppService.UpdateAsync(assuntoUpdateDto);

            result.Should().NotBeNull();
            result.CodAs.Should().Be(addedAssunto.CodAs);
            result.Descricao.Should().Be(assuntoUpdateDto.Descricao);
        }

        [Fact(DisplayName = "Excluir Assunto com sucesso")]
        public async Task DeleteAsync_ShouldDeleteAssunto_WhenAssuntoExists()
        {
            var assuntoInsertDto = GenerateAssuntoInsertDto();
            var addedAssunto = await _assuntoAppService.AddAsync(assuntoInsertDto);
            var assuntoDeleteDto = GenerateAssuntoDeleteDto(addedAssunto.CodAs);

            var result = await _assuntoAppService.DeleteAsync(assuntoDeleteDto);

            result.Should().NotBeNull();
            result.CodAs.Should().Be(assuntoDeleteDto.CodAs);
        }

        [Fact(DisplayName = "Obter Assunto por ID com sucesso")]
        public async Task GetByIdAsync_ShouldReturnAssunto_WhenAssuntoExists()
        {
            var assuntoInsertDto = GenerateAssuntoInsertDto();
            var addedAssunto = await _assuntoAppService.AddAsync(assuntoInsertDto);

            var result = await _assuntoAppService.GetByIdAsync(addedAssunto.CodAs);

            result.Should().NotBeNull();
            result.CodAs.Should().Be(addedAssunto.CodAs);
            result.Descricao.Should().Be(addedAssunto.Descricao);
        }

        [Fact(DisplayName = "Obter todos os Assuntos com sucesso")]
        public async Task GetAllAsync_ShouldReturnAllAssuntos_WhenAssuntosExist()
        {
            await _assuntoAppService.AddAsync(GenerateAssuntoInsertDto());
            await _assuntoAppService.AddAsync(GenerateAssuntoInsertDto());

            var result = await _assuntoAppService.GetAllAsync();

            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(1);
        }

        // Tests that should fail due to validation issues

        [Fact(DisplayName = "Adicionar Assunto deve falhar quando Descrição estiver vazia")]
        public async Task AddAsync_ShouldThrowValidationException_WhenDescricaoIsEmpty()
        {
            var invalidAssuntoInsertDto = new AssuntoInsertDto { Descricao = "" };

            Func<Task> act = async () => await _assuntoAppService.AddAsync(invalidAssuntoInsertDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*A descrição do assunto é obrigatória.*");
        }

        [Fact(DisplayName = "Atualizar Assunto deve falhar quando Descrição estiver vazia")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenDescricaoIsEmpty()
        {
            var assuntoInsertDto = GenerateAssuntoInsertDto();
            var addedAssunto = await _assuntoAppService.AddAsync(assuntoInsertDto);

            var invalidAssuntoUpdateDto = new AssuntoUpdateDto
            {
                CodAs = addedAssunto.CodAs,
                Descricao = ""
            };

            Func<Task> act = async () => await _assuntoAppService.UpdateAsync(invalidAssuntoUpdateDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*A descrição do assunto é obrigatória.*");
        }

        [Fact(DisplayName = "Atualizar Assunto deve falhar quando o código do Assunto não existe")]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenAssuntoDoesNotExist()
        {
            var invalidAssuntoUpdateDto = new AssuntoUpdateDto
            {
                CodAs = 9999,
                Descricao = "Descrição Atualizada"
            };

            Func<Task> act = async () => await _assuntoAppService.UpdateAsync(invalidAssuntoUpdateDto);

            await act.Should().ThrowAsync<NotFoundExceptionAssunto>()
                .WithMessage($"Assunto {invalidAssuntoUpdateDto.CodAs} não encontrado.");
        }

        [Fact(DisplayName = "Excluir Assunto deve falhar quando o código do Assunto não existe")]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenAssuntoDoesNotExist()
        {
            var invalidAssuntoDeleteDto = new AssuntoDeleteDto { CodAs = 9999 };

            Func<Task> act = async () => await _assuntoAppService.DeleteAsync(invalidAssuntoDeleteDto);

            await act.Should().ThrowAsync<NotFoundExceptionAssunto>()
                .WithMessage($"Assunto {invalidAssuntoDeleteDto.CodAs} não encontrado.");
        }
    }
}
