using AutoMapper;
using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Services;
using BibliotecaApp.Domain.Entities;
using BibliotecaApp.Domain.Enums;
using BibliotecaApp.Domain.Interfaces.Services;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BibliotecaApp.Tests.Aplication
{
    public class PrecoLivroAppServiceTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPrecoLivroDomainService> _precoLivroDomainServiceMock;
        private readonly PrecoLivroAppService _precoLivroAppService;

        public PrecoLivroAppServiceTest()
        {
            _mapperMock = new Mock<IMapper>();
            _precoLivroDomainServiceMock = new Mock<IPrecoLivroDomainService>();
            _precoLivroAppService = new PrecoLivroAppService(_mapperMock.Object, _precoLivroDomainServiceMock.Object);
        }

        private PrecoLivroInsertDto GeneratePrecoLivroInsertDto() => new PrecoLivroInsertDto
        {
            LivroCodl = 100,
            TipoCompra = TipoCompra.Balcao,
            Valor = 50.0M
        };

        private PrecoLivroUpdateDto GeneratePrecoLivroUpdateDto() => new PrecoLivroUpdateDto
        {
            Codp = 1,
            LivroCodl = 100,
            TipoCompra = TipoCompra.Balcao,
            Valor = 50.0M
        };

        private PrecoLivroDeleteDto GeneratePrecoLivroDeleteDto() => new PrecoLivroDeleteDto
        {
            Codp = 1
        };

        private PrecoLivro GeneratePrecoLivroEntity() => new PrecoLivro
        {
            Codp = 1,
            LivroCodl = 100,
            TipoCompra = TipoCompra.Balcao,
            Valor = 50.0M
        };

        [Fact(DisplayName = "Adicionar PrecoLivro com sucesso")]
        public async Task AddAsync_ShouldReturnResponseDto_WhenValid()
        {
            var dto = GeneratePrecoLivroInsertDto();
            var precoLivroEntity = GeneratePrecoLivroEntity();

            _mapperMock.Setup(m => m.Map<PrecoLivro>(dto)).Returns(precoLivroEntity);
            _precoLivroDomainServiceMock.Setup(s => s.AddAsync(precoLivroEntity));
            _mapperMock.Setup(m => m.Map<PrecoLivroResponseDto>(precoLivroEntity)).Returns(new PrecoLivroResponseDto { Codp = precoLivroEntity.Codp });

            var result = await _precoLivroAppService.AddAsync(dto);

            result.Should().NotBeNull();
            result.Codp.Should().Be(precoLivroEntity.Codp);
        }

        [Fact(DisplayName = "Atualizar PrecoLivro com sucesso")]
        public async Task UpdateAsync_ShouldReturnResponseDto_WhenValid()
        {
            var dto = GeneratePrecoLivroUpdateDto();
            var precoLivroEntity = GeneratePrecoLivroEntity();

            _mapperMock.Setup(m => m.Map<PrecoLivro>(dto)).Returns(precoLivroEntity);
            _precoLivroDomainServiceMock.Setup(s => s.UpdateAsync(precoLivroEntity));
            _mapperMock.Setup(m => m.Map<PrecoLivroResponseDto>(precoLivroEntity)).Returns(new PrecoLivroResponseDto { Codp = precoLivroEntity.Codp });

            var result = await _precoLivroAppService.UpdateAsync(dto);

            result.Should().NotBeNull();
            result.Codp.Should().Be(precoLivroEntity.Codp);
        }

        [Fact(DisplayName = "Excluir PrecoLivro com sucesso")]
        public async Task DeleteAsync_ShouldReturnResponseDto_WhenValid()
        {
            var dto = GeneratePrecoLivroDeleteDto();
            var precoLivroEntity = GeneratePrecoLivroEntity();

            _mapperMock.Setup(m => m.Map<PrecoLivro>(dto)).Returns(precoLivroEntity);
            _precoLivroDomainServiceMock.Setup(s => s.DeleteAsync(precoLivroEntity));
            _mapperMock.Setup(m => m.Map<PrecoLivroResponseDto>(precoLivroEntity)).Returns(new PrecoLivroResponseDto { Codp = precoLivroEntity.Codp });

            var result = await _precoLivroAppService.DeleteAsync(dto);

            result.Should().NotBeNull();
            result.Codp.Should().Be(precoLivroEntity.Codp);
        }

        [Fact(DisplayName = "Consultar todos os PrecoLivro com sucesso")]
        public async Task GetAllAsync_ShouldReturnListOfResponseDto()
        {
            var precoLivroList = new List<PrecoLivro> { GeneratePrecoLivroEntity() };
            var precoLivroDtoList = new List<PrecoLivroResponseDto> { new PrecoLivroResponseDto { Codp = 1 } };

            _precoLivroDomainServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(precoLivroList);
            _mapperMock.Setup(m => m.Map<List<PrecoLivroResponseDto>>(precoLivroList)).Returns(precoLivroDtoList);

            var result = await _precoLivroAppService.GetAllAsync();

            result.Should().NotBeNull();
            result.Should().HaveCountGreaterThan(0);
        }

        [Fact(DisplayName = "Consultar PrecoLivro por ID com sucesso")]
        public async Task GetByIdAsync_ShouldReturnResponseDto_WhenPrecoLivroExists()
        {
            var precoLivroEntity = GeneratePrecoLivroEntity();
            var responseDto = new PrecoLivroResponseDto { Codp = precoLivroEntity.Codp };

            _precoLivroDomainServiceMock.Setup(s => s.GetByIdAsync(precoLivroEntity.Codp)).ReturnsAsync(precoLivroEntity);
            _mapperMock.Setup(m => m.Map<PrecoLivroResponseDto>(precoLivroEntity)).Returns(responseDto);

            var result = await _precoLivroAppService.GetByIdAsync(precoLivroEntity.Codp);

            result.Should().NotBeNull();
            result.Codp.Should().Be(precoLivroEntity.Codp);
        }
    }
}
