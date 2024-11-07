using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Interfaces;
using BibliotecaApp.Aplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace BibliotecaApp.API.Controllers
{
//    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {
        private readonly IAutorAppService _autorAppService;

        public AutorController(IAutorAppService autorAppService)
        {
            _autorAppService = autorAppService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AutorResponseDto), 201)]
        public async Task<IActionResult> Post([FromBody] AutorInsertDto request)
        {
            return StatusCode(201, await _autorAppService.AddAsync(request));
        }

        [HttpPut]
        [ProducesResponseType(typeof(AutorResponseDto), 200)]
        public async Task<IActionResult> Put([FromBody] AutorUpdateDto request)
        {
            return StatusCode(200, await _autorAppService.UpdateAsync(request));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(AutorResponseDto), 200)]
        public async Task<IActionResult> Delete(AutorDeleteDto request)
        {
            return StatusCode(200, await _autorAppService.DeleteAsync(request));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AutorResponseDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return StatusCode(200, await _autorAppService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AutorResponseDto), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            return StatusCode(200, await _autorAppService.GetByIdAsync(id));
        }
    }
}
