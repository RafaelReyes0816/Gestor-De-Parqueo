-- Script de diagnóstico para verificar el trigger de recompensas
-- Ejecutar este script en Supabase SQL Editor

-- 1. Verificar si el trigger existe
SELECT 
    trigger_name,
    event_manipulation,
    event_object_table,
    action_statement,
    action_timing
FROM information_schema.triggers
WHERE trigger_name = 'trigger_verificar_recompensa';

-- 2. Verificar si la función existe
SELECT 
    routine_name,
    routine_type,
    routine_definition
FROM information_schema.routines
WHERE routine_name = 'verificar_recompensa';

-- 3. Verificar registros que deberían tener recompensa pero no la tienen
SELECT 
    r.id as registro_id,
    r.vehiculo_id,
    v.placa,
    r.hora_entrada,
    r.hora_salida,
    r.horas_totales,
    r.estado,
    CASE 
        WHEN r.horas_totales <= 15 THEN 'Limpiavidrios'
        WHEN r.horas_totales <= 20 THEN 'Vaselina'
        ELSE 'Kit de Limpieza'
    END as tipo_recompensa_esperada,
    CASE 
        WHEN EXISTS (SELECT 1 FROM recompensas rec WHERE rec.registro_parqueo_id = r.id) 
        THEN 'Sí' 
        ELSE 'No' 
    END as tiene_recompensa,
    rec.id as recompensa_id,
    rec.tipo_recompensa as recompensa_tipo
FROM registros_parqueo r
LEFT JOIN vehiculos v ON v.id = r.vehiculo_id
LEFT JOIN recompensas rec ON rec.registro_parqueo_id = r.id
WHERE r.hora_salida IS NOT NULL 
    AND r.horas_totales IS NOT NULL
    AND r.horas_totales > 10
ORDER BY r.horas_totales DESC;

-- 4. Verificar la estructura de la tabla recompensas
SELECT 
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_name = 'recompensas'
ORDER BY ordinal_position;

-- 5. Contar recompensas existentes
SELECT COUNT(*) as total_recompensas FROM recompensas;

-- 6. Ver las últimas recompensas creadas
SELECT 
    r.id,
    r.vehiculo_id,
    v.placa,
    r.tipo_recompensa,
    r.horas_acumuladas,
    r.fecha_recompensa,
    r.registro_parqueo_id
FROM recompensas r
LEFT JOIN vehiculos v ON v.id = r.vehiculo_id
ORDER BY r.fecha_recompensa DESC
LIMIT 10;

