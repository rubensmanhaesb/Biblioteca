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
    public class AutorAppServiceTest
    {
        private readonly Mock<IAutorDomainService> _autorDomainServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AutorAppService _autorAppService;

        public AutorAppServiceTest()
        {
            _autorDomainServiceMock = new Mock<IAutorDomainService>();
            _mapperMock = new Mock<IMapper>();
            _autorAppService = new AutorAppService(_mapperMock.Object, _autorDomainServiceMock.Object);
        }

        private AutorInsertDto GenerateAutorInsertDto()
        {
            return new Faker<AutorInsertDto>()
                .RuleFor(a => a.Nome, f => f.Person.FullName)
                .Generate();
        }

        private AutorUpdateDto GenerateAutorUpdateDto()
        {
            return new Faker<AutorUpdateDto>()
                .RuleFor(a => a.CodAu, f => f.Random.Int(1, 100))
                .RuleFor(a => a.Nome, f => f.Person.FullName)
                .Generate();
        }

        private AutorDeleteDto GenerateAutorDeleteDto()
        {
            return new Faker<AutorDeleteDto>()
                .RuleFor(a => a.CodAu, f => f.Random.Int(1, 100))
                .Generate();
        }

        private Autor GenerateAutor()
        {
            return new Faker<Autor>()
                .RuleFor(a => a.CodAu, f => f.Random.Int(1, 100))
                .RuleFor(a => a.Nome, f => f.Person.FullName)
                .Generate();
        }

        private AutorResponseDto GenerateAutorResponseDto()
        {
            return new Faker<AutorResponseDto>()
                .RuleFor(a => a.CodAu, f => f.Random.Int(1, 100))
                .RuleFor(a => a.Nome, f => f.Person.FullName)
                .Generate();
        }

        [Fact(DisplayName = "Adicionar Autor com sucesso")]
        public async Task AddAsync_ShouldAddAutor_WhenValid()
        {
            // Arrange
            var autorInsertDto = GenerateAutorInsertDto();
            var autor = GenerateAutor();
            var autorResponseDto = GenerateAutorResponseDto();

            _mapperMock.Setup(m => m.Map<Autor>(autorInsertDto)).Returns(autor);
            _autorDomainServiceMock.Setup(s => s.AddAsync(autor));
            _mapperMock.Setup(m => m.Map<AutorResponseDto>(autor)).Returns(autorResponseDto);

            // Act
            var result = await _autorAppService.AddAsync(autorInsertDto);

            // Assert
            result.Should().BeEquivalentTo(autorResponseDto);
        }

        [Fact(DisplayName = "Adicionar Autor deve falhar quando dados são inválidos")]
        public async Task AddAsync_ShouldThrowValidationException_WhenInvalidData()
        {
            // Arrange
            var autorInsertDto = new AutorInsertDto { Nome = "" }; // Dados inválidos
            var autor = GenerateAutor();

            _mapperMock.Setup(m => m.Map<Autor>(autorInsertDto)).Returns(autor);
            _autorDomainServiceMock.Setup(s => s.AddAsync(autor)).ThrowsAsync(new ValidationException("Nome é obrigatório"));

            // Act
            Func<Task> act = async () => await _autorAppService.AddAsync(autorInsertDto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Nome é obrigatório");
        }

        [Fact(DisplayName = "Atualizar Autor com sucesso")]
        public async Task UpdateAsync_ShouldUpdateAutor_WhenValid()
        {
            // Arrange
            var autorUpdateDto = GenerateAutorUpdateDto();
            var autor = GenerateAutor();
            var autorResponseDto = GenerateAutorResponseDto();

            _mapperMock.Setup(m => m.Map<Autor>(autorUpdateDto)).Returns(autor);
            _autorDomainServiceMock.Setup(s => s.UpdateAsync(autor));
            _mapperMock.Setup(m => m.Map<AutorResponseDto>(autor)).Returns(autorResponseDto);

            // Act
            var result = await _autorAppService.UpdateAsync(autorUpdateDto);

            // Assert
            result.Should().BeEquivalentTo(autorResponseDto);
        }

        [Fact(DisplayName = "Atualizar Autor deve falhar quando não encontrado")]
        public async Task UpdateAsync_ShouldThrowAutorNotFoundException_WhenAutorNotFound()
        {
            // Arrange
            var autorUpdateDto = GenerateAutorUpdateDto();
            var autor = GenerateAutor();

            _mapperMock.Setup(m => m.Map<Autor>(autorUpdateDto)).Returns(autor);
            _autorDomainServiceMock.Setup(s => s.UpdateAsync(autor)).ThrowsAsync(new NotFoundExceptionAutor(autor.CodAu));

            // Act
            Func<Task> act = async () => await _autorAppService.UpdateAsync(autorUpdateDto);

            // Assert
            await act.Should().ThrowAsync<NotFoundExceptionAutor>().WithMessage($"Autor {autor.CodAu} não encontrado.");
        }

        [Fact(DisplayName = "Excluir Autor com sucesso")]
        public async Task DeleteAsync_ShouldDeleteAutor_WhenAutorExists()
        {
            // Arrange
            var autorDeleteDto = GenerateAutorDeleteDto();
            var autor = GenerateAutor();
            var autorResponseDto = GenerateAutorResponseDto();

            _mapperMock.Setup(m => m.Map<Autor>(autorDeleteDto)).Returns(autor);
            _autorDomainServiceMock.Setup(s => s.DeleteAsync(autor));
            _mapperMock.Setup(m => m.Map<AutorResponseDto>(autor)).Returns(autorResponseDto);

            // Act
            var result = await _autorAppService.DeleteAsync(autorDeleteDto);

            // Assert
            result.Should().BeEquivalentTo(autorResponseDto);
        }

        [Fact(DisplayName = "Excluir Autor deve falhar quando não encontrado")]
        public async Task DeleteAsync_ShouldThrowAutorNotFoundException_WhenAutorNotFound()
        {
            // Arrange
            var autorDeleteDto = GenerateAutorDeleteDto();
            var autor = GenerateAutor();

            _mapperMock.Setup(m => m.Map<Autor>(autorDeleteDto)).Returns(autor);
            _autorDomainServiceMock.Setup(s => s.DeleteAsync(autor)).ThrowsAsync(new NotFoundExceptionAutor(autor.CodAu));

            // Act
            Func<Task> act = async () => await _autorAppService.DeleteAsync(autorDeleteDto);

            // Assert
            await act.Should().ThrowAsync<NotFoundExceptionAutor>().WithMessage($"Autor {autor.CodAu} não encontrado.");
        }

        [Fact(DisplayName = "Obter todos os autores com sucesso")]
        public async Task GetAllAsync_ShouldReturnAutores_WhenAutoresExist()
        {
            // Arrange
            var autores = new List<Autor> { GenerateAutor(), GenerateAutor() };
            var autoresResponseDto = new List<AutorResponseDto> { GenerateAutorResponseDto(), GenerateAutorResponseDto() };

            _autorDomainServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(autores);
            _mapperMock.Setup(m => m.Map<List<AutorResponseDto>>(autores)).Returns(autoresResponseDto);

            // Act
            var result = await _autorAppService.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(autoresResponseDto);
        }

        [Fact(DisplayName = "Obter Autor por ID com sucesso")]
        public async Task GetByIdAsync_ShouldReturnAutor_WhenAutorExists()
        {
            // Arrange
            var autor = GenerateAutor();
            var autorResponseDto = GenerateAutorResponseDto();

            _autorDomainServiceMock.Setup(s => s.GetByIdAsync(autor.CodAu)).ReturnsAsync(autor);
            _mapperMock.Setup(m => m.Map<AutorResponseDto>(autor)).Returns(autorResponseDto);

            // Act
            var result = await _autorAppService.GetByIdAsync(autor.CodAu);

            // Assert
            result.Should().BeEquivalentTo(autorResponseDto);
        }

        [Fact(DisplayName = "Obter Autor por ID deve retornar null quando Autor não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenAutorNotFound()
        {

            var autorId = new Faker().Random.Int();

            _autorDomainServiceMock.Setup(s => s.GetByIdAsync(autorId)).ReturnsAsync((Autor)null);

    
            var result = await _autorAppService.GetByIdAsync(autorId);

      
            result.Should().BeNull();
        }
    }
}
