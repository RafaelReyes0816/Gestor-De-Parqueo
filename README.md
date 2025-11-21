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

## Despliegue en Railway

1. Conecta tu repositorio a Railway (https://railway.app)
2. Railway detectará automáticamente que es un proyecto .NET
3. Configura las variables de entorno en Railway:
   - `ASPNETCORE_ENVIRONMENT`: `Production`
   - `Supabase__ProjectUrl`: URL de tu proyecto Supabase
   - `Supabase__ApiKey`: API Key de Supabase
4. Railway desplegará automáticamente

**Ver `Agent/COPIAR_PEGAR_RAILWAY.md` para instrucciones detalladas con valores exactos.**

## Base de Datos

Ejecuta el script `Agent/schema.sql` en Supabase SQL Editor para crear las tablas, triggers y funciones necesarias.

## Funcionalidades

- Registro de entrada de vehículos
- Registro de salida de vehículos (con hora manual)
- Visualización de vehículos activos
- Historial de vehículos
- Sistema automático de recompensas (más de 10 horas)

