-- SOLUCIÓN COMPLETA PARA EL TRIGGER DE RECOMPENSAS
-- Ejecutar este script completo en Supabase SQL Editor

-- ============================================
-- PASO 1: Limpiar todo lo anterior
-- ============================================
DROP TRIGGER IF EXISTS trigger_verificar_recompensa ON registros_parqueo;
DROP TRIGGER IF EXISTS trigger_verificar_recompensa_actualizacion ON registros_parqueo;
DROP FUNCTION IF EXISTS verificar_recompensa() CASCADE;
DROP FUNCTION IF EXISTS verificar_recompensa_actualizacion() CASCADE;

-- ============================================
-- PASO 2: Crear la función del trigger
-- ============================================
CREATE OR REPLACE FUNCTION verificar_recompensa()
RETURNS TRIGGER 
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    horas NUMERIC(10, 2);
    tipo_recompensa VARCHAR(50);
    existe_recompensa BOOLEAN;
BEGIN
    -- Verificar condiciones básicas
    IF NEW.hora_salida IS NOT NULL AND NEW.horas_totales IS NOT NULL THEN
        horas := NEW.horas_totales;
        
        -- Si supera las 10 horas
        IF horas > 10 THEN
            -- Verificar si ya existe recompensa
            SELECT EXISTS(
                SELECT 1 FROM recompensas 
                WHERE registro_parqueo_id = NEW.id
            ) INTO existe_recompensa;
            
            -- Solo crear si no existe
            IF NOT existe_recompensa THEN
                -- Determinar tipo de recompensa
                tipo_recompensa := CASE 
                    WHEN horas <= 15 THEN 'Limpiavidrios'
                    WHEN horas <= 20 THEN 'Vaselina'
                    ELSE 'Kit de Limpieza'
                END;
                
                -- Insertar la recompensa
                INSERT INTO recompensas (
                    vehiculo_id, 
                    registro_parqueo_id, 
                    tipo_recompensa, 
                    horas_acumuladas,
                    fecha_recompensa
                )
                VALUES (
                    NEW.vehiculo_id, 
                    NEW.id, 
                    tipo_recompensa, 
                    horas,
                    CURRENT_TIMESTAMP
                );
            END IF;
        END IF;
    END IF;
    
    RETURN NEW;
END;
$$;

-- ============================================
-- PASO 3: Crear el trigger
-- ============================================
CREATE TRIGGER trigger_verificar_recompensa
    AFTER UPDATE ON registros_parqueo
    FOR EACH ROW
    WHEN (
        NEW.hora_salida IS NOT NULL 
        AND NEW.horas_totales IS NOT NULL 
        AND NEW.horas_totales > 10
    )
    EXECUTE FUNCTION verificar_recompensa();

-- ============================================
-- PASO 4: Verificar que se creó correctamente
-- ============================================
SELECT 
    '✅ Trigger creado' as resultado,
    trigger_name,
    event_manipulation,
    action_timing
FROM information_schema.triggers
WHERE trigger_name = 'trigger_verificar_recompensa';

SELECT 
    '✅ Función creada' as resultado,
    routine_name
FROM information_schema.routines
WHERE routine_name = 'verificar_recompensa';

-- ============================================
-- PASO 5: Verificar políticas RLS (si están activas)
-- ============================================
-- Si Row Level Security está activo, necesitamos desactivarlo o crear políticas
-- Ejecuta esto si el trigger no funciona:

-- Desactivar RLS temporalmente (solo si es necesario)
-- ALTER TABLE recompensas DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE registros_parqueo DISABLE ROW LEVEL SECURITY;

-- O crear políticas que permitan al trigger insertar:
/*
CREATE POLICY "Permitir trigger insertar recompensas"
ON recompensas
FOR INSERT
TO authenticated, anon, service_role
WITH CHECK (true);
*/

-- ============================================
-- PASO 6: Probar el trigger manualmente
-- ============================================
-- Descomenta y ajusta el ID para probar:
/*
-- Buscar un registro para probar
SELECT id, vehiculo_id, hora_salida, horas_totales 
FROM registros_parqueo 
WHERE hora_salida IS NOT NULL 
    AND horas_totales > 10
LIMIT 1;

-- Actualizar el registro (cambia el ID por uno real)
UPDATE registros_parqueo 
SET horas_totales = 12.5,
    updated_at = CURRENT_TIMESTAMP
WHERE id = 1;  -- Cambia por un ID real

-- Verificar si se creó la recompensa
SELECT * FROM recompensas 
WHERE registro_parqueo_id = 1;  -- Cambia por el mismo ID
*/

