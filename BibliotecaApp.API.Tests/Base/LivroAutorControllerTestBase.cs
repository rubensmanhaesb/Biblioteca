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
using BibliotecaApp.API.Tests.Base;
using Microsoft.Extensions.Logging;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Validation;

namespace BibliotecaApp.API.Tests.Tests
{
    public class LivroAutorControllerTestBase
    {
        private readonly HttpClient _client;
        private readonly LivroControllerTestBase _livroTestBase;
        private readonly AutorControllerTestBase _autorTestBase;
        public LivroAutorControllerTestBase()
        {
            _livroTestBase = new LivroControllerTestBase();
            _autorTestBase = new AutorControllerTestBase();

            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<DataContext>(options =>
                        options.UseInMemoryDatabase("BibliotecaAppTest"));
                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                    services.AddScoped<ILivroRepository, LivroRepository>();
                    services.AddScoped<IAutorRepository, AutorRepository>();
                    services.AddScoped<ILivroAutorRepository, LivroAutorRepository>();
                    services.AddScoped<ILivroAutorAppService, LivroAutorAppService>();
                    services.AddScoped<ILivroAutorDomainService, LivroAutorDomainService>();
                    services.AddTransient<IValidator<LivroAutor>, LivroAutorValidator>();

                    // AutoMapper e Validators
                    services.AddAutoMapper(cfg => cfg.AddProfile<LivroAutorMapping>());
                    services.AddControllers().AddApplicationPart(typeof(LivroAutorController).Assembly);
                    services.AddValidatorsFromAssemblyContaining<LivroAutorDto>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole(); // Log no console durante o teste
                    logging.AddDebug();   // Log para depuração
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

        public Task<HttpResponseMessage> AddLivroAutorAsync(LivroAutorDto dto)
        {
            return _client.PostAsJsonAsync("/api/livroautor", dto);
        }

        public Task<HttpResponseMessage> UpdateLivroAutorAsync(LivroAutorDto dto)
        {
            return _client.PutAsJsonAsync("/api/livroautor", dto);
        }

        public Task<HttpResponseMessage> DeleteLivroAutorAsync(LivroAutorDto livroAutorDto)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/livroautor")
            {
                Content = JsonContent.Create(livroAutorDto)
            };
            return _client.SendAsync(requestMessage);
        }

        public Task<HttpResponseMessage> GetLivroAutorByIdAsync(LivroAutorPkDto pk)
        {
            return _client.GetAsync($"/api/livroautor/{pk.LivroCodl}/{pk.AutorCodAu}" );
        }

        public Task<HttpResponseMessage> GetAllLivroAutorAsync()
        {
            return _client.GetAsync("/api/livroautor");
        }

        public async Task<LivroResponseDto> CreateLivroAsync()
        {
            var livroDto = _livroTestBase.CreateValidLivro();
            var response = await _livroTestBase.AddLivroAsync(livroDto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<LivroResponseDto>();
        }

        public async Task<AutorResponseDto> CreateAutorAsync()
        {
            var autorDto = _autorTestBase.CreateValidAutor();
            var response = await _autorTestBase.AddAutorAsync(autorDto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AutorResponseDto>();
        }

        public async Task<(LivroResponseDto, AutorResponseDto)> PrepareLivroAndAutorAsync()
        {
            var livro = await CreateLivroAsync();
            var autor = await CreateAutorAsync();

            return (livro, autor);
        }

        public Type[] ShouldRegisterControllers()
        {
            return typeof(LivroAutorController).Assembly.GetExportedTypes();
        }
    }
}
