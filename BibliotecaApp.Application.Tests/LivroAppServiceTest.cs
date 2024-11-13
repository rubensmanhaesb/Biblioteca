
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
    public class LivroAppServiceTest
    {
        private readonly IMapper _mapper;
        private readonly LivroAppService _livroAppService;
        private readonly DataContext _context;
        private readonly LivroDomainService _livroDomainService;

        public LivroAppServiceTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BibliotecaAppTest")
                .Options;

            _context = new DataContext(options, new LoggerFactory().CreateLogger<DataContext>());
            var LivroRepository = new LivroRepository(_context);
            var unitOfWork = new UnitOfWork(_context);
            _livroDomainService = new LivroDomainService(unitOfWork);

            var config = new MapperConfiguration(cfg => cfg.AddProfile(new LivroMapping()));
            _mapper = config.CreateMapper();
            _livroAppService = new LivroAppService(_mapper, _livroDomainService);
        }

        private LivroInsertDto GeneratelivroInsertDto() => new LivroInsertDto
        {
            Titulo = "Titulo Teste",
            Edicao = 1,
            Editora = "Editora Teste",
            AnoPublicacao = "2021"
        };

        private LivroUpdateDto GenerateLivroUpdateDto(int codl) => new LivroUpdateDto
        {
            Codl = codl,
            Titulo = "Alteração Titulo Teste",
            Edicao = 4,
            Editora = "Alteração Editora Teste",
            AnoPublicacao = "2024"
        };

        private LivroDeleteDto GenerateLivroDeleteDto(int codl) => new LivroDeleteDto
        {
            Codl = codl
        };

        [Fact(DisplayName = "Adicionar Livro com sucesso")]
        public async Task AddAsync_ShouldAddLivro_WhenValid()
        {
            var livroInsertDto = GeneratelivroInsertDto();
            var result = await _livroAppService.AddAsync(livroInsertDto);

            result.Should().NotBeNull();
            result.Titulo.Should().Be(livroInsertDto.Titulo);
            result.Editora.Should().Be(livroInsertDto.Editora);
            result.Edicao.Should().Be(livroInsertDto.Edicao);
            result.Codl.Should().BeGreaterThan(0);
        }

        [Fact(DisplayName = "Atualizar Livro com sucesso")]
        public async Task UpdateAsync_ShouldUpdateLivro_WhenValid()
        {
            var livroInsertDto = GeneratelivroInsertDto();
            var addedLivro = await _livroAppService.AddAsync(livroInsertDto);
            var LivroUpdateDto = GenerateLivroUpdateDto(addedLivro.Codl);

            var result = await _livroAppService.UpdateAsync(LivroUpdateDto);

            result.Should().NotBeNull();
            result.Codl.Should().Be(LivroUpdateDto.Codl);
            result.Titulo.Should().Be(LivroUpdateDto.Titulo);
            result.Edicao.Should().Be(LivroUpdateDto.Edicao);
            result.Editora.Should().Be(LivroUpdateDto.Editora);
            result.AnoPublicacao.Should().Be(LivroUpdateDto.AnoPublicacao);

        }

        [Fact(DisplayName = "Excluir Livro com sucesso")]
        public async Task DeleteAsync_ShouldDeleteLivro_WhenLivroExists()
        {
            var livroInsertDto = GeneratelivroInsertDto();
            var addedLivro = await _livroAppService.AddAsync(livroInsertDto);
            var LivroDeleteDto = GenerateLivroDeleteDto(addedLivro.Codl);

            var result = await _livroAppService.DeleteAsync(LivroDeleteDto);

            result.Should().NotBeNull();
            result.Codl.Should().Be(addedLivro.Codl);
            result.Titulo.Should().Be(addedLivro.Titulo);
            result.Edicao.Should().Be(addedLivro.Edicao);
            result.Editora.Should().Be(addedLivro.Editora);
            result.AnoPublicacao.Should().Be(addedLivro.AnoPublicacao);

        }

        [Fact(DisplayName = "Obter Livro por ID com sucesso")]
        public async Task GetByIdAsync_ShouldReturnLivro_WhenLivroExists()
        {
            var livroInsertDto = GeneratelivroInsertDto();
            var addedLivro = await _livroAppService.AddAsync(livroInsertDto);

            var result = await _livroAppService.GetByIdAsync(addedLivro.Codl);

            result.Should().NotBeNull();
            result.Codl.Should().Be(addedLivro.Codl);
            result.Titulo.Should().Be(addedLivro.Titulo);
            result.Edicao.Should().Be(addedLivro.Edicao);
            result.Editora.Should().Be(addedLivro.Editora);
            result.AnoPublicacao.Should().Be(addedLivro.AnoPublicacao);

        }

        [Fact(DisplayName = "Obter todos os Livros com sucesso")]
        public async Task GetAllAsync_ShouldReturnAllLivros_WhenLivrosExist()
        {
            await _livroAppService.AddAsync(GeneratelivroInsertDto());
            await _livroAppService.AddAsync(GeneratelivroInsertDto());

            var result = await _livroAppService.GetAllAsync();

            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(1);
        }



        [Fact(DisplayName = "Adicionar Livro deve falhar quando o titulo estiver vazio")]
        public async Task AddAsync_ShouldThrowValidationException_WhenTituloIsEmpty()
        {
            var invalidlivroInsertDto = GeneratelivroInsertDto();
            invalidlivroInsertDto.Titulo = "" ;

            Func<Task> act = async () => await _livroAppService.AddAsync(invalidlivroInsertDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*O título do Livro é obrigatório.*");
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando a edicção estiver vazio")]
        public async Task AddAsync_ShouldThrowValidationException_WhenEdicaoIsEmpty()
        {
            var invalidlivroInsertDto = GeneratelivroInsertDto();
            invalidlivroInsertDto.Edicao = 0;

            Func<Task> act = async () => await _livroAppService.AddAsync(invalidlivroInsertDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*A Edição não pode ser zero ou negativa.*");
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando a editora estiver vazia")]
        public async Task AddAsync_ShouldThrowValidationException_WhenEditoraIsEmpty()
        {
            var invalidlivroInsertDto = GeneratelivroInsertDto();
            invalidlivroInsertDto.Editora= "";

            Func<Task> act = async () => await _livroAppService.AddAsync(invalidlivroInsertDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*A Editora do Livro é obrigatória.*");
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando o ano publicação estiver vazio")]
        public async Task AddAsync_ShouldThrowValidationException_WhenAnoPublicacaoIsEmpty()
        {
            var invalidlivroInsertDto = GeneratelivroInsertDto();
            invalidlivroInsertDto.AnoPublicacao = "";

            Func<Task> act = async () => await _livroAppService.AddAsync(invalidlivroInsertDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*A Publicação deve ter exatamente 4 caracteres.*");
        }


        [Fact(DisplayName = "Atualizar Livro deve falhar quando o titulo estiver vazia")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenTituloIsEmpty()
        {
            var livroInsertDto = GeneratelivroInsertDto();
            var addedLivro = await _livroAppService.AddAsync(livroInsertDto);

            var invalidLivroUpdateDto = GenerateLivroUpdateDto(addedLivro.Codl);
            invalidLivroUpdateDto.Titulo = "";

            Func<Task> act = async () => await _livroAppService.UpdateAsync(invalidLivroUpdateDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*O título do Livro é obrigatório.*");
        }

        [Fact(DisplayName = "Atualizar Livro deve falhar quando a editora estiver vazia")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenEditoraIsEmpty()
        {
            var livroInsertDto = GeneratelivroInsertDto();
            var addedLivro = await _livroAppService.AddAsync(livroInsertDto);

            var invalidLivroUpdateDto = GenerateLivroUpdateDto(addedLivro.Codl);
            invalidLivroUpdateDto.Editora = "";

            Func<Task> act = async () => await _livroAppService.UpdateAsync(invalidLivroUpdateDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*A Editora do livro é obrigatória*");
        }

        [Fact(DisplayName = "Atualizar Livro deve falhar quando o ano publicação estiver vazia")]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenAnoPublicacaoIsEmpty()
        {
            var livroInsertDto = GeneratelivroInsertDto();
            var addedLivro = await _livroAppService.AddAsync(livroInsertDto);

            var invalidLivroUpdateDto = GenerateLivroUpdateDto(addedLivro.Codl);
            invalidLivroUpdateDto.AnoPublicacao = "";

            Func<Task> act = async () => await _livroAppService.UpdateAsync(invalidLivroUpdateDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*A Publicação deve ter exatamente 4 caracteres.*");
        }



        [Fact(DisplayName = "Atualizar Livro deve falhar quando o código do Livro não existe")]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenLivroDoesNotExist()
        {
            var invalidLivroUpdateDto = GenerateLivroUpdateDto(9999);

            Func<Task> act = async () => await _livroAppService.UpdateAsync(invalidLivroUpdateDto);

            await act.Should().ThrowAsync<NotFoundExceptionLivro>()
                .WithMessage($"Livro {invalidLivroUpdateDto.Codl} não encontrado.");
        }

        [Fact(DisplayName = "Excluir Livro deve falhar quando o código do Livro não existe")]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenLivroDoesNotExist()
        {
            var invalidLivroDeleteDto = new LivroDeleteDto { Codl = 9999 };

            Func<Task> act = async () => await _livroAppService.DeleteAsync(invalidLivroDeleteDto);

            await act.Should().ThrowAsync<NotFoundExceptionLivro>()
                .WithMessage($"Livro {invalidLivroDeleteDto.Codl} não encontrado.");
        }
    }
}
