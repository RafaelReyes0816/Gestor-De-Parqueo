-- Script SQL para Sistema de Control de Parqueo UPDS
-- Base de datos: Supabase (PostgreSQL)

-- Tabla de Vehículos
CREATE TABLE IF NOT EXISTS vehiculos (
    id SERIAL PRIMARY KEY,
    placa VARCHAR(10) UNIQUE NOT NULL,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla de Registros de Parqueo (Entradas y Salidas)
CREATE TABLE IF NOT EXISTS registros_parqueo (
    id SERIAL PRIMARY KEY,
    vehiculo_id INTEGER NOT NULL REFERENCES vehiculos(id) ON DELETE CASCADE,
    hora_entrada TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    hora_salida TIMESTAMP NULL,
    horas_totales NUMERIC(10, 2) NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'Dentro' CHECK (estado IN ('Dentro', 'Fuera')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla de Recompensas
CREATE TABLE IF NOT EXISTS recompensas (
    id SERIAL PRIMARY KEY,
    vehiculo_id INTEGER NOT NULL REFERENCES vehiculos(id) ON DELETE CASCADE,
    registro_parqueo_id INTEGER NOT NULL REFERENCES registros_parqueo(id) ON DELETE CASCADE,
    tipo_recompensa VARCHAR(50) NOT NULL,
    horas_acumuladas NUMERIC(10, 2) NOT NULL,
    fecha_recompensa TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Índices para mejorar el rendimiento
CREATE INDEX IF NOT EXISTS idx_vehiculos_placa ON vehiculos(placa);
CREATE INDEX IF NOT EXISTS idx_registros_vehiculo_id ON registros_parqueo(vehiculo_id);
CREATE INDEX IF NOT EXISTS idx_registros_estado ON registros_parqueo(estado);
CREATE INDEX IF NOT EXISTS idx_registros_hora_entrada ON registros_parqueo(hora_entrada);
CREATE INDEX IF NOT EXISTS idx_recompensas_vehiculo_id ON recompensas(vehiculo_id);

-- Función para actualizar horas_totales automáticamente cuando se registra la salida
CREATE OR REPLACE FUNCTION calcular_horas_totales()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.hora_salida IS NOT NULL AND NEW.hora_entrada IS NOT NULL THEN
        NEW.horas_totales := EXTRACT(EPOCH FROM (NEW.hora_salida - NEW.hora_entrada)) / 3600.0;
        NEW.estado := 'Fuera';
        NEW.updated_at := CURRENT_TIMESTAMP;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger para calcular horas_totales automáticamente
DROP TRIGGER IF EXISTS trigger_calcular_horas ON registros_parqueo;
CREATE TRIGGER trigger_calcular_horas
    BEFORE UPDATE ON registros_parqueo
    FOR EACH ROW
    WHEN (NEW.hora_salida IS NOT NULL AND OLD.hora_salida IS NULL)
    EXECUTE FUNCTION calcular_horas_totales();

-- Función para verificar y otorgar recompensa si supera las 10 horas
CREATE OR REPLACE FUNCTION verificar_recompensa()
RETURNS TRIGGER AS $$
DECLARE
    horas NUMERIC(10, 2);
    tipo_recompensa VARCHAR(50);
BEGIN
    -- Solo procesar cuando se registra la salida
    IF NEW.hora_salida IS NOT NULL AND NEW.horas_totales IS NOT NULL THEN
        horas := NEW.horas_totales;
        
        -- Si supera las 10 horas, otorgar recompensa
        IF horas > 10 THEN
            -- Asignar tipo de recompensa (puedes personalizar la lógica aquí)
            tipo_recompensa := CASE 
                WHEN horas <= 15 THEN 'Limpiavidrios'
                WHEN horas <= 20 THEN 'Vaselina'
                ELSE 'Kit de Limpieza'
            END;
            
            -- Insertar recompensa solo si no existe ya para este registro
            INSERT INTO recompensas (vehiculo_id, registro_parqueo_id, tipo_recompensa, horas_acumuladas)
            VALUES (NEW.vehiculo_id, NEW.id, tipo_recompensa, horas)
            ON CONFLICT DO NOTHING;
        END IF;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger para verificar recompensas automáticamente
DROP TRIGGER IF EXISTS trigger_verificar_recompensa ON registros_parqueo;
CREATE TRIGGER trigger_verificar_recompensa
    AFTER UPDATE ON registros_parqueo
    FOR EACH ROW
    WHEN (NEW.hora_salida IS NOT NULL AND OLD.hora_salida IS NULL AND NEW.horas_totales > 10)
    EXECUTE FUNCTION verificar_recompensa();

-- Comentarios en las tablas
COMMENT ON TABLE vehiculos IS 'Almacena la información de los vehículos registrados en el sistema';
COMMENT ON TABLE registros_parqueo IS 'Registra las entradas y salidas de vehículos del parqueo';
COMMENT ON TABLE recompensas IS 'Registra las recompensas otorgadas a vehículos que superan las 10 horas';

-- Comentarios en columnas importantes
COMMENT ON COLUMN vehiculos.placa IS 'Número de placa del vehículo (ejemplo: 1891ZLD)';
COMMENT ON COLUMN registros_parqueo.horas_totales IS 'Total de horas que el vehículo permaneció en el parqueo';
COMMENT ON COLUMN registros_parqueo.estado IS 'Estado actual: Dentro o Fuera del parqueo';
COMMENT ON COLUMN recompensas.tipo_recompensa IS 'Tipo de recompensa otorgada (limpiavidrios, vaselina, etc.)';

