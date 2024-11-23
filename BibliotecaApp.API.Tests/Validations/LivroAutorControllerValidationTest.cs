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
    public class LivroAutorControllerValidationTest
    {
        private readonly LivroAutorControllerTestBase _testBase;

        public LivroAutorControllerValidationTest()
        {
            _testBase = new LivroAutorControllerTestBase();
        }

        [Fact(DisplayName = "Verificar se a rota /api/livroAutor está acessível")]
        public async Task Route_ShouldBeAccessible()
        {
            var response = await _testBase.GetAllLivroAutorAsync();
            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "Verificar se os controladores estão registrados corretamente")]
        public void ShouldRegisterControllers()
        {
            var controllers = _testBase.ShouldRegisterControllers();
            controllers.Should().Contain(c => c.Name == "LivroAutorController");
        }

        [Fact(DisplayName = "Adicionar LivroAutor com sucesso")]
        public async Task Post_ShouldAddLivroAutor_WhenValid()
        {
            var (livro, autor) = await _testBase.PrepareLivroAndAutorAsync();
            var livroAutor = new LivroAutorDto { LivroCodl = livro.Codl, AutorCodAu = autor.CodAu };

            var response = await _testBase.AddLivroAutorAsync(livroAutor);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<LivroAutorResponseDto>();
            result.Should().NotBeNull();
            result.LivroCodl.Should().Be(livro.Codl);
            result.AutorCodAu.Should().Be(autor.CodAu);
        }

        [Fact(DisplayName = "Adicionar LivroAutor deve falhar quando Livro não existir")]
        public async Task Post_ShouldFail_WhenLivroNotFound()
        {
            var (_, autor) = await _testBase.PrepareLivroAndAutorAsync();
            var livroAutor = new LivroAutorDto { LivroCodl = 9999, AutorCodAu = autor.CodAu }; // Livro inexistente

            var response = await _testBase.AddLivroAutorAsync(livroAutor);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationResponseError>();
            errorResponse.Should().NotBeNull();
            errorResponse.Details.Should().Be($"Livro {livroAutor.LivroCodl} não encontrado.");

        }

        [Fact(DisplayName = "Adicionar LivroAutor deve falhar quando Autor não existir")]
        public async Task Post_ShouldFail_WhenAutorNotFound()
        {
            var (livro, _) = await _testBase.PrepareLivroAndAutorAsync();
            var livroAutor = new LivroAutorDto { LivroCodl = livro.Codl, AutorCodAu = 9999}; // Livro inexistente

            var response = await _testBase.AddLivroAutorAsync(livroAutor);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationResponseError>();
            errorResponse.Should().NotBeNull();
            errorResponse.Details.Should().Be($"Autor {livroAutor.AutorCodAu} não encontrado.");
        }

        [Fact(DisplayName = "Excluir LivroAutor com sucesso")]
        public async Task Delete_ShouldRemoveLivroAutor_WhenValid()
        {
            var (livro, autor) = await _testBase.PrepareLivroAndAutorAsync();
            var livroAutor = new LivroAutorDto { LivroCodl = livro.Codl, AutorCodAu = autor.CodAu };

            // Adicionar LivroAutor
            var addResponse = await _testBase.AddLivroAutorAsync(livroAutor);
            addResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var pk = new LivroAutorDto { LivroCodl = livro.Codl, AutorCodAu = autor.CodAu };
            var response = await _testBase.DeleteLivroAutorAsync(pk);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(DisplayName = "Excluir LivroAutor com falha")]
        public async Task Delete_ShouldRemoveLivroAutor_WhenInvalid()
        {
            var (livro, autor) = await _testBase.PrepareLivroAndAutorAsync();
            var livroAutor = new LivroAutorDto { LivroCodl = livro.Codl, AutorCodAu = autor.CodAu };


            var pk = new LivroAutorDto { LivroCodl = livro.Codl, AutorCodAu = autor.CodAu };
            var response = await _testBase.DeleteLivroAutorAsync(pk);
            response.Should().NotBeNull();

            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationResponseError>();
            errorResponse.Should().NotBeNull();
            errorResponse.Details.Should().Be($"Livro Autor {livroAutor.LivroCodl} e {livroAutor.AutorCodAu} não encontrado.");

        }


        [Fact(DisplayName = "Obter LivroAutor por ID com sucesso")]
        public async Task GetById_ShouldReturnLivroAutor_WhenValid()
        {
            var (livro, autor) = await _testBase.PrepareLivroAndAutorAsync();
            var livroAutor = new LivroAutorDto { LivroCodl = livro.Codl, AutorCodAu = autor.CodAu };

            // Adicionar LivroAutor
            var addResponse = await _testBase.AddLivroAutorAsync(livroAutor);
            addResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var pk = new LivroAutorPkDto { LivroCodl = livro.Codl, AutorCodAu = autor.CodAu };
            var response = await _testBase.GetLivroAutorByIdAsync(pk);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<LivroAutorResponseDto>();
            result.Should().NotBeNull();
            result.LivroCodl.Should().Be(pk.LivroCodl);
            result.AutorCodAu.Should().Be(pk.AutorCodAu);
        }

        [Fact(DisplayName = "Obter todos os LivroAutores com sucesso")]
        public async Task GetAll_ShouldReturnAllLivroAutores()
        {
            var (livro, autor) = await _testBase.PrepareLivroAndAutorAsync();
            var livroAutor = new LivroAutorDto { LivroCodl = livro.Codl, AutorCodAu = autor.CodAu };

            // Adicionar LivroAutor
            await _testBase.AddLivroAutorAsync(livroAutor);

            var response = await _testBase.GetAllLivroAutorAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<List<LivroAutorResponseDto>>();
            result.Should().NotBeNull();
            result.Should().Contain(la => la.LivroCodl == livro.Codl && la.AutorCodAu == autor.CodAu);
        }
    }
}
