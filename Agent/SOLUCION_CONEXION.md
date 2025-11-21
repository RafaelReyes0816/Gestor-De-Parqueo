# Solución al Problema de Conexión con Supabase

## Problema Identificado

El error de timeout indica que **el puerto 5432 no es accesible desde tu red local**. Esto puede deberse a:

1. **Firewall bloqueando el puerto 5432**
2. **Restricciones de red corporativa/universitaria**
3. **Supabase requiere whitelist de IPs** (en algunos planes)

## Soluciones

### Opción 1: Verificar Configuración de Supabase (RECOMENDADO)

1. Ve a tu proyecto en Supabase: https://supabase.com/dashboard
2. Ve a **Settings** → **Database**
3. Busca la sección **Connection Pooling** o **Connection string**
4. **IMPORTANTE**: Verifica si hay una opción de **"Allow connections from anywhere"** o configuración de IPs permitidas
5. Si hay restricciones de IP, agrega tu IP actual o permite todas las conexiones temporalmente

### Opción 2: Usar Connection Pooling (Puerto 6543)

Ya está configurado en `appsettings.json` con el puerto 6543. Este puerto suele estar menos restringido.

**Username para Connection Pooling**: `postgres.qpsfdivuveocpnqcxobr` (ya configurado)

### Opción 3: Verificar Firewall Local

Si estás en Linux, verifica si hay un firewall activo:

```bash
# Verificar si hay firewall activo
sudo ufw status
# O
sudo iptables -L

# Si es necesario, permitir conexiones salientes (NO recomendado para producción)
# Solo para pruebas locales
```

### Opción 4: Usar VPN o Red Diferente

Si estás en una red corporativa/universitaria que bloquea puertos:
- Intenta desde otra red (móvil, casa, etc.)
- O usa una VPN

### Opción 5: Verificar en Supabase Dashboard

1. Ve a **Settings** → **Database** → **Connection string**
2. Copia la cadena de conexión **completa** que Supabase proporciona
3. Úsala directamente en `appsettings.json`

**Formato que Supabase proporciona:**
```
postgresql://postgres.qpsfdivuveocpnqcxobr:[YOUR-PASSWORD]@aws-0-us-west-1.pooler.supabase.com:6543/postgres
```

### Opción 6: Probar desde Supabase SQL Editor

1. Ve a **SQL Editor** en Supabase
2. Ejecuta una consulta simple: `SELECT 1;`
3. Si funciona ahí, el problema es de conectividad desde tu máquina local

## Configuración Actual

La aplicación está configurada para usar:
- **Puerto**: 6543 (Connection Pooling)
- **Username**: `postgres.qpsfdivuveocpnqcxobr`
- **Password**: `qLyPAFqOEvPuaUk`
- **Database**: `postgres`
- **SSL**: Requerido

## Próximos Pasos

1. **Verifica en Supabase** si hay restricciones de IP
2. **Prueba desde otra red** (móvil, casa) para confirmar si es problema de red
3. **Copia la cadena de conexión completa** de Supabase y úsala directamente
4. Si nada funciona, considera usar la **API REST de Supabase** en lugar de conexión directa a PostgreSQL

## Nota Importante

Si estás en una red universitaria/corporativa, es muy probable que el puerto 5432 esté bloqueado por políticas de seguridad. En ese caso:
- Usa Connection Pooling (puerto 6543) - ya configurado
- O solicita a tu administrador de red que permita conexiones salientes a ese puerto
- O desarrolla desde otra red

