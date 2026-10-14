using Microsoft.EntityFrameworkCore;
using desafio03.Infrastructure.Data;
using desafio03.Application.Interfaces;
using desafio03.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios al contenedor de dependencias.
builder.Services.AddControllers();

// 2. Configurar Entity Framework Core con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("desafio03.Infrastructure")
    ));

// 3. Registrar los servicios de la capa Application (Inyección de Dependencias)
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Configurar el pipeline de solicitudes HTTP.
// Permitir Swagger siempre (incluso en producción) para facilitar la revisión y pruebas en Docker / PDF
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();