# Configuración del Sistema de Control de Parqueo

## ✅ Pasos Completados

1. ✅ Script SQL creado y ejecutado en Supabase
2. ✅ Modelos de datos creados (Vehiculo, RegistroParqueo, Recompensa)
3. ✅ Servicio de parqueo implementado
4. ✅ Controladores y vistas creados
5. ✅ Interfaz de usuario completa

## 🔧 Configuración Pendiente

### 1. Obtener la Cadena de Conexión Completa

Para completar la configuración, necesitas obtener la cadena de conexión completa de Supabase:

1. Ve a tu proyecto en Supabase: https://supabase.com/dashboard
2. Selecciona tu proyecto
3. Ve a **Settings** → **Database**
4. Busca la sección **Connection string** o **Connection pooling**
5. Copia la cadena de conexión completa (URI o Connection string)

**Importante:** Supabase proporciona la cadena de conexión completa que incluye:
- El nombre correcto de la base de datos (generalmente `postgres` pero puede variar)
- El host, puerto, usuario y contraseña correctos
- Los parámetros SSL necesarios

### 2. Actualizar appsettings.json

**Opción A: Usar la cadena de conexión completa de Supabase**

Copia la cadena de conexión completa de Supabase y úsala directamente:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "LA_CADENA_COMPLETA_DE_SUPABASE"
  }
}
```

**Opción B: Construir manualmente (si conoces todos los valores)**

Si prefieres construirla manualmente, el formato es:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=qpsfdivuveocpnqcxobr.supabase.co;Port=5432;Database=NOMBRE_DE_TU_BD;Username=postgres;Password='TU_PASSWORD';SslMode=Require;TrustServerCertificate=True"
  }
}
```

**Nota sobre el nombre de la base de datos:**
- En Supabase, la base de datos por defecto generalmente se llama `postgres`
- Si creaste una base de datos con otro nombre, usa ese nombre
- Verifica el nombre exacto en la cadena de conexión que Supabase proporciona

### 3. Probar la Conexión

Una vez configurada la cadena de conexión, ejecuta:

```bash
dotnet run
```

Y navega a `https://localhost:5001` (o el puerto que te indique)

## 📋 Funcionalidades Implementadas

### 1. Registro de Vehículos
- Registro automático al ingresar una placa
- Validación de formato de placa
- Almacenamiento en base de datos

### 2. Control de Horarios
- Registro de hora de entrada
- Registro de hora de salida
- Cálculo automático de horas totales (mediante trigger en BD)
- Actualización automática del estado a "Fuera del Parqueo"

### 3. Sistema de Recompensas
- Cálculo automático de horas permanecidas
- Otorgamiento automático de recompensa si supera 10 horas
- Tipos de recompensa según horas:
  - 10-15 horas: Limpiavidrios
  - 15-20 horas: Vaselina
  - Más de 20 horas: Kit de Limpieza

## 🎯 Páginas Disponibles

1. **Inicio** (`/`) - Dashboard principal con acceso a todas las funcionalidades
2. **Registro de Entrada** (`/Parqueo/Entrada`) - Registrar entrada de vehículo
3. **Registro de Salida** (`/Parqueo/Salida`) - Registrar salida de vehículo
4. **Vehículos Activos** (`/Parqueo/Activos`) - Lista de vehículos actualmente en el parqueo
5. **Historial** (`/Parqueo/Historial`) - Consultar historial y recompensas de un vehículo

## 🚀 Despliegue en Render

Para desplegar en Render:

1. Crea un nuevo servicio Web Service en Render
2. Conecta tu repositorio de Git
3. Configura:
   - **Build Command:** `dotnet restore && dotnet publish -c Release -o ./publish`
   - **Start Command:** `cd publish && dotnet Control-de-Parqueo.dll`
4. Agrega la variable de entorno `ASPNETCORE_ENVIRONMENT=Production`
5. Actualiza la cadena de conexión en `appsettings.json` o usa variables de entorno

## ⚠️ Importante

- La contraseña de la base de datos debe mantenerse segura
- No subas `appsettings.json` con la contraseña real a repositorios públicos
- Para producción, usa variables de entorno o secretos de Render

## 📝 Notas Técnicas

- El sistema usa Entity Framework Core con PostgreSQL
- Los triggers en la base de datos calculan automáticamente las horas y otorgan recompensas
- La interfaz usa Bootstrap 5 con Bootstrap Icons
- Todas las placas se convierten automáticamente a mayúsculas

