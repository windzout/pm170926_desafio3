using Microsoft.AspNetCore.Mvc;
using desafio03.Application.DTOs;
using desafio03.Application.Interfaces;

namespace desafio03.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> ObtenerTodos()
        {
            var clientes = await _clienteService.ObtenerTodosAsync();
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> ObtenerPorId(int id)
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            if (cliente == null)
            {
                return NotFound(new { mensaje = "Cliente no encontrado." });
            }
            return Ok(cliente);
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDto>> Crear([FromBody] CrearClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nuevoCliente = await _clienteService.CrearAsync(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoCliente.Id }, nuevoCliente);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}