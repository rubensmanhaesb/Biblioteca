using BibliotecaApp.API.Middlewares;
using BibliotecaApp.API.Controllers;
using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Interfaces;
using BibliotecaApp.Aplication.Mappings;
using BibliotecaApp.Aplication.Services;
using BibliotecaApp.Domain.Interfaces.Services;
using BibliotecaApp.Domain.Services;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Infra.Data.Context;
using BibliotecaApp.Infra.Data.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using BibliotecaApp.API.Tests.Validations;

namespace BibliotecaApp.API.Tests
{
    public class AssuntoControllerTest
    {
        private readonly HttpClient _client;

        public AssuntoControllerTest()
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<DataContext>(options =>
                        options.UseInMemoryDatabase("BibliotecaAppTest"));
                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                    services.AddScoped<IAssuntoRepository, AssuntoRepository>();
                    services.AddScoped<IAssuntoAppService, AssuntoAppService>();
                    services.AddScoped<IAssuntoDomainService, AssuntoDomainService>();
                    services.AddAutoMapper(cfg => cfg.AddProfile<AssuntoMapping>());
                    services.AddControllers().AddApplicationPart(typeof(AssuntoController).Assembly);
                    services.AddValidatorsFromAssemblyContaining<AssuntoInsertDto>(); // Adiciona validadores
                    services.AddValidatorsFromAssemblyContaining<AssuntoDeleteDto>();
                    services.AddValidatorsFromAssemblyContaining<AssuntoUpdateDto>();
                    services.AddValidatorsFromAssemblyContaining<AssuntoResponseDto>();

                })
                .Configure(app =>
                {
                    app.UseMiddleware<ExceptionMiddleware>();
                    app.UseMiddleware<CircuitBreakerMiddleware>();
                    app.UseMiddleware<RetryMiddleware>();
                    app.UseMiddleware<ValidationExceptionMiddleware>();
                    app.UseMiddleware<NotFoundExceptionMiddleware>();
                    app.UseMiddleware<RecordAlreadyExistsExceptionMiddleware>();
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                });

            var testServer = new TestServer(builder);
            _client = testServer.CreateClient();
        }
        [Fact(DisplayName = "Verificar se a rota /api/assunto est� acess�vel")]
        public async Task Route_ShouldBeAccessible()
        {
            var response = await _client.GetAsync("/api/assunto");
            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Verificar se os controladores est�o registrados corretamente")]
        public void ShouldRegisterControllers()
        {
            var controllers = typeof(AssuntoController).Assembly.GetExportedTypes();
            controllers.Should().Contain(c => c.Name == "AssuntoController");
        }

        // Teste de sucesso: Adicionar Assunto
        [Fact(DisplayName = "Adicionar Assunto com sucesso")]
        public async Task Post_ShouldAddAssunto_WhenValid()
        {
            var request = new AssuntoInsertDto { Descricao = "Novo Assunto" };

            var response = await _client.PostAsJsonAsync("/api/assunto", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<AssuntoResponseDto>();
            result.Should().NotBeNull();
            result.Descricao.Should().Be(request.Descricao);
        }

        // Teste de falha: Adicionar Assunto com descri��o vazia
        [Fact(DisplayName = "Adicionar Assunto deve falhar quando Descri��o estiver vazia")]
        public async Task Post_ShouldFail_WhenDescricaoIsEmpty()
        {
            // Cria o objeto de requisi��o com descri��o vazia
            var request = new AssuntoInsertDto { Descricao = "" };

            // Envia a requisi��o POST
            var response = await _client.PostAsJsonAsync("/api/assunto", request);

            // Verifica se o c�digo de status � BadRequest
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // L� e desserializa o conte�do da resposta
            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

            // Verifica se a resposta cont�m erros
            result.Should().NotBeNull();
            result.Errors.Should().NotBeNullOrEmpty();

            // Verifica se o erro esperado est� presente
            var descricaoError = result.Errors.FirstOrDefault(e =>
                e.Field == "Descricao" && e.ErrorMessage == "A descri��o do assunto � obrigat�ria.");

            descricaoError.Should().NotBeNull();
        }


        // Teste de sucesso: Atualizar Assunto
        [Fact(DisplayName = "Atualizar Assunto com sucesso")]
        public async Task Put_ShouldUpdateAssunto_WhenValid()
        {
            var addRequest = new AssuntoInsertDto { Descricao = "Assunto Original" };
            var addResponse = await _client.PostAsJsonAsync("/api/assunto", addRequest);
            var addedAssunto = await addResponse.Content.ReadFromJsonAsync<AssuntoResponseDto>();

            var updateRequest = new AssuntoUpdateDto { CodAs = addedAssunto.CodAs, Descricao = "Assunto Atualizado" };

            var response = await _client.PutAsJsonAsync("/api/assunto", updateRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AssuntoResponseDto>();
            result.Should().NotBeNull();
            result.Descricao.Should().Be(updateRequest.Descricao);
        }

        // Teste de falha: Atualizar Assunto inexistente
        [Fact(DisplayName = "Atualizar Assunto deve falhar quando o c�digo n�o existir")]
        public async Task Put_ShouldFail_WhenAssuntoDoesNotExist()
        {
            var updateRequest = new AssuntoUpdateDto { CodAs = 9999, Descricao = "Assunto Atualizado" };

            var response = await _client.PutAsJsonAsync("/api/assunto", updateRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Teste de falha: Atualizar Assunto deve falhar quando a descri��o n�o for informada
        [Fact(DisplayName = "Atualizar Assunto deve falhar quando Descri��o n�o for informada")]
        public async Task Put_ShouldFail_WhenDescricaoIsEmpty()
        {
            // Cria um assunto para ser alterado
            var addRequest = new AssuntoInsertDto { Descricao = "Assunto Original" };
            var addResponse = await _client.PostAsJsonAsync("/api/assunto", addRequest);
            var addedAssunto = await addResponse.Content.ReadFromJsonAsync<AssuntoResponseDto>();

            // Prepara a requisi��o de atualiza��o com descri��o vazia
            var updateRequest = new AssuntoUpdateDto { CodAs = addedAssunto.CodAs, Descricao = "" };

            // Envia a requisi��o PUT
            var response = await _client.PutAsJsonAsync("/api/assunto", updateRequest);

            // Verifica se o c�digo de status � BadRequest
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // L� e desserializa o conte�do da resposta
            var result = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

            // Verifica se a resposta cont�m erros
            result.Should().NotBeNull();
            result.Errors.Should().NotBeNullOrEmpty();

            // Verifica se o erro esperado est� presente
            var descricaoError = result.Errors.FirstOrDefault(e =>
                e.Field == "Descricao" && e.ErrorMessage == "A descri��o do assunto � obrigat�ria.");

            descricaoError.Should().NotBeNull();
        }

        // Teste de sucesso: Excluir Assunto
        [Fact(DisplayName = "Excluir Assunto com sucesso")]
        public async Task Delete_ShouldRemoveAssunto_WhenAssuntoExists()
        {
            // Cria um assunto para remover
            var addRequest = new AssuntoInsertDto { Descricao = "Assunto para Remo��o" };
            var addResponse = await _client.PostAsJsonAsync("/api/assunto", addRequest);
            var addedAssunto = await addResponse.Content.ReadFromJsonAsync<AssuntoResponseDto>();

            // Prepara a requisi��o de exclus�o
            var deleteRequest = new AssuntoDeleteDto { CodAs = addedAssunto.CodAs };
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/assunto")
            {
                Content = JsonContent.Create(deleteRequest)
            };

            // Envia a requisi��o DELETE
            var response = await _client.SendAsync(requestMessage);

            // Verifica o resultado
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // Teste de falha: Excluir Assunto inexistente
        [Fact(DisplayName = "Excluir Assunto deve falhar quando o c�digo n�o existir")]
        public async Task Delete_ShouldFail_WhenAssuntoDoesNotExist()
        {
            // Prepara a requisi��o de exclus�o para um ID inexistente
            var deleteRequest = new AssuntoDeleteDto { CodAs = 9999 };
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/assunto")
            {
                Content = JsonContent.Create(deleteRequest) // Serializa o objeto como JSON
            };

            // Envia a requisi��o DELETE
            var response = await _client.SendAsync(requestMessage);

            // Verifica se o c�digo de status � 404 (NotFound)
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Teste de sucesso: Obter todos os Assuntos
        [Fact(DisplayName = "Obter todos os Assuntos com sucesso")]
        public async Task GetAll_ShouldReturnAllAssuntos()
        {
            await _client.PostAsJsonAsync("/api/assunto", new AssuntoInsertDto { Descricao = "Assunto 1" });
            await _client.PostAsJsonAsync("/api/assunto", new AssuntoInsertDto { Descricao = "Assunto 2" });

            var response = await _client.GetAsync("/api/assunto");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<List<AssuntoResponseDto>>();
            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(1);
        }

        // Teste de sucesso: Obter Assunto por ID
        [Fact(DisplayName = "Obter Assunto por ID com sucesso")]
        public async Task GetById_ShouldReturnAssunto_WhenAssuntoExists()
        {
            var addRequest = new AssuntoInsertDto { Descricao = "Assunto para Busca" };
            var addResponse = await _client.PostAsJsonAsync("/api/assunto", addRequest);
            var addedAssunto = await addResponse.Content.ReadFromJsonAsync<AssuntoResponseDto>();

            var response = await _client.GetAsync($"/api/assunto/{addedAssunto.CodAs}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AssuntoResponseDto>();
            result.Should().NotBeNull();
            result.CodAs.Should().Be(addedAssunto.CodAs);
            result.Descricao.Should().Be(addedAssunto.Descricao);
        }
    }
}
