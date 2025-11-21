-- Script para corregir el trigger de recompensas
-- Ejecutar este script en Supabase SQL Editor

-- Eliminar el trigger anterior
DROP TRIGGER IF EXISTS trigger_verificar_recompensa ON registros_parqueo;

-- Función mejorada para verificar y otorgar recompensa si supera las 10 horas
CREATE OR REPLACE FUNCTION verificar_recompensa()
RETURNS TRIGGER AS $$
DECLARE
    horas NUMERIC(10, 2);
    tipo_recompensa VARCHAR(50);
    existe_recompensa BOOLEAN;
BEGIN
    -- Solo procesar cuando se registra la salida
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
                -- Asignar tipo de recompensa (puedes personalizar la lógica aquí)
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

-- Recrear el trigger para verificar recompensas automáticamente
-- Este trigger se ejecuta cuando:
-- 1. Se registra la salida por primera vez (hora_salida cambia de NULL a NOT NULL)
-- 2. Se actualiza manualmente hora_salida o horas_totales
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

-- Agregar comentario
COMMENT ON FUNCTION verificar_recompensa() IS 'Verifica si un vehículo superó las 10 horas y otorga recompensa automáticamente. Se ejecuta cuando se registra salida o se actualiza manualmente.';

