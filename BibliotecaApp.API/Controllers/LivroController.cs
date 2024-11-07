using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApp.API.Controllers
{
//    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LivroController : ControllerBase
    {
        private readonly ILivroAppService _livroAppService;

        public LivroController(ILivroAppService livroAppService)
        {
            _livroAppService = livroAppService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(LivroResponseDto), 201)]
        public async Task<IActionResult> Post([FromBody] LivroInsertDto request)
        {
            return StatusCode(201, await _livroAppService.AddAsync(request));
        }

        [HttpPut]
        [ProducesResponseType(typeof(LivroResponseDto), 200)]
        public async Task<IActionResult> Put([FromBody] LivroUpdateDto request)
        {
            return StatusCode(200, await _livroAppService.UpdateAsync(request));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(LivroResponseDto), 200)]
        public async Task<IActionResult> Delete(LivroDeleteDto request)
        {
            return StatusCode(200, await _livroAppService.DeleteAsync(request));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<LivroResponseDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return StatusCode(200, await _livroAppService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LivroResponseDto), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            return StatusCode(200, await _livroAppService.GetByIdAsync(id));
        }

    }

}

