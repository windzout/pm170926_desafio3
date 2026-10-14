using desafio03.Application.DTOs;
using desafio03.Application.Interfaces;
using desafio03.Domain.Entities;
using desafio03.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace desafio03.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly ApplicationDbContext _context;

        public ClienteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClienteDto>> ObtenerTodosAsync()
        {
            return await _context.Clientes
                .Select(c => new ClienteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    Email = c.Email
                }).ToListAsync();
        }

        public async Task<ClienteDto?> ObtenerPorIdAsync(int id)
        {
            var c = await _context.Clientes.FindAsync(id);
            if (c == null) return null;

            return new ClienteDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Email = c.Email
            };
        }

        public async Task<ClienteDto> CrearAsync(CrearClienteDto dto)
        {
            var cliente = new Cliente
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Email = cliente.Email
            };
        }
    }
}