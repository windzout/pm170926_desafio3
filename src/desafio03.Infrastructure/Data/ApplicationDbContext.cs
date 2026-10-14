using Microsoft.EntityFrameworkCore;
using desafio03.Domain.Entities;

namespace desafio03.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración adicional de relaciones con Fluent API (Opcional pero recomendado)

            // Relación Cliente -> Pedidos (1 a N)
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict); // Evitar borrado en cascada accidental de clientes con pedidos

            // Relación Producto -> Pedidos (1 a N)
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Producto)
                .WithMany(prod => prod.Pedidos)
                .HasForeignKey(p => p.ProductoId)
                .OnDelete(DeleteBehavior.Restrict); // Evitar borrado en cascada accidental de productos con pedidos

            // Precisión para decimales en Producto (Precio)
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");
        }
    }
}