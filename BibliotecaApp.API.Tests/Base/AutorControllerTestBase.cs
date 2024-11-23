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
    public class AutorControllerTestBase
    {
        private readonly HttpClient _client;

        public AutorControllerTestBase()
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    // Configuração do DbContext e repositórios
                    services.AddDbContext<DataContext>(options =>
                        options.UseInMemoryDatabase("BibliotecaAppTest"));
                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                    services.AddScoped<IAutorRepository, AutorRepository>();
                    services.AddScoped<IAutorAppService, AutorAppService>();
                    services.AddScoped<IAutorDomainService, AutorDomainService>();

                    // Configuração de AutoMapper e Validators
                    services.AddAutoMapper(cfg => cfg.AddProfile<AutorMapping>());
                    services.AddControllers().AddApplicationPart(typeof(AutorController).Assembly);
                    services.AddValidatorsFromAssemblyContaining<AutorInsertDto>();
                    services.AddValidatorsFromAssemblyContaining<AutorUpdateDto>();
                    services.AddValidatorsFromAssemblyContaining<AutorDeleteDto>();
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

        // Métodos de acesso ao controlador
        public Task<HttpResponseMessage> GetAllAutoresAsync()
        {
            return _client.GetAsync("/api/autor");
        }

        public Task<HttpResponseMessage> AddAutorAsync(AutorInsertDto autorDto)
        {
            return _client.PostAsJsonAsync("/api/autor", autorDto);
        }

        public Task<HttpResponseMessage> UpdateAutorAsync(AutorUpdateDto autorDto)
        {
            return _client.PutAsJsonAsync("/api/autor", autorDto);
        }

        public Task<HttpResponseMessage> DeleteAutorAsync(AutorDeleteDto autorDto)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/autor")
            {
                Content = JsonContent.Create(autorDto)
            };
            return _client.SendAsync(requestMessage);
        }

        public Task<HttpResponseMessage> GetAutorByIdAsync(int codAu)
        {
            return _client.GetAsync($"/api/autor/{codAu}");
        }

        // Geração de dados de teste com as mesmas regras da classe original
        public AutorInsertDto CreateValidAutor()
        {
            return new AutorInsertDto
            {
                Nome = "Novo Autor"
            };
        }

        public AutorInsertDto CreateInvalidAutor()
        {
            return new AutorInsertDto
            {
                Nome = ""
            };
        }

        public AutorUpdateDto CreateValidAutorUpdate(int codAu)
        {
            return new AutorUpdateDto
            {
                CodAu = codAu,
                Nome = "Autor Atualizado"
            };
        }

        public AutorUpdateDto CreateInvalidAutorUpdate(int codAu)
        {
            return new AutorUpdateDto
            {
                CodAu = codAu,
                Nome = ""
            };
        }

        public AutorDeleteDto CreateValidAutorDelete(int codAu)
        {
            return new AutorDeleteDto
            {
                CodAu = codAu
            };
        }

        public Type[]? ShouldRegisterControllers()
        {
            return typeof(LivroController).Assembly.GetExportedTypes();
        }
    }
}
