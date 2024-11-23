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
    public class LivroAssuntoControllerTestBase
    {
        private readonly HttpClient _client;
        private readonly LivroControllerTestBase _livroTestBase;
        private readonly AssuntoControllerTestBase _assuntoTestBase;
        public LivroAssuntoControllerTestBase()
        {
            _livroTestBase = new LivroControllerTestBase();
            _assuntoTestBase = new AssuntoControllerTestBase();

            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<DataContext>(options =>
                        options.UseInMemoryDatabase("BibliotecaAppTest"));
                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                    services.AddScoped<ILivroRepository, LivroRepository>();
                    services.AddScoped<IAssuntoRepository, AssuntoRepository>();
                    services.AddScoped<ILivroAssuntoRepository, LivroAssuntoRepository>();
                    services.AddScoped<ILivroAssuntoAppService, LivroAssuntoAppService>();
                    services.AddScoped<ILivroAssuntoDomainService, LivroAssuntoDomainService>();
                    services.AddTransient<IValidator<LivroAssunto>, LivroAssuntoValidator>();

                    // AutoMapper e Validators
                    services.AddAutoMapper(cfg => cfg.AddProfile<LivroAssuntoMapping>());
                    services.AddControllers().AddApplicationPart(typeof(LivroAssuntoController).Assembly);
                    services.AddValidatorsFromAssemblyContaining<LivroAssuntoDto>();
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

        public Task<HttpResponseMessage> AddLivroAssuntoAsync(LivroAssuntoDto dto)
        {
            return _client.PostAsJsonAsync("/api/livroAssunto", dto);
        }

        public Task<HttpResponseMessage> UpdateLivroAssuntoAsync(LivroAssuntoDto dto)
        {
            return _client.PutAsJsonAsync("/api/livroAssunto", dto);
        }

        public Task<HttpResponseMessage> DeleteLivroAssuntoAsync(LivroAssuntoDto livroAssuntoDto)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/livroAssunto")
            {
                Content = JsonContent.Create(livroAssuntoDto)
            };
            return _client.SendAsync(requestMessage);
        }

        public Task<HttpResponseMessage> GetLivroAssuntoByIdAsync(LivroAssuntoPkDto pk)
        {
            return _client.GetAsync($"/api/livroAssunto/{pk.LivroCodl}/{pk.AssuntoCodAs}");
        }

        public Task<HttpResponseMessage> GetAllLivroAssuntoAsync()
        {
            return _client.GetAsync("/api/livroAssunto");
        }


        public async Task<LivroResponseDto> CreateLivroAsync()
        {

            var livroDto = _livroTestBase.CreateValidLivro();
            var response = await _livroTestBase.AddLivroAsync(livroDto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<LivroResponseDto>();
        }

        public async Task<AssuntoResponseDto> CreateAssuntoAsync()
        {

            var assuntoDto = _assuntoTestBase.CreateValidAssunto();
            var response = await _assuntoTestBase.AddAssuntoAsync(assuntoDto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AssuntoResponseDto>();
        }


        public async Task<(LivroResponseDto, AssuntoResponseDto)> PrepareLivroAndAssuntoAsync()
        {
            var livro = await CreateLivroAsync();
            var assunto = await CreateAssuntoAsync();

            return (livro, assunto);
        }

        public Type[] ShouldRegisterControllers()
        {
            return typeof(LivroAssuntoController).Assembly.GetExportedTypes();
        }
    }
}

