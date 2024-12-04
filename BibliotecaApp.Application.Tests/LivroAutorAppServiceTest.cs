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
    public class LivroAutorAppServiceTest
    {
        private readonly Mock<ILivroAutorDomainService> _livroAutorDomainServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly LivroAutorAppService _livroAutorAppService;

        public LivroAutorAppServiceTest()
        {
            _livroAutorDomainServiceMock = new Mock<ILivroAutorDomainService>();
            _mapperMock = new Mock<IMapper>();
            _livroAutorAppService = new LivroAutorAppService(_mapperMock.Object, _livroAutorDomainServiceMock.Object);
        }

        private LivroAutorDto GenerateLivroAutorDto()
        {
            return new Faker<LivroAutorDto>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AutorCodAu, f => f.Random.Int())
                .Generate();
        }

        private LivroAutor GenerateLivroAutor()
        {
            return new Faker<LivroAutor>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AutorCodAu, f => f.Random.Int())
                .Generate();
        }

        private LivroAutorResponseDto GenerateLivroAutorResponseDto()
        {
            return new Faker<LivroAutorResponseDto>()
                .RuleFor(la => la.LivroCodl, f => f.Random.Int())
                .RuleFor(la => la.AutorCodAu, f => f.Random.Int())
                .Generate();
        }

        private async Task<(LivroAutorDto, LivroAutorResponseDto)> ExecuteAddLivroAutorRoutineAsync()
        {
            // Arrange
            var livroAutorDto = GenerateLivroAutorDto();
            var livroAutor = new LivroAutor
            {
                LivroCodl = livroAutorDto.LivroCodl,
                AutorCodAu = livroAutorDto.AutorCodAu
            };
            var livroAutorResponseDto = new LivroAutorResponseDto
            {
                LivroCodl = livroAutor.LivroCodl,
                AutorCodAu = livroAutor.AutorCodAu
            };

            // Configurar os mocks
            _mapperMock.Setup(m => m.Map<LivroAutor>(livroAutorDto)).Returns(livroAutor);
            _livroAutorDomainServiceMock.Setup(s => s.AddAsync(livroAutor));
            _mapperMock.Setup(m => m.Map<LivroAutorResponseDto>(livroAutor)).Returns(livroAutorResponseDto);

            // Act
            var result = await _livroAutorAppService.AddAsync(livroAutorDto);

            return (livroAutorDto, result);
        }

        [Fact(DisplayName = "Adicionar LivroAutor com sucesso")]
        public async Task AddAsync_ShouldAddLivroAutor_WhenValid()
        {
            var (livroAutorDto, result) = await ExecuteAddLivroAutorRoutineAsync();


            result.Should().BeEquivalentTo(livroAutorDto);
        }

        [Fact(DisplayName = "Adicionar LivroAutor deve falhar quando dados são inválidos")]
        public async Task AddAsync_ShouldThrowValidationException_WhenInvalidData()
        {
            // Arrange
            var livroAutorDto = new LivroAutorDto { LivroCodl = 0, AutorCodAu = 0 }; // Dados inválidos
            var livroAutor = GenerateLivroAutor();

            _mapperMock.Setup(m => m.Map<LivroAutor>(livroAutorDto)).Returns(livroAutor);
            _livroAutorDomainServiceMock.Setup(s => s.AddAsync(livroAutor)).ThrowsAsync(new ValidationException("Campos LivroCodl e AutorCodAs são obrigatórios"));

            // Act
            Func<Task> act = async () => await _livroAutorAppService.AddAsync(livroAutorDto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Campos LivroCodl e AutorCodAs são obrigatórios");
        }

        [Fact(DisplayName = "Atualizar LivroAutor com sucesso")]
        public async Task UpdateAsync_ShouldUpdateLivroAutor_WhenValid()
        {
            // Arrange
            var livroAutorDto = GenerateLivroAutorDto();
            var livroAutor = GenerateLivroAutor();
            var livroAutorResponseDto = GenerateLivroAutorResponseDto();

            _mapperMock.Setup(m => m.Map<LivroAutor>(livroAutorDto)).Returns(livroAutor);
            _livroAutorDomainServiceMock.Setup(s => s.UpdateAsync(livroAutor));
            _mapperMock.Setup(m => m.Map<LivroAutorResponseDto>(livroAutor)).Returns(livroAutorResponseDto);

            // Act
            var result = await _livroAutorAppService.UpdateAsync(livroAutorDto);

            // Assert
            result.Should().BeEquivalentTo(livroAutorResponseDto);
        }

        [Fact(DisplayName = "Atualizar LivroAutor deve falhar quando não encontrado")]
        public async Task UpdateAsync_ShouldThrowLivroAutorNotFoundException_WhenLivroAutorNotFound()
        {
            // Arrange
            var livroAutorDto = GenerateLivroAutorDto();
            var livroAutor = GenerateLivroAutor();

            _mapperMock.Setup(m => m.Map<LivroAutor>(livroAutorDto)).Returns(livroAutor);
            _livroAutorDomainServiceMock.Setup(s => s.UpdateAsync(livroAutor)).ThrowsAsync(new NotFoundExceptionLivroAutor(livroAutor.LivroCodl, livroAutor.AutorCodAu));

            // Act
            Func<Task> act = async () => await _livroAutorAppService.UpdateAsync(livroAutorDto);

            // Assert
            await act.Should().ThrowAsync<NotFoundExceptionLivroAutor>().WithMessage($"Livro Autor {livroAutor.LivroCodl} e {livroAutor.AutorCodAu} não encontrado.");
        }

        [Fact(DisplayName = "Excluir LivroAutor com sucesso")]
        public async Task DeleteAsync_ShouldDeleteLivroAutor_WhenLivroAutorExists()
        {
            // Arrange: Inclui um LivroAutor para teste de exclusão
            var (livroAutorDto, livroAutorResponseDto) = await ExecuteAddLivroAutorRoutineAsync();

            var livroAutor = new LivroAutor
            {
                LivroCodl = livroAutorDto.LivroCodl,
                AutorCodAu = livroAutorDto.AutorCodAu
            };

            // Configurar o mock para GetByIdAsync
            _livroAutorDomainServiceMock
                .Setup(s => s.GetByIdAsync(It.Is<LivroAutorPk>(pk =>
                    pk.LivroCodl == livroAutorDto.LivroCodl &&
                    pk.AutorCodAu == livroAutorDto.AutorCodAu)))
                .ReturnsAsync(livroAutor);

            // Configurar o mock para DeleteAsync
            _livroAutorDomainServiceMock.Setup(s => s.DeleteAsync(It.IsAny<LivroAutor>()));

            // Configurar o mapeamento para o DTO de resposta
            _mapperMock.Setup(m => m.Map<LivroAutorResponseDto>(livroAutor)).Returns(livroAutorResponseDto);

            // Act: Executa o método de exclusão
            var result = await _livroAutorAppService.DeleteAsync(livroAutorDto);

            // Assert
            result.Should().BeEquivalentTo(livroAutorResponseDto);

            // Verificar que GetByIdAsync foi chamado corretamente
            _livroAutorDomainServiceMock.Verify(s => s.GetByIdAsync(It.IsAny<LivroAutorPk>()), Times.Once);

            // Verificar que DeleteAsync foi chamado
            _livroAutorDomainServiceMock.Verify(s => s.DeleteAsync(It.IsAny<LivroAutor>()), Times.Once);
        }

        [Fact(DisplayName = "Excluir LivroAutor deve falhar quando não encontrado")]
        public async Task DeleteAsync_ShouldThrowLivroAutorNotFoundException_WhenLivroAutorNotFound()
        {
            // Arrange
            var livroAutorDto = GenerateLivroAutorDto();
            var livroAutor = GenerateLivroAutor();

            _mapperMock.Setup(m => m.Map<LivroAutor>(livroAutorDto)).Returns(livroAutor);
            _livroAutorDomainServiceMock.Setup(s => s.DeleteAsync(livroAutor)).ThrowsAsync(new NotFoundExceptionLivroAutor(livroAutor.LivroCodl, livroAutor.AutorCodAu));

            // Act
            Func<Task> act = async () => await _livroAutorAppService.DeleteAsync(livroAutorDto);

            // Assert
            await act.Should().ThrowAsync<NotFoundExceptionLivroAutor>().WithMessage($"Livro Autor {livroAutor.LivroCodl} e {livroAutor.AutorCodAu} não encontrado.");
        }

        [Fact(DisplayName = "Obter todos os LivroAutors com sucesso")]
        public async Task GetAllAsync_ShouldReturnLivroAutors_WhenLivroAutorsExist()
        {
            // Arrange
            var livroAutors = new List<LivroAutor> { GenerateLivroAutor(), GenerateLivroAutor() };
            var livroAutorsResponseDto = new List<LivroAutorResponseDto> { GenerateLivroAutorResponseDto(), GenerateLivroAutorResponseDto() };

            _livroAutorDomainServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(livroAutors);
            _mapperMock.Setup(m => m.Map<List<LivroAutorResponseDto>>(livroAutors)).Returns(livroAutorsResponseDto);

            // Act
            var result = await _livroAutorAppService.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(livroAutorsResponseDto);
        }

        [Fact(DisplayName = "Obter LivroAutor por ID com sucesso")]
        public async Task GetByIdAsync_ShouldReturnLivroAutor_WhenLivroAutorExists()
        {
            // Arrange
            var livroAutor = GenerateLivroAutor();
            var livroAutorResponseDto = GenerateLivroAutorResponseDto();

            // Definindo chaves compostas para entidade e DTO
            var livroAutorPkDto = new LivroAutorPkDto
            {
                LivroCodl = livroAutor.LivroCodl,
                AutorCodAu = livroAutor.AutorCodAu
            };

            var livroAutorPk = new LivroAutorPk
            {
                LivroCodl = livroAutor.LivroCodl,
                AutorCodAu = livroAutor.AutorCodAu
            };

            // Configurando os mocks
            _mapperMock.Setup(m => m.Map<LivroAutorPk>(livroAutorPkDto)).Returns(livroAutorPk);
            _livroAutorDomainServiceMock.Setup(s => s.GetByIdAsync(livroAutorPk)).ReturnsAsync(livroAutor);
            _mapperMock.Setup(m => m.Map<LivroAutorResponseDto>(livroAutor)).Returns(livroAutorResponseDto);

            // Act
            var result = await _livroAutorAppService.GetByIdAsync(livroAutorPkDto);

            // Assert
            result.Should().BeEquivalentTo(livroAutorResponseDto);
        }

        [Fact(DisplayName = "Obter LivroAutor por ID deve retornar null quando não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenLivroAutorNotFound()
        {
            // Arrange
            var livroCodl = new Faker().Random.Int();
            var AutorCodAs = new Faker().Random.Int();

            var livroAutorDtoPK = new LivroAutorPkDto { LivroCodl = livroCodl, AutorCodAu = AutorCodAs };
            var livroAutorPK = new LivroAutorPk { LivroCodl = livroCodl, AutorCodAu = AutorCodAs };

            _livroAutorDomainServiceMock.Setup(s => s.GetByIdAsync(livroAutorPK)).ReturnsAsync((LivroAutor)null);

            // Act
            var result = await _livroAutorAppService.GetByIdAsync(livroAutorDtoPK);

            // Assert
            result.Should().BeNull();
        }
    }
}
