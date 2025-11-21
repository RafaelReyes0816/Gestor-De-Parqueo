-- Script para verificar y asignar recompensas pendientes
-- Ejecutar este script en Supabase SQL Editor para asignar recompensas a registros que ya tienen salida pero no tienen recompensa

-- Función para verificar y asignar recompensas pendientes
CREATE OR REPLACE FUNCTION verificar_recompensas_pendientes()
RETURNS TABLE(
    registro_id INTEGER,
    vehiculo_id INTEGER,
    horas_totales NUMERIC,
    tipo_recompensa VARCHAR,
    recompensa_asignada BOOLEAN
) AS $$
DECLARE
    rec RECORD;
    tipo_recompensa VARCHAR(50);
    existe_recompensa BOOLEAN;
BEGIN
    -- Buscar todos los registros que tienen salida, más de 10 horas, pero no tienen recompensa
    FOR rec IN 
        SELECT 
            r.id,
            r.vehiculo_id,
            r.horas_totales
        FROM registros_parqueo r
        WHERE r.hora_salida IS NOT NULL 
            AND r.horas_totales IS NOT NULL
            AND r.horas_totales > 10
            AND NOT EXISTS (
                SELECT 1 FROM recompensas rec 
                WHERE rec.registro_parqueo_id = r.id
            )
    LOOP
        -- Determinar tipo de recompensa según las horas
        tipo_recompensa := CASE 
            WHEN rec.horas_totales <= 15 THEN 'Limpiavidrios'
            WHEN rec.horas_totales <= 20 THEN 'Vaselina'
            ELSE 'Kit de Limpieza'
        END;
        
        -- Insertar la recompensa
        INSERT INTO recompensas (vehiculo_id, registro_parqueo_id, tipo_recompensa, horas_acumuladas)
        VALUES (rec.vehiculo_id, rec.id, tipo_recompensa, rec.horas_totales)
        ON CONFLICT DO NOTHING;
        
        -- Retornar información
        registro_id := rec.id;
        vehiculo_id := rec.vehiculo_id;
        horas_totales := rec.horas_totales;
        tipo_recompensa := tipo_recompensa;
        recompensa_asignada := TRUE;
        
        RETURN NEXT;
    END LOOP;
    
    RETURN;
END;
$$ LANGUAGE plpgsql;

-- Ejecutar la función para asignar recompensas pendientes
SELECT * FROM verificar_recompensas_pendientes();

-- También podemos crear una función más simple que solo asigne sin retornar datos
CREATE OR REPLACE FUNCTION asignar_recompensas_pendientes()
RETURNS INTEGER AS $$
DECLARE
    registros_actualizados INTEGER := 0;
    rec RECORD;
    tipo_recompensa VARCHAR(50);
BEGIN
    -- Buscar y asignar recompensas pendientes
    FOR rec IN 
        SELECT 
            r.id,
            r.vehiculo_id,
            r.horas_totales
        FROM registros_parqueo r
        WHERE r.hora_salida IS NOT NULL 
            AND r.horas_totales IS NOT NULL
            AND r.horas_totales > 10
            AND NOT EXISTS (
                SELECT 1 FROM recompensas rec 
                WHERE rec.registro_parqueo_id = r.id
            )
    LOOP
        -- Determinar tipo de recompensa
        tipo_recompensa := CASE 
            WHEN rec.horas_totales <= 15 THEN 'Limpiavidrios'
            WHEN rec.horas_totales <= 20 THEN 'Vaselina'
            ELSE 'Kit de Limpieza'
        END;
        
        -- Insertar la recompensa
        INSERT INTO recompensas (vehiculo_id, registro_parqueo_id, tipo_recompensa, horas_acumuladas)
        VALUES (rec.vehiculo_id, rec.id, tipo_recompensa, rec.horas_totales);
        
        registros_actualizados := registros_actualizados + 1;
    END LOOP;
    
    RETURN registros_actualizados;
END;
$$ LANGUAGE plpgsql;

-- Ejecutar para asignar todas las recompensas pendientes
SELECT asignar_recompensas_pendientes() AS recompensas_asignadas;

