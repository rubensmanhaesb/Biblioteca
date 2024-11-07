using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Interfaces;
using BibliotecaApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApp.API.Controllers
{
//    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PrecoLivroController : ControllerBase
    {
        private readonly IPrecoLivroAppService _precoLivroAppService;

        public PrecoLivroController(IPrecoLivroAppService autorAppService)
        {
            _precoLivroAppService = autorAppService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(PrecoLivroResponseDto), 201)]
        public async Task<IActionResult> Post([FromBody] PrecoLivroInsertDto request)
        {
            return StatusCode(201, await _precoLivroAppService.AddAsync(request));
        }

        [HttpPut]
        [ProducesResponseType(typeof(PrecoLivroResponseDto), 200)]
        public async Task<IActionResult> Put([FromBody] PrecoLivroUpdateDto request)
        {
            return StatusCode(200, await _precoLivroAppService.UpdateAsync(request));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(PrecoLivroResponseDto), 200)]
        public async Task<IActionResult> Delete(PrecoLivroDeleteDto request)
        {
            return StatusCode(200, await _precoLivroAppService.DeleteAsync(request));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PrecoLivroResponseDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return StatusCode(200, await _precoLivroAppService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PrecoLivroResponseDto), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            return StatusCode(200, await _precoLivroAppService.GetByIdAsync(id));
        }

        [HttpGet("{codl}/{tipoCompra}")]
        [ProducesResponseType(typeof(PrecoLivroResponseDto), 200)]
        public async Task<IActionResult> GetByAllByKey(int codl, TipoCompra tipoCompra)
        {

            return StatusCode(200, await _precoLivroAppService.GetAllByKey(codl, tipoCompra));
        }

    }

}
