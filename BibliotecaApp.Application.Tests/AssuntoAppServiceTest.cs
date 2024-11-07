using AutoMapper;
using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Services;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Exceptions;
using BibliotecaApp.Domain.Interfaces.Services;
using Bogus;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BibliotecaApp.Aplication.Test.Services
{
    public class AssuntoAppServiceTest
    {
        private readonly Mock<IAssuntoDomainService> _assuntoDomainServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AssuntoAppService _assuntoAppService;

        public AssuntoAppServiceTest()
        {
            _assuntoDomainServiceMock = new Mock<IAssuntoDomainService>();
            _mapperMock = new Mock<IMapper>();
            _assuntoAppService = new AssuntoAppService(_mapperMock.Object, _assuntoDomainServiceMock.Object);
        }

        private AssuntoInsertDto GenerateAssuntoInsertDto() => new Faker<AssuntoInsertDto>()
            .RuleFor(a => a.Descricao, f => f.Lorem.Letter(20)).Generate();

        private AssuntoUpdateDto GenerateAssuntoUpdateDto() => new Faker<AssuntoUpdateDto>()
            .RuleFor(a => a.CodAs, f => f.Random.Int(1, 100))
            .RuleFor(a => a.Descricao, f => f.Lorem.Letter(20)).Generate();

        private AssuntoDeleteDto GenerateAssuntoDeleteDto() => new Faker<AssuntoDeleteDto>()
            .RuleFor(a => a.CodAs, f => f.Random.Int(1, 100)).Generate();

        private Assunto GenerateAssunto() => new Faker<Assunto>()
            .RuleFor(a => a.CodAs, f => f.Random.Int(1, 100))
            .RuleFor(a => a.Descricao, f => f.Lorem.Letter(20)).Generate();

        [Fact(DisplayName = "Adicionar Assunto com sucesso")]
        public async Task AddAsync_ShouldAddAssunto_WhenValid()
        {
            var assuntoInsertDto = GenerateAssuntoInsertDto();
            var assunto = GenerateAssunto();

            _mapperMock.Setup(m => m.Map<Assunto>(assuntoInsertDto)).Returns(assunto);
            _assuntoDomainServiceMock.Setup(s => s.AddAsync(assunto));

            _mapperMock.Setup(m => m.Map<AssuntoResponseDto>(assunto)).Returns(new AssuntoResponseDto
            {
                CodAs = assunto.CodAs,
                Descricao = assunto.Descricao
            });

            var result = await _assuntoAppService.AddAsync(assuntoInsertDto);

            result.Should().BeEquivalentTo(new AssuntoResponseDto
            {
                CodAs = assunto.CodAs,
                Descricao = assunto.Descricao
            });
        }

        [Fact(DisplayName = "Atualizar Assunto com sucesso")]
        public async Task UpdateAsync_ShouldUpdateAssunto_WhenValid()
        {
            var assuntoInsertDto = GenerateAssuntoInsertDto();
            var assunto = GenerateAssunto();
            var assuntoUpdateDto = GenerateAssuntoUpdateDto();

            _mapperMock.Setup(m => m.Map<Assunto>(assuntoInsertDto)).Returns(assunto);
            _assuntoDomainServiceMock.Setup(s => s.AddAsync(assunto));

            _mapperMock.Setup(m => m.Map<Assunto>(assuntoUpdateDto)).Returns(assunto);
            _assuntoDomainServiceMock.Setup(s => s.UpdateAsync(assunto));

            _mapperMock.Setup(m => m.Map<AssuntoResponseDto>(assunto)).Returns(new AssuntoResponseDto
            {
                CodAs = assunto.CodAs,
                Descricao = assunto.Descricao
            });

            var result = await _assuntoAppService.UpdateAsync(assuntoUpdateDto);

            result.Should().BeEquivalentTo(new AssuntoResponseDto
            {
                CodAs = assunto.CodAs,
                Descricao = assunto.Descricao
            });

            await _assuntoDomainServiceMock.Object.DeleteAsync(assunto);
        }

        [Fact(DisplayName = "Excluir Assunto com sucesso")]
        public async Task DeleteAsync_ShouldDeleteAssunto_WhenAssuntoExists()
        {
            var assuntoInsertDto = GenerateAssuntoInsertDto();
            var assunto = GenerateAssunto();
            var assuntoDeleteDto = GenerateAssuntoDeleteDto();

            _mapperMock.Setup(m => m.Map<Assunto>(assuntoInsertDto)).Returns(assunto);
            _assuntoDomainServiceMock.Setup(s => s.AddAsync(assunto));

            _mapperMock.Setup(m => m.Map<Assunto>(assuntoDeleteDto)).Returns(assunto);
            _assuntoDomainServiceMock.Setup(s => s.DeleteAsync(assunto));

            _mapperMock.Setup(m => m.Map<AssuntoResponseDto>(assunto)).Returns(new AssuntoResponseDto
            {
                CodAs = assunto.CodAs,
                Descricao = assunto.Descricao
            });

            var result = await _assuntoAppService.DeleteAsync(assuntoDeleteDto);

            result.Should().BeEquivalentTo(new AssuntoResponseDto
            {
                CodAs = assunto.CodAs,
                Descricao = assunto.Descricao
            });
        }

        [Fact(DisplayName = "Obter Assunto por ID com sucesso")]
        public async Task GetByIdAsync_ShouldReturnAssunto_WhenAssuntoExists()
        {
            var assunto = GenerateAssunto();

            _assuntoDomainServiceMock.Setup(s => s.GetByIdAsync(assunto.CodAs)).ReturnsAsync(assunto);

            _mapperMock.Setup(m => m.Map<AssuntoResponseDto>(assunto)).Returns(new AssuntoResponseDto
            {
                CodAs = assunto.CodAs,
                Descricao = assunto.Descricao
            });

            var result = await _assuntoAppService.GetByIdAsync(assunto.CodAs);

            result.Should().BeEquivalentTo(new AssuntoResponseDto
            {
                CodAs = assunto.CodAs,
                Descricao = assunto.Descricao
            });
        }

        [Fact(DisplayName = "Obter Assunto por ID deve retornar null quando Assunto não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenAssuntoNotFound()
        {
            var assuntoId = new Faker().Random.Int();

            _assuntoDomainServiceMock.Setup(s => s.GetByIdAsync(assuntoId)).ReturnsAsync((Assunto)null);

            var result = await _assuntoAppService.GetByIdAsync(assuntoId);

            result.Should().BeNull();
        }
    }
}
