using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BibliotecaApp.API.Tests.Base;
using BibliotecaApp.API.Tests.Validations;
using BibliotecaApp.Aplication.Dtos;
using FluentAssertions;
using Xunit;

namespace BibliotecaApp.API.Tests.Validations
{
    public class AutorControllerValidationTest
    {
        private readonly AutorControllerTestBase _testBase;

        public AutorControllerValidationTest()
        {
            _testBase = new AutorControllerTestBase();
        }

        [Fact(DisplayName = "Verificar se a rota /api/autor está acessível")]
        public async Task Route_ShouldBeAccessible()
        {
            var response = await _testBase.GetAllAutoresAsync();
            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Adicionar Autor com sucesso")]
        public async Task Post_ShouldAddAutor_WhenValid()
        {
            var validAutor = _testBase.CreateValidAutor();

            var response = await _testBase.AddAutorAsync(validAutor);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<AutorResponseDto>();
            result.Should().NotBeNull();
            result.Nome.Should().Be(validAutor.Nome);
        }

        [Fact(DisplayName = "Adicionar Autor deve falhar quando Nome estiver vazio")]
        public async Task Post_ShouldFail_WhenNomeIsEmpty()
        {
            var invalidAutor = _testBase.CreateInvalidAutor();

            var response = await _testBase.AddAutorAsync(invalidAutor);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
            result.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.Field == "Nome" && e.ErrorMessage == "O nome do autor é obrigatório.");
        }

        [Fact(DisplayName = "Atualizar Autor com sucesso")]
        public async Task Put_ShouldUpdateAutor_WhenValid()
        {
            var validAutor = _testBase.CreateValidAutor();
            var addResponse = await _testBase.AddAutorAsync(validAutor);
            var addedAutor = await addResponse.Content.ReadFromJsonAsync<AutorResponseDto>();

            var validUpdate = _testBase.CreateValidAutorUpdate(addedAutor.CodAu);
            var response = await _testBase.UpdateAutorAsync(validUpdate);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AutorResponseDto>();
            result.Should().NotBeNull();
            result.Nome.Should().Be(validUpdate.Nome);
        }

        [Fact(DisplayName = "Atualizar Autor deve falhar quando Nome estiver vazio")]
        public async Task Put_ShouldFail_WhenNomeIsEmpty()
        {
            var validAutor = _testBase.CreateValidAutor();
            var addResponse = await _testBase.AddAutorAsync(validAutor);
            var addedAutor = await addResponse.Content.ReadFromJsonAsync<AutorResponseDto>();

            var invalidUpdate = _testBase.CreateInvalidAutorUpdate(addedAutor.CodAu);
            var response = await _testBase.UpdateAutorAsync(invalidUpdate);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
            result.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.Field == "Nome" && e.ErrorMessage == "O nome do autor é obrigatório.");
        }

        [Fact(DisplayName = "Excluir Autor com sucesso")]
        public async Task Delete_ShouldRemoveAutor_WhenAutorExists()
        {
            var validAutor = _testBase.CreateValidAutor();
            var addResponse = await _testBase.AddAutorAsync(validAutor);
            var addedAutor = await addResponse.Content.ReadFromJsonAsync<AutorResponseDto>();

            var validDelete = _testBase.CreateValidAutorDelete(addedAutor.CodAu);
            var response = await _testBase.DeleteAutorAsync(validDelete);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(DisplayName = "Obter Autor por ID com sucesso")]
        public async Task GetById_ShouldReturnAutor_WhenAutorExists()
        {
            var validAutor = _testBase.CreateValidAutor();
            var addResponse = await _testBase.AddAutorAsync(validAutor);
            var addedAutor = await addResponse.Content.ReadFromJsonAsync<AutorResponseDto>();

            var response = await _testBase.GetAutorByIdAsync(addedAutor.CodAu);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AutorResponseDto>();
            result.Should().NotBeNull();
            result.CodAu.Should().Be(addedAutor.CodAu);
            result.Nome.Should().Be(addedAutor.Nome);
        }
        [Fact(DisplayName = "Verificar se os controladores estão registrados corretamente")]
        public void ShouldRegisterControllers()
        {
            var controllers = _testBase.ShouldRegisterControllers();
            controllers.Should().Contain(c => c.Name == "AutorController");
        }

    }
}
