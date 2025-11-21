# Control de Parqueo UPDS

Sistema de control de parqueo desarrollado con ASP.NET Core y Supabase.

## Requisitos

- .NET 8.0 SDK
- Cuenta de Supabase

## Configuración Local

1. Clonar el repositorio
2. Configurar `appsettings.json` con tus credenciales de Supabase:
```json
{
  "Supabase": {
    "ProjectUrl": "https://tu-proyecto.supabase.co",
    "ApiKey": "tu-api-key"
  }
}
```

3. Ejecutar el proyecto:
```bash
dotnet run
```

## Despliegue en Render

### Opción 1: Usando render.yaml (Recomendado)

1. Conecta tu repositorio a Render
2. Render detectará automáticamente el archivo `render.yaml`
3. Configura las variables de entorno en Render:
   - `Supabase__ProjectUrl`: URL de tu proyecto Supabase
   - `Supabase__ApiKey`: API Key de Supabase

### Opción 2: Configuración Manual

1. Crea un nuevo Web Service en Render
2. Configura:
   - **Environment**: `dotnet`
   - **Build Command**: `dotnet restore && dotnet publish -c Release -o ./publish`
   - **Start Command**: `cd ./publish && dotnet Control-de-Parqueo.dll`
3. Agrega las variables de entorno:
   - `ASPNETCORE_ENVIRONMENT`: `Production`
   - `Supabase__ProjectUrl`: Tu URL de Supabase
   - `Supabase__ApiKey`: Tu API Key de Supabase

## Base de Datos

Ejecuta el script `Agent/schema.sql` en Supabase SQL Editor para crear las tablas, triggers y funciones necesarias.

## Funcionalidades

- Registro de entrada de vehículos
- Registro de salida de vehículos (con hora manual)
- Visualización de vehículos activos
- Historial de vehículos
- Sistema automático de recompensas (más de 10 horas)

