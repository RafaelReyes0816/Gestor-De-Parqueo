-- Script completo para recrear el trigger de recompensas desde cero
-- Ejecutar este script en Supabase SQL Editor

-- Paso 1: Eliminar triggers y funciones existentes
DROP TRIGGER IF EXISTS trigger_verificar_recompensa ON registros_parqueo;
DROP TRIGGER IF EXISTS trigger_verificar_recompensa_actualizacion ON registros_parqueo;
DROP FUNCTION IF EXISTS verificar_recompensa();
DROP FUNCTION IF EXISTS verificar_recompensa_actualizacion();

-- Paso 2: Crear la función mejorada
CREATE OR REPLACE FUNCTION verificar_recompensa()
RETURNS TRIGGER AS $$
DECLARE
    horas NUMERIC(10, 2);
    tipo_recompensa VARCHAR(50);
    existe_recompensa BOOLEAN;
BEGIN
    -- Solo procesar cuando tiene salida y horas calculadas
    IF NEW.hora_salida IS NOT NULL AND NEW.horas_totales IS NOT NULL THEN
        horas := NEW.horas_totales;
        
        -- Si supera las 10 horas, otorgar recompensa
        IF horas > 10 THEN
            -- Verificar si ya existe una recompensa para este registro
            SELECT EXISTS(
                SELECT 1 FROM recompensas 
                WHERE registro_parqueo_id = NEW.id
            ) INTO existe_recompensa;
            
            -- Solo insertar si no existe ya una recompensa para este registro
            IF NOT existe_recompensa THEN
                -- Asignar tipo de recompensa según las horas
                tipo_recompensa := CASE 
                    WHEN horas <= 15 THEN 'Limpiavidrios'
                    WHEN horas <= 20 THEN 'Vaselina'
                    ELSE 'Kit de Limpieza'
                END;
                
                -- Insertar recompensa
                INSERT INTO recompensas (vehiculo_id, registro_parqueo_id, tipo_recompensa, horas_acumuladas)
                VALUES (NEW.vehiculo_id, NEW.id, tipo_recompensa, horas);
            END IF;
        END IF;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Paso 3: Crear el trigger que se ejecuta cuando se actualiza el registro
CREATE TRIGGER trigger_verificar_recompensa
    AFTER UPDATE ON registros_parqueo
    FOR EACH ROW
    WHEN (
        NEW.hora_salida IS NOT NULL 
        AND NEW.horas_totales IS NOT NULL 
        AND NEW.horas_totales > 10
        AND (
            -- Caso 1: Se registra salida por primera vez
            (OLD.hora_salida IS NULL AND NEW.hora_salida IS NOT NULL)
            OR
            -- Caso 2: Se actualiza manualmente hora_salida
            (OLD.hora_salida IS NOT NULL AND NEW.hora_salida IS NOT NULL AND OLD.hora_salida != NEW.hora_salida)
            OR
            -- Caso 3: Se actualiza manualmente horas_totales
            (OLD.horas_totales IS NULL OR OLD.horas_totales != NEW.horas_totales)
        )
    )
    EXECUTE FUNCTION verificar_recompensa();

-- Paso 4: Verificar que se creó correctamente
SELECT 
    'Trigger creado' as estado,
    trigger_name,
    event_manipulation,
    action_timing
FROM information_schema.triggers
WHERE trigger_name = 'trigger_verificar_recompensa';

SELECT 
    'Función creada' as estado,
    routine_name,
    routine_type
FROM information_schema.routines
WHERE routine_name = 'verificar_recompensa';

-- Paso 5: Probar el trigger manualmente con un registro de prueba
-- (Descomenta y ajusta el ID si quieres probar)
/*
UPDATE registros_parqueo 
SET hora_salida = CURRENT_TIMESTAMP,
    horas_totales = 12.5
WHERE id = 1;  -- Cambia el ID por uno real

-- Verificar si se creó la recompensa
SELECT * FROM recompensas WHERE registro_parqueo_id = 1;
*/

