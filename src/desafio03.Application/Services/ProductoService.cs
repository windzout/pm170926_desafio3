using desafio03.Application.DTOs;
using desafio03.Application.Interfaces;
using desafio03.Domain.Entities;
using desafio03.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace desafio03.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly ApplicationDbContext _context;

        public ProductoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoDto>> ObtenerTodosAsync()
        {
            return await _context.Productos
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock
                }).ToListAsync();
        }

        public async Task<ProductoDto?> ObtenerPorIdAsync(int id)
        {
            var p = await _context.Productos.FindAsync(id);
            if (p == null) return null;

            return new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Stock = p.Stock
            };
        }

        public async Task<ProductoDto> CrearAsync(CrearProductoDto dto)
        {
            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Stock = dto.Stock
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock
            };
        }
    }
}