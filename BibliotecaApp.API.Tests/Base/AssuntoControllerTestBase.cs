using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace BibliotecaApp.API.Tests.Base
{
    public class AssuntoControllerTestBase
    {
        private readonly HttpClient _client;

        public AssuntoControllerTestBase()
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    // Configurações do banco em memória e dependências
                    services.AddDbContext<DataContext>(options =>
                        options.UseInMemoryDatabase("BibliotecaAppTest"));
                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                    services.AddScoped<IAssuntoRepository, AssuntoRepository>();
                    services.AddScoped<IAssuntoAppService, AssuntoAppService>();
                    services.AddScoped<IAssuntoDomainService, AssuntoDomainService>();

                    // Configuração de AutoMapper e Validators
                    services.AddAutoMapper(cfg => cfg.AddProfile<AssuntoMapping>());
                    services.AddControllers().AddApplicationPart(typeof(AssuntoController).Assembly);
                    services.AddValidatorsFromAssemblyContaining<AssuntoInsertDto>();
                    services.AddValidatorsFromAssemblyContaining<AssuntoUpdateDto>();
                    services.AddValidatorsFromAssemblyContaining<AssuntoDeleteDto>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole(); // Log no console durante o teste
                    logging.AddDebug();   // Log para depuração
                })
                .Configure(app =>
                {
                    // Configuração dos middlewares
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

        // Métodos para acessar os endpoints da API
        public Task<HttpResponseMessage> GetAllAssuntosAsync()
        {
            return _client.GetAsync("/api/assunto");
        }

        public Task<HttpResponseMessage> AddAssuntoAsync(AssuntoInsertDto assuntoDto)
        {
            return _client.PostAsJsonAsync("/api/assunto", assuntoDto);
        }

        public Task<HttpResponseMessage> UpdateAssuntoAsync(AssuntoUpdateDto assuntoDto)
        {
            return _client.PutAsJsonAsync("/api/assunto", assuntoDto);
        }

        public Task<HttpResponseMessage> DeleteAssuntoAsync(AssuntoDeleteDto assuntoDto)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/assunto")
            {
                Content = JsonContent.Create(assuntoDto)
            };
            return _client.SendAsync(requestMessage);
        }

        public Task<HttpResponseMessage> GetAssuntoByIdAsync(int codAs)
        {
            return _client.GetAsync($"/api/assunto/{codAs}");
        }

        // Métodos para geração de dados
        public AssuntoInsertDto CreateValidAssunto()
        {
            return new AssuntoInsertDto
            {
                Descricao = "Novo Assunto"
            };
        }

        public AssuntoInsertDto CreateInvalidAssunto()
        {
            return new AssuntoInsertDto
            {
                Descricao = ""
            };
        }

        public AssuntoUpdateDto CreateValidAssuntoUpdate(int codAs)
        {
            return new AssuntoUpdateDto
            {
                CodAs = codAs,
                Descricao = "Assunto Atualizado"
            };
        }

        public AssuntoUpdateDto CreateInvalidAssuntoUpdate(int codAs)
        {
            return new AssuntoUpdateDto
            {
                CodAs = codAs,
                Descricao = ""
            };
        }

        public AssuntoDeleteDto CreateValidAssuntoDelete(int codAs)
        {
            return new AssuntoDeleteDto
            {
                CodAs = codAs
            };
        }
        
        public Type[]? ShouldRegisterControllers()
        {
            return typeof(LivroController).Assembly.GetExportedTypes();
        }
    }
}
