-- Script para verificar permisos y configuración de Supabase
-- Ejecutar este script en Supabase SQL Editor

-- 1. Verificar que las tablas existen
SELECT 
    table_name,
    table_type
FROM information_schema.tables
WHERE table_schema = 'public'
    AND table_name IN ('vehiculos', 'registros_parqueo', 'recompensas')
ORDER BY table_name;

-- 2. Verificar permisos en la tabla recompensas
SELECT 
    grantee,
    privilege_type
FROM information_schema.role_table_grants
WHERE table_name = 'recompensas';

-- 3. Verificar si hay políticas RLS (Row Level Security) activas
SELECT 
    schemaname,
    tablename,
    policyname,
    permissive,
    roles,
    cmd,
    qual
FROM pg_policies
WHERE tablename IN ('registros_parqueo', 'recompensas');

-- 4. Verificar secuencias (para IDs autoincrementales)
SELECT 
    sequence_name,
    last_value
FROM information_schema.sequences
WHERE sequence_name LIKE '%recompensas%' OR sequence_name LIKE '%id_seq%';

-- 5. Verificar constraints y foreign keys
SELECT
    tc.table_name,
    tc.constraint_name,
    tc.constraint_type,
    kcu.column_name,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
LEFT JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
WHERE tc.table_name IN ('recompensas', 'registros_parqueo')
ORDER BY tc.table_name, tc.constraint_type;

