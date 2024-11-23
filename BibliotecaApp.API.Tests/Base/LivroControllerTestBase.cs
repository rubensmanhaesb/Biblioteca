using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BibliotecaApp.API.Middlewares;
using BibliotecaApp.API.Controllers;
using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Interfaces;
using BibliotecaApp.Aplication.Mappings;
using BibliotecaApp.Aplication.Services;
using BibliotecaApp.Domain.Interfaces.Repositories;
using BibliotecaApp.Domain.Interfaces.Services;
using BibliotecaApp.Domain.Services;
using BibliotecaApp.Infra.Data.Context;
using BibliotecaApp.Infra.Data.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace BibliotecaApp.API.Tests.Base
{
    public class LivroControllerTestBase
    {
        private readonly HttpClient _client;

        public LivroControllerTestBase()
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    // Configuração do banco em memória e dependências
                    services.AddDbContext<DataContext>(options =>
                        options.UseInMemoryDatabase("BibliotecaAppTest"));
                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                    services.AddScoped<ILivroRepository, LivroRepository>();
                    services.AddScoped<ILivroAppService, LivroAppService>();
                    services.AddScoped<ILivroDomainService, LivroDomainService>();

                    // Configuração de AutoMapper e Validators
                    services.AddAutoMapper(cfg => cfg.AddProfile<LivroMapping>());
                    services.AddControllers().AddApplicationPart(typeof(LivroController).Assembly);
                    services.AddValidatorsFromAssemblyContaining<LivroInsertDto>();
                    services.AddValidatorsFromAssemblyContaining<LivroUpdateDto>();
                    services.AddValidatorsFromAssemblyContaining<LivroDeleteDto>();
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
        public Task<HttpResponseMessage> GetAllLivrosAsync()
        {
            return _client.GetAsync("/api/livro");
        }

        public Task<HttpResponseMessage> AddLivroAsync(LivroInsertDto livroDto)
        {
            return _client.PostAsJsonAsync("/api/livro", livroDto);
        }

        public Task<HttpResponseMessage> UpdateLivroAsync(LivroUpdateDto livroDto)
        {
            return _client.PutAsJsonAsync("/api/livro", livroDto);
        }

        public Task<HttpResponseMessage> DeleteLivroAsync(LivroDeleteDto livroDto)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/livro")
            {
                Content = JsonContent.Create(livroDto)
            };
            return _client.SendAsync(requestMessage);
        }

        public Task<HttpResponseMessage> GetLivroByIdAsync(int codl)
        {
            return _client.GetAsync($"/api/livro/{codl}");
        }

        // Métodos para geração de dados
        public LivroInsertDto CreateValidLivro()
        {
            return new LivroInsertDto
            {
                Titulo = "Livro Teste",
                Editora = "Editora Teste",
                Edicao = 1,
                AnoPublicacao = "2023"
            };
        }

        public LivroInsertDto CreateInvalidLivroTitulo()
        {
            return new LivroInsertDto
            {
                Titulo = "",
                Editora = "Editora Teste",
                Edicao = 1,
                AnoPublicacao = "2023"
            };
        }

        public LivroInsertDto CreateInvalidLivroEdicao()
        {
            return new LivroInsertDto
            {
                Titulo = "Livro Teste",
                Editora = "Editora Teste",
                Edicao = 0,
                AnoPublicacao = "2023"
            };
        }

        public LivroUpdateDto CreateValidLivroUpdate(int codl)
        {
            return new LivroUpdateDto
            {
                Codl = codl,
                Titulo = "Livro Atualizado",
                Editora = "Editora Atualizada",
                Edicao = 2,
                AnoPublicacao = "2024"
            };
        }
        public Type[]? ShouldRegisterControllers()
        {
            return typeof(LivroController).Assembly.GetExportedTypes();
        }

    }
}
