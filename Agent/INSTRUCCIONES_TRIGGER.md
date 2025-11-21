# Instrucciones para Solucionar el Trigger de Recompensas

## Problema
El trigger no está creando recompensas cuando se actualiza un registro manualmente o cuando se registra una salida.

## Solución Paso a Paso

### 1. Ejecutar Diagnóstico
Primero ejecuta `diagnostico_trigger.sql` en Supabase SQL Editor para ver:
- Si el trigger existe
- Si la función existe
- Qué registros deberían tener recompensa pero no la tienen

### 2. Verificar Permisos
Ejecuta `verificar_permisos.sql` para verificar:
- Si Row Level Security (RLS) está bloqueando las inserciones
- Si hay políticas que impiden insertar recompensas

### 3. Recrear el Trigger
Ejecuta `solucion_trigger.sql` que:
- Elimina triggers y funciones antiguas
- Crea la función con `SECURITY DEFINER` (importante para que funcione con RLS)
- Crea el trigger que se ejecuta en cualquier actualización

### 4. Si Row Level Security está activo
Si Supabase tiene RLS activado, necesitas:

**Opción A: Desactivar RLS (más simple)**
```sql
ALTER TABLE recompensas DISABLE ROW LEVEL SECURITY;
ALTER TABLE registros_parqueo DISABLE ROW LEVEL SECURITY;
```

**Opción B: Crear políticas (más seguro)**
```sql
-- Política para permitir que el trigger inserte recompensas
CREATE POLICY "Permitir trigger insertar recompensas"
ON recompensas
FOR INSERT
TO authenticated, anon, service_role
WITH CHECK (true);

-- Política para permitir actualizar registros
CREATE POLICY "Permitir actualizar registros"
ON registros_parqueo
FOR UPDATE
TO authenticated, anon, service_role
USING (true)
WITH CHECK (true);
```

### 5. Probar el Trigger
Después de ejecutar el script, prueba actualizando un registro:

```sql
-- Buscar un registro con más de 10 horas
SELECT id, vehiculo_id, horas_totales 
FROM registros_parqueo 
WHERE hora_salida IS NOT NULL 
    AND horas_totales > 10
    AND NOT EXISTS (SELECT 1 FROM recompensas WHERE registro_parqueo_id = registros_parqueo.id)
LIMIT 1;

-- Actualizar para activar el trigger (cambia el ID)
UPDATE registros_parqueo 
SET horas_totales = horas_totales,  -- Forzar actualización
    updated_at = CURRENT_TIMESTAMP
WHERE id = 1;  -- Cambia por un ID real

-- Verificar si se creó la recompensa
SELECT * FROM recompensas WHERE registro_parqueo_id = 1;
```

## Notas Importantes

1. **SECURITY DEFINER**: La función usa `SECURITY DEFINER` para que tenga permisos suficientes incluso con RLS activo.

2. **El trigger se ejecuta en cualquier actualización**: Ahora se ejecuta siempre que se actualice un registro con `hora_salida` y `horas_totales > 10`, sin importar qué campo se actualice.

3. **Verificación de duplicados**: El trigger verifica si ya existe una recompensa antes de insertar, evitando duplicados.

4. **Si aún no funciona**: Verifica los logs de Supabase o ejecuta el diagnóstico para ver qué está pasando.

