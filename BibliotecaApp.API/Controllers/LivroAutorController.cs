using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace BibliotecaApp.API.Controllers
{
    //    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LivroAutorController : ControllerBase
    {
        private readonly ILivroAutorAppService _livroAutorAppService;

        public LivroAutorController(ILivroAutorAppService livroAutorAppService)
        {
            _livroAutorAppService = livroAutorAppService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(LivroAutorResponseDto), 201)]
        public async Task<IActionResult> Post([FromBody] LivroAutorDto request)
        {
            return StatusCode(201, await _livroAutorAppService.AddAsync(request));
        }


        [HttpDelete]
        [ProducesResponseType(typeof(LivroAutorResponseDto), 200)]
        public async Task<IActionResult> Delete(LivroAutorDto request)
        {
            return StatusCode(200, await _livroAutorAppService.DeleteAsync(request));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<LivroAutorDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return StatusCode(200, await _livroAutorAppService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LivroAutorResponseDto), 200)]
        public async Task<IActionResult> GetById(LivroAutorPkDto id)
        {
            return StatusCode(200, await _livroAutorAppService.GetByIdAsync(id));
        }
    }
}