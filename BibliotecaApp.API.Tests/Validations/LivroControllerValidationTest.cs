using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BibliotecaApp.API.Controllers;
using BibliotecaApp.API.Tests.Base;
using BibliotecaApp.Aplication.Dtos;
using FluentAssertions;
using Xunit;

namespace BibliotecaApp.API.Tests.Validations
{
    public class LivroControllerValidationTest
    {
        private readonly LivroControllerTestBase _testBase;

        public LivroControllerValidationTest()
        {
            _testBase = new LivroControllerTestBase();
        }

        [Fact(DisplayName = "Verificar se a rota /api/livro está acessível")]
        public async Task Route_ShouldBeAccessible()
        {
            var response = await _testBase.GetAllLivrosAsync();
            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Verificar se os controladores estão registrados corretamente")]
        public void ShouldRegisterControllers()
        {
            var controllers = _testBase.ShouldRegisterControllers();
            controllers.Should().Contain(c => c.Name == "LivroController");
        }

        [Fact(DisplayName = "Adicionar Livro com sucesso")]
        public async Task Post_ShouldAddLivro_WhenValid()
        {
            var validLivro = _testBase.CreateValidLivro();

            var response = await _testBase.AddLivroAsync(validLivro);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<LivroResponseDto>();
            result.Should().NotBeNull();
            result.Titulo.Should().Be(validLivro.Titulo);
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando título estiver vazio")]
        public async Task Post_ShouldFail_WhenTituloIsEmpty()
        {
            var invalidLivro = _testBase.CreateInvalidLivroTitulo();

            var response = await _testBase.AddLivroAsync(invalidLivro);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
            result.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.Field == "Titulo" && e.ErrorMessage.Contains("O título do livro é obrigatório."));
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando editora estiver vazia")]
        public async Task Post_ShouldFail_WhenEditoraIsEmpty()
        {
            var invalidLivro = new LivroInsertDto
            {
                Titulo = "Livro Teste",
                Editora = "", // Inválido
                Edicao = 1,
                AnoPublicacao = "2023"
            };

            var response = await _testBase.AddLivroAsync(invalidLivro);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
            result.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.Field == "Editora" && e.ErrorMessage.Contains("A Editora do livro é obrigatória."));
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando edição for zero ou negativa")]
        public async Task Post_ShouldFail_WhenEdicaoIsInvalid()
        {
            var invalidLivro = _testBase.CreateInvalidLivroEdicao();

            var response = await _testBase.AddLivroAsync(invalidLivro);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
            result.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.Field == "Edicao" && e.ErrorMessage.Contains("A Edição não pode ser zero ou negativa."));
        }

        [Fact(DisplayName = "Adicionar Livro deve falhar quando ano de publicação estiver vazio")]
        public async Task Post_ShouldFail_WhenAnoPublicacaoIsEmpty()
        {
            var invalidLivro = new LivroInsertDto
            {
                Titulo = "Livro Teste",
                Editora = "Editora Teste",
                Edicao = 1,
                AnoPublicacao = "" // Inválido
            };

            var response = await _testBase.AddLivroAsync(invalidLivro);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
            result.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.Field == "AnoPublicacao" && e.ErrorMessage.Contains("A Publicação do livro é obrigatória."));
        }

        [Fact(DisplayName = "Atualizar Livro com sucesso")]
        public async Task Put_ShouldUpdateLivro_WhenValid()
        {
            var validLivro = _testBase.CreateValidLivro();
            var addResponse = await _testBase.AddLivroAsync(validLivro);
            var addedLivro = await addResponse.Content.ReadFromJsonAsync<LivroResponseDto>();

            var validUpdate = _testBase.CreateValidLivroUpdate(addedLivro.Codl);
            var response = await _testBase.UpdateLivroAsync(validUpdate);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<LivroResponseDto>();
            result.Should().NotBeNull();
            result.Titulo.Should().Be(validUpdate.Titulo);
        }

        [Fact(DisplayName = "Atualizar Livro deve falhar quando título estiver vazio")]
        public async Task Put_ShouldFail_WhenTituloIsEmpty()
        {
            var validLivro = _testBase.CreateValidLivro();
            var addResponse = await _testBase.AddLivroAsync(validLivro);
            var addedLivro = await addResponse.Content.ReadFromJsonAsync<LivroResponseDto>();

            var invalidUpdate = new LivroUpdateDto
            {
                Codl = addedLivro.Codl,
                Titulo = "", // Inválido
                Editora = "Editora Atualizada",
                Edicao = 2,
                AnoPublicacao = "2024"
            };

            var response = await _testBase.UpdateLivroAsync(invalidUpdate);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
            result.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.Field == "Titulo" && e.ErrorMessage.Contains("O título do livro é obrigatório."));
        }

        [Fact(DisplayName = "Atualizar Livro deve falhar quando editora estiver vazia")]
        public async Task Put_ShouldFail_WhenEditoraIsEmpty()
        {
            var validLivro = _testBase.CreateValidLivro();
            var addResponse = await _testBase.AddLivroAsync(validLivro);
            var addedLivro = await addResponse.Content.ReadFromJsonAsync<LivroResponseDto>();

            var invalidUpdate = new LivroUpdateDto
            {
                Codl = addedLivro.Codl,
                Titulo = "Livro Atualizado",
                Editora = "", // Inválido
                Edicao = 2,
                AnoPublicacao = "2024"
            };

            var response = await _testBase.UpdateLivroAsync(invalidUpdate);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
            result.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.Field == "Editora" && e.ErrorMessage.Contains("A Editora do livro é obrigatória."));
        }

        [Fact(DisplayName = "Atualizar Livro deve falhar quando edição for zero ou negativa")]
        public async Task Put_ShouldFail_WhenEdicaoIsInvalid()
        {
            var validLivro = _testBase.CreateValidLivro();
            var addResponse = await _testBase.AddLivroAsync(validLivro);
            var addedLivro = await addResponse.Content.ReadFromJsonAsync<LivroResponseDto>();

            var invalidUpdate = new LivroUpdateDto
            {
                Codl = addedLivro.Codl,
                Titulo = "Livro Atualizado",
                Editora = "Editora Atualizada",
                Edicao = 0, // Inválido
                AnoPublicacao = "2024"
            };

            var response = await _testBase.UpdateLivroAsync(invalidUpdate);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
            result.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.Field == "Edicao" && e.ErrorMessage.Contains("A Edição não pode ser zero ou negativa."));
        }

        [Fact(DisplayName = "Atualizar Livro deve falhar quando ano de publicação estiver vazio")]
        public async Task Put_ShouldFail_WhenAnoPublicacaoIsEmpty()
        {
            var validLivro = _testBase.CreateValidLivro();
            var addResponse = await _testBase.AddLivroAsync(validLivro);
            var addedLivro = await addResponse.Content.ReadFromJsonAsync<LivroResponseDto>();

            var invalidUpdate = new LivroUpdateDto
            {
                Codl = addedLivro.Codl,
                Titulo = "Livro Atualizado",
                Editora = "Editora Atualizada",
                Edicao = 2,
                AnoPublicacao = "" // Inválido
            };

            var response = await _testBase.UpdateLivroAsync(invalidUpdate);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
            result.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.Field == "AnoPublicacao" && e.ErrorMessage.Contains("A Publicação do livro é obrigatória."));
        }

        [Fact(DisplayName = "Excluir Livro deve falhar quando livro não encontrado")]
        public async Task Delete_ShouldFail_WhenLivroNotFound()
        {
            var deleteRequest = new LivroDeleteDto { Codl = 9999 }; // ID inexistente

            var response = await _testBase.DeleteLivroAsync(deleteRequest);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Obter todos os Livros com sucesso")]
        public async Task GetAll_ShouldReturnAllLivros()
        {
            await _testBase.AddLivroAsync(_testBase.CreateValidLivro());
            await _testBase.AddLivroAsync(_testBase.CreateValidLivro());

            var response = await _testBase.GetAllLivrosAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<List<LivroResponseDto>>();
            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(1);
        }

        [Fact(DisplayName = "Obter Livro por ID com sucesso")]
        public async Task GetById_ShouldReturnLivro_WhenLivroExists()
        {
            var validLivro = _testBase.CreateValidLivro();
            var addResponse = await _testBase.AddLivroAsync(validLivro);
            var addedLivro = await addResponse.Content.ReadFromJsonAsync<LivroResponseDto>();

            var response = await _testBase.GetLivroByIdAsync(addedLivro.Codl);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<LivroResponseDto>();
            result.Should().NotBeNull();
            result.Titulo.Should().Be(addedLivro.Titulo);
        }
    }
}
