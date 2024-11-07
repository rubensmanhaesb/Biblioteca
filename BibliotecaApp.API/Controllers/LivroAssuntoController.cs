using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivroAssuntoController : ControllerBase
    {
        private readonly ILivroAssuntoAppService _livroAssuntoAppService;

        public LivroAssuntoController(ILivroAssuntoAppService livroAutorAppService)
        {
            _livroAssuntoAppService = livroAutorAppService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(LivroAssuntoResponseDto), 201)]
        public async Task<IActionResult> Post([FromBody] LivroAssuntoDto request)
        {
            return StatusCode(201, await _livroAssuntoAppService.AddAsync(request));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(LivroAssuntoResponseDto), 200)]
        public async Task<IActionResult> Delete(LivroAssuntoDto request)
        {
            return StatusCode(200, await _livroAssuntoAppService.DeleteAsync(request));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<LivroAssuntoDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return StatusCode(200, await _livroAssuntoAppService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LivroAssuntoResponseDto), 200)]
        public async Task<IActionResult> GetById(LivroAssuntoPkDto id)
        {
            return StatusCode(200, await _livroAssuntoAppService.GetByIdAsync(id));
        }
    }

}
