using Xunit;
using Microsoft.EntityFrameworkCore;
using desafio03.Infrastructure.Data;
using desafio03.Application.Services;
using desafio03.Application.DTOs;

namespace desafio03.Tests
{
    public class PedidoServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Base de datos aislada para cada test
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CrearCliente_DatosValidos_RetornaClienteCreado()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new ClienteService(context);
            var dto = new CrearClienteDto { Nombre = "Juan", Apellido = "Pérez", Email = "juan.perez@test.com" };

            // Act
            var resultado = await service.CrearAsync(dto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Juan", resultado.Nombre);
            Assert.Equal("juan.perez@test.com", resultado.Email);
            Assert.True(resultado.Id > 0);
        }

        [Fact]
        public async Task CrearProducto_DatosValidos_GuardaEnBD()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new ProductoService(context);
            var dto = new CrearProductoDto { Nombre = "Laptop", Descripcion = "Gamer", Precio = 1200.00m, Stock = 10 };

            // Act
            var resultado = await service.CrearAsync(dto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Laptop", resultado.Nombre);
            Assert.Equal(1200.00m, resultado.Precio);
            Assert.Equal(10, resultado.Stock);
        }

        [Fact]
        public async Task CrearPedido_StockSuficiente_CreaPedidoYReduceStock()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            // Sembrar datos iniciales
            var cliente = new Domain.Entities.Cliente { Nombre = "Ana", Apellido = "Gómez", Email = "ana@test.com" };
            var producto = new Domain.Entities.Producto { Nombre = "Teclado", Descripcion = "Mecánico", Precio = 50.00m, Stock = 5 };
            context.Clientes.Add(cliente);
            context.Productos.Add(producto);
            await context.SaveChangesAsync();

            var pedidoService = new PedidoService(context);
            var dto = new CrearPedidoDto { ClienteId = cliente.Id, ProductoId = producto.Id, Cantidad = 2 };

            // Act
            var resultado = await pedidoService.CrearAsync(dto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Cantidad);

            // Verificar regla de negocio: El stock del producto debe reducirse de 5 a 3
            var productoActualizado = await context.Productos.FindAsync(producto.Id);
            Assert.Equal(3, productoActualizado?.Stock);
        }

        [Fact]
        public async Task CrearPedido_CantidadMayorAStock_LanzaExcepcion()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var cliente = new Domain.Entities.Cliente { Nombre = "Carlos", Apellido = "López", Email = "carlos@test.com" };
            var producto = new Domain.Entities.Producto { Nombre = "Mouse", Descripcion = "Inalámbrico", Precio = 25.00m, Stock = 2 };
            context.Clientes.Add(cliente);
            context.Productos.Add(producto);
            await context.SaveChangesAsync();

            var pedidoService = new PedidoService(context);
            var dto = new CrearPedidoDto { ClienteId = cliente.Id, ProductoId = producto.Id, Cantidad = 5 }; // Pide 5, hay 2

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => pedidoService.CrearAsync(dto));
            Assert.Contains("Stock insuficiente", exception.Message);
        }

        [Fact]
        public async Task ObtenerPedidoPorId_Existente_RetornaPedidoConDetalles()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var cliente = new Domain.Entities.Cliente { Nombre = "Lucía", Apellido = "Martínez", Email = "lucia@test.com" };
            var producto = new Domain.Entities.Producto { Nombre = "Monitor", Descripcion = "24 pulgadas", Precio = 200.00m, Stock = 10 };
            context.Clientes.Add(cliente);
            context.Productos.Add(producto);
            await context.SaveChangesAsync();

            var pedidoService = new PedidoService(context);
            var pedidoCreado = await pedidoService.CrearAsync(new CrearPedidoDto { ClienteId = cliente.Id, ProductoId = producto.Id, Cantidad = 1 });

            // Act
            var resultado = await pedidoService.ObtenerPorIdAsync(pedidoCreado.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pedidoCreado.Id, resultado.Id);
            Assert.Equal("Lucía Martínez", resultado.ClienteNombre);
            Assert.Equal("Monitor", resultado.ProductoNombre);
        }

        [Fact]
        public async Task ObtenerPedidoPorId_Inexistente_RetornaNull()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var pedidoService = new PedidoService(context);

            // Act
            var resultado = await pedidoService.ObtenerPorIdAsync(999); // ID que no existe

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task CrearPedido_ClienteInexistente_LanzaExcepcion()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var producto = new Domain.Entities.Producto { Nombre = "Tablet", Descripcion = "10 pulgadas", Precio = 300.00m, Stock = 5 };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();

            var pedidoService = new PedidoService(context);
            var dto = new CrearPedidoDto { ClienteId = 999, ProductoId = producto.Id, Cantidad = 1 }; // Cliente 999 no existe

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => pedidoService.CrearAsync(dto));
            Assert.Contains("cliente especificado no existe", exception.Message);
        }

        [Fact]
        public async Task CrearPedido_ProductoInexistente_LanzaExcepcion()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var cliente = new Domain.Entities.Cliente { Nombre = "Sofía", Apellido = "Ríos", Email = "sofia@test.com" };
            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();

            var pedidoService = new PedidoService(context);
            var dto = new CrearPedidoDto { ClienteId = cliente.Id, ProductoId = 999, Cantidad = 1 }; // Producto 999 no existe

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => pedidoService.CrearAsync(dto));
            Assert.Contains("producto especificado no existe", exception.Message);
        }
    }
}