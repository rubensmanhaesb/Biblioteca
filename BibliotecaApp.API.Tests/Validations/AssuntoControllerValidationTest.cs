using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BibliotecaApp.API.Tests.Base;
using BibliotecaApp.Aplication.Dtos;
using FluentAssertions;
using Xunit;

namespace BibliotecaApp.API.Tests.Validations
{
    public class AssuntoControllerValidationTest
    {
        private readonly AssuntoControllerTestBase _testBase;

        public AssuntoControllerValidationTest()
        {
            _testBase = new AssuntoControllerTestBase();
        }

        [Fact(DisplayName = "Verificar se a rota /api/assunto está acessível")]
        public async Task Route_ShouldBeAccessible()
        {
            var response = await _testBase.GetAllAssuntosAsync();
            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Adicionar Assunto com sucesso")]
        public async Task Post_ShouldAddAssunto_WhenValid()
        {
            var validAssunto = _testBase.CreateValidAssunto();

            var response = await _testBase.AddAssuntoAsync(validAssunto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<AssuntoResponseDto>();
            result.Should().NotBeNull();
            result.Descricao.Should().Be(validAssunto.Descricao);
        }

        [Fact(DisplayName = "Adicionar Assunto deve falhar quando Descrição estiver vazia")]
        public async Task Post_ShouldFail_WhenDescricaoIsEmpty()
        {
            var invalidAssunto = _testBase.CreateInvalidAssunto();

            var response = await _testBase.AddAssuntoAsync(invalidAssunto);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
            result.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.Field == "Descricao" && e.ErrorMessage == "A descrição do assunto é obrigatória.");
        }

        [Fact(DisplayName = "Atualizar Assunto com sucesso")]
        public async Task Put_ShouldUpdateAssunto_WhenValid()
        {
            var validAssunto = _testBase.CreateValidAssunto();
            var addResponse = await _testBase.AddAssuntoAsync(validAssunto);
            var addedAssunto = await addResponse.Content.ReadFromJsonAsync<AssuntoResponseDto>();

            var validUpdate = _testBase.CreateValidAssuntoUpdate(addedAssunto.CodAs);
            var response = await _testBase.UpdateAssuntoAsync(validUpdate);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AssuntoResponseDto>();
            result.Should().NotBeNull();
            result.Descricao.Should().Be(validUpdate.Descricao);
        }

        [Fact(DisplayName = "Excluir Assunto com sucesso")]
        public async Task Delete_ShouldRemoveAssunto_WhenAssuntoExists()
        {
            var validAssunto = _testBase.CreateValidAssunto();
            var addResponse = await _testBase.AddAssuntoAsync(validAssunto);
            var addedAssunto = await addResponse.Content.ReadFromJsonAsync<AssuntoResponseDto>();

            var validDelete = _testBase.CreateValidAssuntoDelete(addedAssunto.CodAs);
            var response = await _testBase.DeleteAssuntoAsync(validDelete);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(DisplayName = "Obter Assunto por ID com sucesso")]
        public async Task GetById_ShouldReturnAssunto_WhenAssuntoExists()
        {
            var validAssunto = _testBase.CreateValidAssunto();
            var addResponse = await _testBase.AddAssuntoAsync(validAssunto);
            var addedAssunto = await addResponse.Content.ReadFromJsonAsync<AssuntoResponseDto>();

            var response = await _testBase.GetAssuntoByIdAsync(addedAssunto.CodAs);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AssuntoResponseDto>();
            result.Should().NotBeNull();
            result.Descricao.Should().Be(addedAssunto.Descricao);
        }
        [Fact(DisplayName = "Verificar se os controladores estão registrados corretamente")]
        public void ShouldRegisterControllers()
        {
            var controllers = _testBase.ShouldRegisterControllers();
            controllers.Should().Contain(c => c.Name == "AssuntoController");
        }

    }
}
