using Microsoft.AspNetCore.Mvc;
using desafio03.Application.DTOs;
using desafio03.Application.Interfaces;

namespace desafio03.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoDto>>> ObtenerTodos()
        {
            var pedidos = await _pedidoService.ObtenerTodosAsync();
            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoDto>> ObtenerPorId(int id)
        {
            var pedido = await _pedidoService.ObtenerPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound(new { mensaje = "Pedido no encontrado." });
            }
            return Ok(pedido);
        }

        [HttpPost]
        public async Task<ActionResult<PedidoDto>> Crear([FromBody] CrearPedidoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nuevoPedido = await _pedidoService.CrearAsync(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoPedido.Id }, nuevoPedido);
            }
            catch (InvalidOperationException ex)
            {
                // Aquí capturamos la regla de negocio de stock insuficiente o cliente/producto inexistente
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error interno.", detalle = ex.Message });
            }
        }
    }
}