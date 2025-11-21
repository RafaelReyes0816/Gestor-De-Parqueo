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


## Base de Datos

Ejecuta el script `Agent/schema.sql` en Supabase SQL Editor para crear las tablas, triggers y funciones necesarias.

## Funcionalidades

- Registro de entrada de vehículos
- Registro de salida de vehículos (con hora manual)
- Visualización de vehículos activos
- Historial de vehículos
- Sistema automático de recompensas (más de 10 horas)

