using desafio03.Application.DTOs;
using desafio03.Domain.Entities;

namespace desafio03.Application.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> ObtenerTodosAsync();
        Task<ClienteDto?> ObtenerPorIdAsync(int id);
        Task<ClienteDto> CrearAsync(CrearClienteDto dto);
    }

    public interface IProductoService
    {
        Task<IEnumerable<ProductoDto>> ObtenerTodosAsync();
        Task<ProductoDto?> ObtenerPorIdAsync(int id);
        Task<ProductoDto> CrearAsync(CrearProductoDto dto);
    }

    public interface IPedidoService
    {
        Task<IEnumerable<PedidoDto>> ObtenerTodosAsync();
        Task<PedidoDto?> ObtenerPorIdAsync(int id);
        Task<PedidoDto> CrearAsync(CrearPedidoDto dto); // ¡Aquí aplicamos la regla de negocio del stock!
    }
}