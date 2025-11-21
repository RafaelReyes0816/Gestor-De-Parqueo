# Cómo Verificar la Cadena de Conexión en Supabase

## Pasos para Obtener la Cadena de Conexión Correcta

1. **Ve a tu proyecto en Supabase**
   - https://supabase.com/dashboard
   - Selecciona tu proyecto

2. **Ve a Settings → Database**
   - En el menú lateral izquierdo, haz clic en "Settings" (Configuración)
   - Luego haz clic en "Database" (Base de datos)

3. **Busca la sección "Connection string"**
   - Deberías ver una sección que dice "Connection string" o "Connection info"
   - Puede estar en formato URI o en formato de parámetros

4. **Copia la cadena completa**
   - Debería verse algo como:
     ```
     postgresql://postgres:[YOUR-PASSWORD]@db.qpsfdivuveocpnqcxobr.supabase.co:5432/postgres
     ```
   - O en formato de parámetros:
     ```
     Host=db.qpsfdivuveocpnqcxobr.supabase.co
     Port=5432
     Database=postgres
     Username=postgres
     Password=[YOUR-PASSWORD]
     ```

5. **IMPORTANTE: Verifica el HOST**
   - Puede ser `qpsfdivuveocpnqcxobr.supabase.co` 
   - O puede ser `db.qpsfdivuveocpnqcxobr.supabase.co` (con prefijo "db.")
   - O puede ser una IP diferente

6. **Verifica si hay restricciones de IP**
   - Busca una sección que diga "Network restrictions" o "IP Allowlist"
   - Si hay restricciones, agrega tu IP o permite todas las conexiones

## Si No Encuentras la Cadena de Conexión

1. **Busca "Connection info" o "Database URL"**
2. **O busca "Connection pooling"** (aunque dijiste que no está)
3. **O ve a "Project Settings" → "Database"**

## Formato Actual en appsettings.json

Actualmente está configurado así:
```
Host=qpsfdivuveocpnqcxobr.supabase.co
Port=5432
Database=postgres
Username=postgres
Password=qLyPAFqOEvPuaUk
```

**Verifica especialmente:**
- ¿El HOST es correcto? (puede necesitar el prefijo "db.")
- ¿El PORT es 5432?
- ¿El USERNAME es "postgres"?
- ¿Hay alguna configuración de red que esté bloqueando?

## Alternativa: Usar la API REST

Si el puerto 5432 está bloqueado por tu red, podemos cambiar la implementación para usar la API REST de Supabase (que funciona sobre HTTPS y no requiere puertos especiales). Esto requeriría cambiar el servicio, pero funcionaría desde cualquier red.

