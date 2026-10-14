using Microsoft.AspNetCore.Mvc;
using desafio03.Application.DTOs;
using desafio03.Application.Interfaces;

namespace desafio03.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> ObtenerTodos()
        {
            var productos = await _productoService.ObtenerTodosAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> ObtenerPorId(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                return NotFound(new { mensaje = "Producto no encontrado." });
            }
            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoDto>> Crear([FromBody] CrearProductoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nuevoProducto = await _productoService.CrearAsync(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoProducto.Id }, nuevoProducto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}