CREATE DATABASE mdtecnologia WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Spanish_Panama.1252';

CREATE TABLE usuarios (
	id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	username VARCHAR(50) UNIQUE NOT NULL,
	password VARCHAR(100) CHECK (password LIKE '$2_$_%' AND LENGTH(password) >= 60),
	disabled BOOLEAN NOT NULL DEFAULT False,
	rol VARCHAR(50) NOT NULL,
	created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at TIMESTAMP
)

CREATE TABLE clientes (
	id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	nombre VARCHAR(100) NOT NULL,
	apellido VARCHAR(100) NOT NULL,
	correo VARCHAR(255) UNIQUE NOT NULL,
	telefono VARCHAR(15) UNIQUE,
	usuario UUID UNIQUE,
	created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at TIMESTAMP,
	FOREIGN KEY (usuario) REFERENCES usuarios(id) ON DELETE SET NULL ON UPDATE CASCADE
)

CREATE TABLE trabajadores (
    id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    correo VARCHAR(255) NOT NULL,
    telefono VARCHAR(15),
    usuario UUID,
    cargo VARCHAR(100) NOT NULL,
    fecha_ingreso DATE NOT NULL,
    estado BOOLEAN DEFAULT true NOT NULL,
    salario NUMERIC(12, 2) CHECK (salario >= 0),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP
);

CREATE TABLE provincias (
	id SERIAL PRIMARY KEY,
	nombre VARCHAR(100) NOT NULL
)

CREATE TABLE direcciones (
	id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	descripcion TEXT,
	provincia SERIAL NOT NULL,
	created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (provincia) REFERENCES provincias(id) ON UPDATE CASCADE
)

CREATE TABLE direccion_cliente (
	id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	cliente UUID NOT NULL,
	direccion UUID NOT NULL,
	created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (cliente) REFERENCES clientes(id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (direccion) REFERENCES direcciones(id) ON DELETE CASCADE ON UPDATE CASCADE
)

CREATE TABLE ventas (
    id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    estado VARCHAR(50) NOT NULL,
    cantidad_total_productos INTEGER NOT NULL CHECK (cantidad_total_productos >= 0),
    subtotal NUMERIC(12, 2) NOT NULL CHECK (subtotal >= 0),
    descuento NUMERIC(12, 2) DEFAULT 0.00 NOT NULL CHECK (descuento >= 0),
    impuesto NUMERIC(12, 2) NOT NULL CHECK (impuesto >= 0),
    total NUMERIC(12, 2) NOT NULL CHECK (total >= 0),
    direccion_entrega UUID NOT NULL,
    cliente UUID NOT NULL
);

CREATE TABLE categorias (
	id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	nombre VARCHAR(50) NOT NULL,
	descripcion TEXT,
	categoria_padre UUID,
	imagen_url text,
	FOREIGN KEY (categoria_padre) REFERENCES categorias(id) ON DELETE SET NULL ON UPDATE CASCADE
)

CREATE TABLE productos (
	id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	nombre TEXT NOT NULL,
	marca TEXT NOT NULL,
	descripcion TEXT,
	categoria UUID,
	FOREIGN KEY (categoria) REFERENCES categorias(id) ON DELETE SET NULL ON UPDATE CASCADE
)

CREATE TABLE imagenes_productos (
	id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	descripcion TEXT,
	url TEXT NOT NULL,
	producto UUID NOT NULL,
	FOREIGN KEY (producto) REFERENCES productos(id) ON UPDATE CASCADE ON DELETE SET NULL
)

CREATE TABLE detalles_venta (
    id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    cantidad INTEGER NOT NULL,
    precio_unitario NUMERIC(12, 2) NOT NULL CHECK (precio_unitario >= 0),
    subtotal NUMERIC(12, 2) NOT NULL CHECK (subtotal >= 0),
    descuento NUMERIC(12, 2) DEFAULT 0.00 NOT NULL CHECK (descuento >= 0),
    impuesto NUMERIC(12, 2) NOT NULL CHECK (impuesto >= 0),
    total NUMERIC(12, 2) NOT NULL CHECK (total >= 0),
    producto UUID NOT NULL,
    venta UUID NOT NULL,
	FOREIGN KEY (venta) REFERENCES ventas(id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE proveedores (
    id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    nombre VARCHAR(100) NOT NULL,
    direccion UUID,
    correo VARCHAR(255) UNIQUE,
    telefono VARCHAR(15) UNIQUE,
	FOREIGN KEY (direccion) REFERENCES direcciones(id) ON UPDATE CASCADE ON DELETE SET NULL
)

CREATE TABLE contacto_proveedor (
	id SERIAL PRIMARY KEY,
	nombre VARCHAR(100),
	correo VARCHAR(255), 
	telefono VARCHAR(15),
	proveedor UUID NOT NULL,
	FOREIGN KEY (proveedor) REFERENCES proveedores(id) ON DELETE CASCADE ON UPDATE CASCADE
)

CREATE TABLE productos_proveedores (
	id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	producto UUID NOT NULL,
	proveedor UUID NOT NULL,
	precio NUMERIC(12, 2) NOT NULL CHECK (precio >= 0),
	impuesto NUMERIC(12, 2) NOT NULL CHECK (impuesto >= 0),
	total NUMERIC(12, 2) NOT NULL CHECK (total >= 0),
	stock INT,
	fecha_actualizado DATE NOT NULL DEFAULT CURRENT_DATE,
	FOREIGN KEY (producto) REFERENCES productos(id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (proveedor) REFERENCES proveedores(id) ON DELETE CASCADE ON UPDATE CASCADE
)

CREATE TABLE ordenes_compra_proveedor (
	id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	proveedor UUID NOT NULL,
	id_orden TEXT NOT NULL,
	fecha_estimada_entrega DATE CHECK (fecha_estimada_entrega >= CURRENT_DATE),
	estado VARCHAR(50) NOT NULL,
	FOREIGN KEY (proveedor) REFERENCES proveedores(id) ON DELETE SET NULL ON UPDATE CASCADE
)
/*
CREATE TABLE documentos (
	id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	tipo_operacion ENUM NOT NULL,
	id_operacion UUID NOT NULL,
	url TEXT NOT NULL,
	tipo_documento VARCHAR(100)
)
*/
CREATE TABLE historial_productos (
	id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    producto UUID NOT NULL,
	proveedor UUID NOT NULL,
    precio_base_anterior NUMERIC(12, 2) NOT NULL CHECK (precio_base_anterior >= 0),
	precio_total_anterior NUMERIC(12, 2) NOT NULL CHECK (precio_total_anterior >= 0),
    fecha_cambio TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (producto) REFERENCES productos(id) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (proveedor) REFERENCES proveedores(id) ON DELETE CASCADE ON UPDATE CASCADE
)

-- Triggers
CREATE OR REPLACE FUNCTION guardar_historial_precio()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO historial_productos (producto, precio_base_anterior, precio_total_anterior, proveedor)
    VALUES (OLD.producto, OLD.precio, OLD.total, OLD.proveedor);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER tr_guardar_historial_precio
BEFORE UPDATE OF total ON productos_proveedores
FOR EACH ROW
WHEN (OLD.total IS DISTINCT FROM NEW.total)
EXECUTE FUNCTION guardar_historial_precio();

CREATE OR REPLACE FUNCTION actualizar_timestamp() 
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER usuarios_actualizar_updated_at
BEFORE UPDATE ON usuarios
FOR EACH ROW
EXECUTE FUNCTION actualizar_timestamp();

CREATE TRIGGER clientes_actualizar_updated_at
BEFORE UPDATE ON clientes
FOR EACH ROW
EXECUTE FUNCTION actualizar_timestamp();

CREATE TRIGGER trabajadores_actualizar_updated_at
BEFORE UPDATE ON trabajadores
FOR EACH ROW
EXECUTE FUNCTION actualizar_timestamp();
