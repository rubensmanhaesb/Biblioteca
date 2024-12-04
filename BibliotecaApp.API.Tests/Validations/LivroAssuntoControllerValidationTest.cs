using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BibliotecaApp.API.Tests.Tests;
using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace BibliotecaApp.API.Tests.Validations
{
    public class LivroAssuntoControllerValidationTest
    {
        private readonly LivroAssuntoControllerTestBase _testBase;

        public LivroAssuntoControllerValidationTest()
        {
            _testBase = new LivroAssuntoControllerTestBase();
        }

        [Fact(DisplayName = "Verificar se a rota /api/livroAssunto está acessível")]
        public async Task Route_ShouldBeAccessible()
        {
            var response = await _testBase.GetAllLivroAssuntoAsync();
            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Verificar se os controladores estão registrados corretamente")]
        public void ShouldRegisterControllers()
        {
            var controllers = _testBase.ShouldRegisterControllers();
            controllers.Should().Contain(c => c.Name == "LivroAssuntoController");
        }

        [Fact(DisplayName = "Adicionar LivroAssunto com sucesso")]
        public async Task Post_ShouldAddLivroAssunto_WhenValid()
        {
            var (livro, assunto) = await _testBase.PrepareLivroAndAssuntoAsync();
            var livroAssunto = new LivroAssuntoDto { LivroCodl = livro.Codl, AssuntoCodAs = assunto.CodAs};

            var response = await _testBase.AddLivroAssuntoAsync(livroAssunto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<LivroAssuntoResponseDto>();
            result.Should().NotBeNull();
            result.LivroCodl.Should().Be(livro.Codl);
            result.AssuntoCodAs.Should().Be(assunto.CodAs);
            result.Assunto.Should().Be(assunto);
            result.Livro.Should().Be(livro);
        }

        [Fact(DisplayName = "Adicionar LivroAssunto deve falhar quando Livro não existir")]
        public async Task Post_ShouldFail_WhenLivroNotFound()
        {
            var (_, assunto) = await _testBase.PrepareLivroAndAssuntoAsync();
            var livroAssunto = new LivroAssuntoDto { LivroCodl = 9999, AssuntoCodAs = assunto .CodAs}; // Livro inexistente

            var response = await _testBase.AddLivroAssuntoAsync(livroAssunto);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationResponseError>();
            errorResponse.Should().NotBeNull();
            errorResponse.Details.Should().Be($"Livro {livroAssunto.LivroCodl} não encontrado.");

        }

        [Fact(DisplayName = "Adicionar LivroAssunto deve falhar quando Assunto não existir")]
        public async Task Post_ShouldFail_WhenAssuntoNotFound()
        {
            var (livro, _) = await _testBase.PrepareLivroAndAssuntoAsync();
            var livroAssunto = new LivroAssuntoDto { LivroCodl = livro.Codl, AssuntoCodAs = 9999 }; // Livro inexistente

            var response = await _testBase.AddLivroAssuntoAsync(livroAssunto);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationResponseError>();
            errorResponse.Should().NotBeNull();
            errorResponse.Details.Should().Be($"Assunto {livroAssunto.AssuntoCodAs} não encontrado.");
        }

        [Fact(DisplayName = "Excluir LivroAssunto com sucesso")]
        public async Task Delete_ShouldRemoveLivroAssunto_WhenValid()
        {
            var (livro, assunto) = await _testBase.PrepareLivroAndAssuntoAsync();
            var livroAssunto = new LivroAssuntoDto { LivroCodl = livro.Codl, AssuntoCodAs = assunto.CodAs};

            // Adicionar LivroAssunto
            var addResponse = await _testBase.AddLivroAssuntoAsync(livroAssunto);
            addResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var pk = new LivroAssuntoDto { LivroCodl = livro.Codl, AssuntoCodAs = assunto.CodAs};
            var response = await _testBase.DeleteLivroAssuntoAsync(pk);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<LivroAssuntoResponseDto>();
            result.Should().NotBeNull();
            result.LivroCodl.Should().Be(livro.Codl);
            result.AssuntoCodAs.Should().Be(assunto.CodAs);
            result.Livro.Should().Be(livro);
            result.Assunto.Should().Be(assunto);

        }

        [Fact(DisplayName = "Excluir LivroAssunto com falha")]
        public async Task Delete_ShouldRemoveLivroAssunto_WhenInvalid()
        {
            var (livro, assunto) = await _testBase.PrepareLivroAndAssuntoAsync();
            var livroAssunto = new LivroAssuntoDto { LivroCodl = livro.Codl, AssuntoCodAs =assunto.CodAs};


            var pk = new LivroAssuntoDto { LivroCodl = livro.Codl, AssuntoCodAs = assunto.CodAs};
            var response = await _testBase.DeleteLivroAssuntoAsync(pk);
            response.Should().NotBeNull();

            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationResponseError>();
            errorResponse.Should().NotBeNull();
            errorResponse.Details.Should().Be($"Livro Assunto {livroAssunto.LivroCodl} e {livroAssunto.AssuntoCodAs} não encontrado.");

        }


        [Fact(DisplayName = "Obter LivroAssunto por ID com sucesso")]
        public async Task GetById_ShouldReturnLivroAssunto_WhenValid()
        {
            var (livro, assunto) = await _testBase.PrepareLivroAndAssuntoAsync();
            var livroAssunto = new LivroAssuntoDto { LivroCodl = livro.Codl, AssuntoCodAs = assunto.CodAs };

            // Adicionar LivroAssunto
            var addResponse = await _testBase.AddLivroAssuntoAsync(livroAssunto);
            addResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var pk = new LivroAssuntoPkDto { LivroCodl = livro.Codl, AssuntoCodAs = assunto.CodAs };
            var response = await _testBase.GetLivroAssuntoByIdAsync(pk);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<LivroAssuntoResponseDto>();
            result.Should().NotBeNull();
            result.LivroCodl.Should().Be(pk.LivroCodl);
            result.AssuntoCodAs.Should().Be(pk.AssuntoCodAs);
        }

        [Fact(DisplayName = "Obter todos os LivroAssuntoes com sucesso")]
        public async Task GetAll_ShouldReturnAllLivroAssuntoes()
        {
            var (livro, assunto) = await _testBase.PrepareLivroAndAssuntoAsync();
            var livroAssunto = new LivroAssuntoDto { LivroCodl = livro.Codl, AssuntoCodAs = assunto.CodAs };

            // Adicionar LivroAssunto
            await _testBase.AddLivroAssuntoAsync(livroAssunto);

            var response = await _testBase.GetAllLivroAssuntoAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<List<LivroAssuntoResponseDto>>();
            result.Should().NotBeNull();
            result.Should().Contain(la => la.LivroCodl == livro.Codl && la.AssuntoCodAs == assunto.CodAs);
        }
    }
}

