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
    public class LivroAppServiceTest
    {
        private readonly Mock<ILivroDomainService> _livroDomainServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly LivroAppService _livroAppService;

        public LivroAppServiceTest()
        {
            _livroDomainServiceMock = new Mock<ILivroDomainService>();
            _mapperMock = new Mock<IMapper>();
            _livroAppService = new LivroAppService(_mapperMock.Object, _livroDomainServiceMock.Object);
        }

        private LivroInsertDto GenerateLivroInsertDto()
        {
            return new Faker<LivroInsertDto>()
                .RuleFor(l => l.Titulo, f => f.Lorem.Sentence())
                .RuleFor(l => l.Editora, f => f.Company.CompanyName())
                .RuleFor(l => l.Edicao, f => f.Random.Int(1, 10))
                .RuleFor(l => l.AnoPublicacao, f => f.Date.Past(20).Year.ToString())
                .Generate();
        }

        private LivroUpdateDto GenerateLivroUpdateDto()
        {
            return new Faker<LivroUpdateDto>()
                .RuleFor(l => l.Codl, f => f.Random.Int())
                .RuleFor(l => l.Titulo, f => f.Lorem.Sentence())
                .RuleFor(l => l.Editora, f => f.Company.CompanyName())
                .RuleFor(l => l.Edicao, f => f.Random.Int(1, 10))
                .RuleFor(l => l.AnoPublicacao, f => f.Date.Past(20).Year.ToString())
                .Generate();
        }

        private LivroDeleteDto GenerateLivroDeleteDto()
        {
            return new Faker<LivroDeleteDto>()
                .RuleFor(l => l.Codl, f => f.Random.Int())
                .Generate();
        }

        private Livro GenerateLivro()
        {
            return new Faker<Livro>()
                .RuleFor(l => l.Codl, f => f.Random.Int())
                .RuleFor(l => l.Titulo, f => f.Lorem.Sentence())
                .RuleFor(l => l.Editora, f => f.Company.CompanyName())
                .RuleFor(l => l.Edicao, f => f.Random.Int(1, 10))
                .RuleFor(l => l.AnoPublicacao, f => f.Date.Past(20).Year.ToString())
                .Generate();
        }

        private LivroResponseDto GenerateLivroResponseDto()
        {
            return new Faker<LivroResponseDto>()
                .RuleFor(l => l.Codl, f => f.Random.Int())
                .RuleFor(l => l.Titulo, f => f.Lorem.Sentence())
                .RuleFor(l => l.Editora, f => f.Company.CompanyName())
                .RuleFor(l => l.Edicao, f => f.Random.Int(1, 10))
                .RuleFor(l => l.AnoPublicacao, f => f.Date.Past(20).Year.ToString())
                .Generate();
        }

        [Fact(DisplayName = "Adicionar Livro com sucesso")]
        public async Task AddAsync_ShouldAddLivro_WhenValid()
        {
            // Arrange
            var livroInsertDto = GenerateLivroInsertDto();
            var livro = GenerateLivro();
            var livroResponseDto = GenerateLivroResponseDto();

            _mapperMock.Setup(m => m.Map<Livro>(livroInsertDto)).Returns(livro);
            _livroDomainServiceMock.Setup(s => s.AddAsync(livro));
            _mapperMock.Setup(m => m.Map<LivroResponseDto>(livro)).Returns(livroResponseDto);

            // Act
            var result = await _livroAppService.AddAsync(livroInsertDto);

            // Assert
            result.Should().BeEquivalentTo(livroResponseDto);
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando dados são inválidos")]
        public async Task AddAsync_ShouldThrowValidationException_WhenInvalidData()
        {
            // Arrange
            var livroInsertDto = new LivroInsertDto { Titulo = "" }; // Dados inválidos
            var livro = GenerateLivro();

            _mapperMock.Setup(m => m.Map<Livro>(livroInsertDto)).Returns(livro);
            _livroDomainServiceMock.Setup(s => s.AddAsync(livro)).ThrowsAsync(new ValidationException("Título é obrigatório"));

            // Act
            Func<Task> act = async () => await _livroAppService.AddAsync(livroInsertDto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Título é obrigatório");
        }

        [Fact(DisplayName = "Atualizar Livro com sucesso")]
        public async Task UpdateAsync_ShouldUpdateLivro_WhenValid()
        {
            // Arrange
            var livroUpdateDto = GenerateLivroUpdateDto();
            var livro = GenerateLivro();
            var livroResponseDto = GenerateLivroResponseDto();

            _mapperMock.Setup(m => m.Map<Livro>(livroUpdateDto)).Returns(livro);
            _livroDomainServiceMock.Setup(s => s.UpdateAsync(livro));
            _mapperMock.Setup(m => m.Map<LivroResponseDto>(livro)).Returns(livroResponseDto);

            // Act
            var result = await _livroAppService.UpdateAsync(livroUpdateDto);

            // Assert
            result.Should().BeEquivalentTo(livroResponseDto);
        }

        [Fact(DisplayName = "Atualizar Livro deve falhar quando não encontrado")]
        public async Task UpdateAsync_ShouldThrowLivroNotFoundException_WhenLivroNotFound()
        {
            // Arrange
            var livroUpdateDto = GenerateLivroUpdateDto();
            var livro = GenerateLivro();

            _mapperMock.Setup(m => m.Map<Livro>(livroUpdateDto)).Returns(livro);
            _livroDomainServiceMock.Setup(s => s.UpdateAsync(livro)).ThrowsAsync(new NotFoundExceptionLivro(livro.Codl));

            // Act
            Func<Task> act = async () => await _livroAppService.UpdateAsync(livroUpdateDto);

            // Assert
            await act.Should().ThrowAsync<NotFoundExceptionLivro>().WithMessage($"Livro {livro.Codl} não encontrado.");
        }

        [Fact(DisplayName = "Excluir Livro com sucesso")]
        public async Task DeleteAsync_ShouldDeleteLivro_WhenLivroExists()
        {
            // Arrange
            var livroDeleteDto = GenerateLivroDeleteDto();
            var livro = GenerateLivro();
            var livroResponseDto = GenerateLivroResponseDto();

            _mapperMock.Setup(m => m.Map<Livro>(livroDeleteDto)).Returns(livro);
            _livroDomainServiceMock.Setup(s => s.DeleteAsync(livro));
            _mapperMock.Setup(m => m.Map<LivroResponseDto>(livro)).Returns(livroResponseDto);

            // Act
            var result = await _livroAppService.DeleteAsync(livroDeleteDto);

            // Assert
            result.Should().BeEquivalentTo(livroResponseDto);
        }

        [Fact(DisplayName = "Excluir Livro deve falhar quando não encontrado")]
        public async Task DeleteAsync_ShouldThrowLivroNotFoundException_WhenLivroNotFound()
        {
            // Arrange
            var livroDeleteDto = GenerateLivroDeleteDto();
            var livro = GenerateLivro();

            _mapperMock.Setup(m => m.Map<Livro>(livroDeleteDto)).Returns(livro);
            _livroDomainServiceMock.Setup(s => s.DeleteAsync(livro)).ThrowsAsync(new NotFoundExceptionLivro(livro.Codl));

            // Act
            Func<Task> act = async () => await _livroAppService.DeleteAsync(livroDeleteDto);

            // Assert
            await act.Should().ThrowAsync<NotFoundExceptionLivro>().WithMessage($"Livro {livro.Codl} não encontrado.");
        }

        [Fact(DisplayName = "Obter todos os livros com sucesso")]
        public async Task GetAllAsync_ShouldReturnLivros_WhenLivrosExist()
        {
            // Arrange
            var livros = new List<Livro> { GenerateLivro(), GenerateLivro() };
            var livrosResponseDto = new List<LivroResponseDto> { GenerateLivroResponseDto(), GenerateLivroResponseDto() };

            _livroDomainServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(livros);
            _mapperMock.Setup(m => m.Map<List<LivroResponseDto>>(livros)).Returns(livrosResponseDto);

            // Act
            var result = await _livroAppService.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(livrosResponseDto);
        }

        [Fact(DisplayName = "Obter livro por ID com sucesso")]
        public async Task GetByIdAsync_ShouldReturnLivro_WhenLivroExists()
        {
            // Arrange
            var livro = GenerateLivro();
            var livroResponseDto = GenerateLivroResponseDto();

            _livroDomainServiceMock.Setup(s => s.GetByIdAsync(livro.Codl)).ReturnsAsync(livro);
            _mapperMock.Setup(m => m.Map<LivroResponseDto>(livro)).Returns(livroResponseDto);

            // Act
            var result = await _livroAppService.GetByIdAsync(livro.Codl);

            // Assert
            result.Should().BeEquivalentTo(livroResponseDto);
        }

        [Fact(DisplayName = "Obter livro por ID deve retornar null quando livro não encontrado")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenLivroNotFound()
        {
            // Arrange
            var livroId = new Faker().Random.Int();

            _livroDomainServiceMock.Setup(s => s.GetByIdAsync(livroId)).ReturnsAsync((Livro)null);

            // Act
            var result = await _livroAppService.GetByIdAsync(livroId);

            // Assert
            result.Should().BeNull();
        }
    }
}
