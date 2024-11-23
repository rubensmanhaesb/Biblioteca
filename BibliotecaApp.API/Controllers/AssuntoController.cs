using BibliotecaApp.Aplication.Dtos;
using BibliotecaApp.Aplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApp.API.Controllers
{
    //    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class AssuntoController : ControllerBase
    {
        private readonly IAssuntoAppService _assuntoAppService;

        public AssuntoController(IAssuntoAppService assuntoAppService)
        {
            _assuntoAppService = assuntoAppService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AssuntoResponseDto), 201)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Post([FromBody] AssuntoInsertDto request)
        {
            return StatusCode(201, await _assuntoAppService.AddAsync(request));
        }

        [HttpPut]
        [ProducesResponseType(typeof(AssuntoResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put([FromBody] AssuntoUpdateDto request)
        {
            Console.WriteLine($"Recebido PUT para CodAs: {request.CodAs} com Descricao: {request.Descricao}");
            return StatusCode(200, await _assuntoAppService.UpdateAsync(request));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(AssuntoResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(AssuntoDeleteDto request)
        {
            return StatusCode(200, await _assuntoAppService.DeleteAsync(request));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AssuntoResponseDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            return StatusCode(200, await _assuntoAppService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AssuntoResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            return StatusCode(200, await _assuntoAppService.GetByIdAsync(id));
        }
    }
}
