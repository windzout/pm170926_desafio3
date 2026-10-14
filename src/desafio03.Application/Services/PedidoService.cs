using desafio03.Application.DTOs;
using desafio03.Application.Interfaces;
using desafio03.Domain.Entities;
using desafio03.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace desafio03.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly ApplicationDbContext _context;

        public PedidoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PedidoDto>> ObtenerTodosAsync()
        {
            return await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Producto)
                .Select(p => new PedidoDto
                {
                    Id = p.Id,
                    FechaPedido = p.FechaPedido,
                    Cantidad = p.Cantidad,
                    ClienteId = p.ClienteId,
                    ClienteNombre = p.Cliente != null ? $"{p.Cliente.Nombre} {p.Cliente.Apellido}" : string.Empty,
                    ProductoId = p.ProductoId,
                    ProductoNombre = p.Producto != null ? p.Producto.Nombre : string.Empty
                }).ToListAsync();
        }

        public async Task<PedidoDto?> ObtenerPorIdAsync(int id)
        {
            var p = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null) return null;

            return new PedidoDto
            {
                Id = p.Id,
                FechaPedido = p.FechaPedido,
                Cantidad = p.Cantidad,
                ClienteId = p.ClienteId,
                ClienteNombre = p.Cliente != null ? $"{p.Cliente.Nombre} {p.Cliente.Apellido}" : string.Empty,
                ProductoId = p.ProductoId,
                ProductoNombre = p.Producto != null ? p.Producto.Nombre : string.Empty
            };
        }

        public async Task<PedidoDto> CrearAsync(CrearPedidoDto dto)
        {
            // Validar que el cliente exista
            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null)
            {
                throw new InvalidOperationException("El cliente especificado no existe.");
            }

            // Validar que el producto exista
            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            if (producto == null)
            {
                throw new InvalidOperationException("El producto especificado no existe.");
            }

            // REGLA DE NEGOCIO: Validar stock suficiente
            if (producto.Stock < dto.Cantidad)
            {
                throw new InvalidOperationException($"Stock insuficiente. Stock disponible: {producto.Stock}, cantidad solicitada: {dto.Cantidad}");
            }

            // Descontar stock del producto
            producto.Stock -= dto.Cantidad;

            // Crear el pedido
            var pedido = new Pedido
            {
                FechaPedido = DateTime.UtcNow,
                Cantidad = dto.Cantidad,
                ClienteId = dto.ClienteId,
                ProductoId = dto.ProductoId
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return new PedidoDto
            {
                Id = pedido.Id,
                FechaPedido = pedido.FechaPedido,
                Cantidad = pedido.Cantidad,
                ClienteId = pedido.ClienteId,
                ClienteNombre = $"{cliente.Nombre} {cliente.Apellido}",
                ProductoId = producto.Id,
                ProductoNombre = producto.Nombre
            };
        }
    }
}