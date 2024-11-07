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
    public class LivroAssuntoAppServiceTest
    {
        private readonly Mock<ILivroAssuntoDomainService> _livroAssuntoDomainServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly LivroAssuntoAppService _livroAssuntoAppService;

        public LivroAssuntoAppServiceTest()
        {
            _livroAssuntoDomainServiceMock = new Mock<ILivroAssuntoDomainService>();
            _mapperMock = new Mock<IMapper>();
            _livroAssuntoAppService = new LivroAssuntoAppService(_mapperMock.Object, _livroAssuntoDomainServiceMock.Object);
        }

        private LivroAssuntoDto GenerateLivroAssuntoDto()
        {
            return new Faker<LivroAssuntoDto>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AssuntoCodAs, f => f.Random.Int())
                .Generate();
        }

        private LivroAssunto GenerateLivroAssunto()
        {
            return new Faker<LivroAssunto>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AssuntoCodAs, f => f.Random.Int())
                .Generate();
        }

        private LivroAssuntoResponseDto GenerateLivroAssuntoResponseDto()
        {
            return new Faker<LivroAssuntoResponseDto>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AssuntoCodAs, f => f.Random.Int())
                .Generate();
        }

        [Fact(DisplayName = "Adicionar LivroAssunto com sucesso")]
        public async Task AddAsync_ShouldAddLivroAssunto_WhenValid()
        {
            // Arrange
            var livroAssuntoDto = GenerateLivroAssuntoDto();
            var livroAssunto = GenerateLivroAssunto();
            var livroAssuntoResponseDto = GenerateLivroAssuntoResponseDto();

            _mapperMock.Setup(m => m.Map<LivroAssunto>(livroAssuntoDto)).Returns(livroAssunto);
            _livroAssuntoDomainServiceMock.Setup(s => s.AddAsync(livroAssunto));
            _mapperMock.Setup(m => m.Map<LivroAssuntoResponseDto>(livroAssunto)).Returns(livroAssuntoResponseDto);

            // Act
            var result = await _livroAssuntoAppService.AddAsync(livroAssuntoDto);

            // Assert
            result.Should().BeEquivalentTo(livroAssuntoResponseDto);
        }

        [Fact(DisplayName = "Adicionar LivroAssunto deve falhar quando dados são inválidos")]
        public async Task AddAsync_ShouldThrowValidationException_WhenInvalidData()
        {
            // Arrange
            var livroAssuntoDto = new LivroAssuntoDto { LivroCodl = 0, AssuntoCodAs = 0 }; // Dados inválidos
            var livroAssunto = GenerateLivroAssunto();

            _mapperMock.Setup(m => m.Map<LivroAssunto>(livroAssuntoDto)).Returns(livroAssunto);
            _livroAssuntoDomainServiceMock.Setup(s => s.AddAsync(livroAssunto)).ThrowsAsync(new ValidationException("Campos LivroCodl e AssuntoCodAs são obrigatórios"));

            // Act
            Func<Task> act = async () => await _livroAssuntoAppService.AddAsync(livroAssuntoDto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Campos LivroCodl e AssuntoCodAs são obrigatórios");
        }

        [Fact(DisplayName = "Atualizar LivroAssunto com sucesso")]
        public async Task UpdateAsync_ShouldUpdateLivroAssunto_WhenValid()
        {
            // Arrange
            var livroAssuntoDto = GenerateLivroAssuntoDto();
            var livroAssunto = GenerateLivroAssunto();
            var livroAssuntoResponseDto = GenerateLivroAssuntoResponseDto();

            _mapperMock.Setup(m => m.Map<LivroAssunto>(livroAssuntoDto)).Returns(livroAssunto);
            _livroAssuntoDomainServiceMock.Setup(s => s.UpdateAsync(livroAssunto));
            _mapperMock.Setup(m => m.Map<LivroAssuntoResponseDto>(livroAssunto)).Returns(livroAssuntoResponseDto);

            // Act
            var result = await _livroAssuntoAppService.UpdateAsync(livroAssuntoDto);

            // Assert
            result.Should().BeEquivalentTo(livroAssuntoResponseDto);
        }

        [Fact(DisplayName = "Atualizar LivroAssunto deve falhar quando não encontrado")]
        public async Task UpdateAsync_ShouldThrowLivroAssuntoNotFoundException_WhenLivroAssuntoNotFound()
        {
            // Arrange
            var livroAssuntoDto = GenerateLivroAssuntoDto();
            var livroAssunto = GenerateLivroAssunto();

            _mapperMock.Setup(m => m.Map<LivroAssunto>(livroAssuntoDto)).Returns(livroAssunto);
            _livroAssuntoDomainServiceMock.Setup(s => s.UpdateAsync(livroAssunto)).ThrowsAsync(new NotFoundExceptionLivroAssunto(livroAssunto.LivroCodl, livroAssunto.AssuntoCodAs));

            // Act
            Func<Task> act = async () => await _livroAssuntoAppService.UpdateAsync(livroAssuntoDto);

            // Assert
            await act.Should().ThrowAsync<NotFoundExceptionLivroAssunto>().WithMessage($"Livro Assunto {livroAssunto.LivroCodl} e {livroAssunto.AssuntoCodAs} não encontrado.");
        }

        [Fact(DisplayName = "Excluir LivroAssunto com sucesso")]
        public async Task DeleteAsync_ShouldDeleteLivroAssunto_WhenLivroAssuntoExists()
        {
            // Arrange
            var livroAssuntoDto = GenerateLivroAssuntoDto();
            var livroAssunto = GenerateLivroAssunto();
            var livroAssuntoResponseDto = GenerateLivroAssuntoResponseDto();

            _mapperMock.Setup(m => m.Map<LivroAssunto>(livroAssuntoDto)).Returns(livroAssunto);
            _livroAssuntoDomainServiceMock.Setup(s => s.DeleteAsync(livroAssunto));
            _mapperMock.Setup(m => m.Map<LivroAssuntoResponseDto>(livroAssunto)).Returns(livroAssuntoResponseDto);

            // Act
            var result = await _livroAssuntoAppService.DeleteAsync(livroAssuntoDto);

            // Assert
            result.Should().BeEquivalentTo(livroAssuntoResponseDto);
        }

        [Fact(DisplayName = "Excluir LivroAssunto deve falhar quando não encontrado")]
        public async Task DeleteAsync_ShouldThrowLivroAssuntoNotFoundException_WhenLivroAssuntoNotFound()
        {
            // Arrange
            var livroAssuntoDto = GenerateLivroAssuntoDto();
            var livroAssunto = GenerateLivroAssunto();

            _mapperMock.Setup(m => m.Map<LivroAssunto>(livroAssuntoDto)).Returns(livroAssunto);
            _livroAssuntoDomainServiceMock.Setup(s => s.DeleteAsync(livroAssunto)).ThrowsAsync(new NotFoundExceptionLivroAssunto(livroAssunto.LivroCodl, livroAssunto.AssuntoCodAs));

            // Act
            Func<Task> act = async () => await _livroAssuntoAppService.DeleteAsync(livroAssuntoDto);

            // Assert
            await act.Should().ThrowAsync<NotFoundExceptionLivroAssunto>().WithMessage($"Livro Assunto {livroAssunto.LivroCodl} e {livroAssunto.AssuntoCodAs} não encontrado.");
        }

        [Fact(DisplayName = "Obter todos os LivroAssuntos com sucesso")]
        public async Task GetAllAsync_ShouldReturnLivroAssuntos_WhenLivroAssuntosExist()
        {
            // Arrange
            var livroAssuntos = new List<LivroAssunto> { GenerateLivroAssunto(), GenerateLivroAssunto() };
            var livroAssuntosResponseDto = new List<LivroAssuntoResponseDto> { GenerateLivroAssuntoResponseDto(), GenerateLivroAssuntoResponseDto() };

            _livroAssuntoDomainServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(livroAssuntos);
            _mapperMock.Setup(m => m.Map<List<LivroAssuntoResponseDto>>(livroAssuntos)).Returns(livroAssuntosResponseDto);

            // Act
            var result = await _livroAssuntoAppService.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(livroAssuntosResponseDto);
        }

        [Fact(DisplayName = "Obter LivroAssunto por ID com sucesso")]
        public async Task GetByIdAsync_ShouldReturnLivroAssunto_WhenLivroAssuntoExists()
        {
            // Arrange
            var livroAssunto = GenerateLivroAssunto();
            var livroAssuntoResponseDto = GenerateLivroAssuntoResponseDto();

            // Definindo chaves compostas para entidade e DTO
            var livroAssuntoPkDto = new LivroAssuntoPkDto
            {
                LivroCodl = livroAssunto.LivroCodl,
                AssuntoCodAs = livroAssunto.AssuntoCodAs
            };

            var livroAssuntoPk = new LivroAssuntoPk
            {
                LivroCodl = livroAssunto.LivroCodl,
                AssuntoCodAs = livroAssunto.AssuntoCodAs
            };

            // Configurando os mocks
            _mapperMock.Setup(m => m.Map<LivroAssuntoPk>(livroAssuntoPkDto)).Returns(livroAssuntoPk);
            _livroAssuntoDomainServiceMock.Setup(s => s.GetByIdAsync(livroAssuntoPk)).ReturnsAsync(livroAssunto);
            _mapperMock.Setup(m => m.Map<LivroAssuntoResponseDto>(livroAssunto)).Returns(livroAssuntoResponseDto);

            // Act
            var result = await _livroAssuntoAppService.GetByIdAsync(livroAssuntoPkDto);

            // Assert
            result.Should().BeEquivalentTo(livroAssuntoResponseDto);
        }

        [Fact(DisplayName = "Obter LivroAssunto por ID deve retornar null quando não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenLivroAssuntoNotFound()
        {
            // Arrange
            var livroCodl = new Faker().Random.Int();
            var assuntoCodAs = new Faker().Random.Int();

            var livroAssuntoDtoPK = new LivroAssuntoPkDto { LivroCodl = livroCodl, AssuntoCodAs = assuntoCodAs };
            var livroAssuntoPK = new LivroAssuntoPk { LivroCodl = livroCodl, AssuntoCodAs = assuntoCodAs };

            _livroAssuntoDomainServiceMock.Setup(s => s.GetByIdAsync(livroAssuntoPK)).ReturnsAsync((LivroAssunto)null);

            // Act
            var result = await _livroAssuntoAppService.GetByIdAsync(livroAssuntoDtoPK);

            // Assert
            result.Should().BeNull();
        }
    }
}
