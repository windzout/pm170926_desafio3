# Etapa 1: Compilación y Publicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos
COPY ["desafio03.sln", "./"]
COPY ["src/desafio03.Api/desafio03.Api.csproj", "src/desafio03.Api/"]
COPY ["src/desafio03.Application/desafio03.Application.csproj", "src/desafio03.Application/"]
COPY ["src/desafio03.Domain/desafio03.Domain.csproj", "src/desafio03.Domain/"]
COPY ["src/desafio03.Infrastructure/desafio03.Infrastructure.csproj", "src/desafio03.Infrastructure/"]
COPY ["tests/desafio03.Tests/desafio03.Tests.csproj", "tests/desafio03.Tests/"]

# Restaurar dependencias
RUN dotnet restore "desafio03.sln"

# Copiar todo el código fuente restante
COPY . .
WORKDIR "/src/src/desafio03.Api"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Imagen final para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "desafio03.Api.dll"]