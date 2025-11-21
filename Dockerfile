# Usar imagen base de .NET 8.0 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos del proyecto
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos y compilar
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Usar imagen runtime de .NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Exponer puerto (Railway inyecta PORT automáticamente)
EXPOSE 8080

ENTRYPOINT ["dotnet", "Control-de-Parqueo.dll"]

