--
-- PostgreSQL database dump
--

-- Dumped from database version 16.4
-- Dumped by pg_dump version 16.4

-- Started on 2024-11-15 20:02:57

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 5028 (class 1262 OID 27151)
-- Name: mdtecnologia; Type: DATABASE; Schema: -; Owner: -
--

CREATE DATABASE mdtecnologia WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Spanish_Panama.1252';


\connect mdtecnologia

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 235 (class 1255 OID 27373)
-- Name: actualizar_timestamp(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.actualizar_timestamp() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$;


--
-- TOC entry 234 (class 1255 OID 27371)
-- Name: guardar_historial_precio(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.guardar_historial_precio() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO historial_productos (producto, precio_base_anterior, precio_total_anterior, proveedor)
    VALUES (OLD.producto, OLD.precio, OLD.total, OLD.proveedor);
    RETURN NEW;
END;
$$;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 224 (class 1259 OID 27245)
-- Name: categorias; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.categorias (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(50) NOT NULL,
    descripcion text,
    categoria_padre uuid
);


--
-- TOC entry 216 (class 1259 OID 27163)
-- Name: clientes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.clientes (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(100) NOT NULL,
    apellido character varying(100) NOT NULL,
    correo character varying(255) NOT NULL,
    telefono character varying(15),
    usuario uuid,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamp without time zone
);


--
-- TOC entry 230 (class 1259 OID 27312)
-- Name: contacto_proveedor; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.contacto_proveedor (
    id integer NOT NULL,
    nombre character varying(100),
    correo character varying(255),
    telefono character varying(15),
    proveedor uuid NOT NULL
);


--
-- TOC entry 229 (class 1259 OID 27311)
-- Name: contacto_proveedor_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.contacto_proveedor_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 5029 (class 0 OID 0)
-- Dependencies: 229
-- Name: contacto_proveedor_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.contacto_proveedor_id_seq OWNED BY public.contacto_proveedor.id;


--
-- TOC entry 227 (class 1259 OID 27284)
-- Name: detalles_venta; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.detalles_venta (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    cantidad integer NOT NULL,
    precio_unitario numeric(12,2) NOT NULL,
    subtotal numeric(12,2) NOT NULL,
    descuento numeric(12,2) DEFAULT 0.00 NOT NULL,
    impuesto numeric(12,2) NOT NULL,
    total numeric(12,2) NOT NULL,
    producto uuid NOT NULL,
    venta uuid NOT NULL,
    CONSTRAINT detalles_venta_descuento_check CHECK ((descuento >= (0)::numeric)),
    CONSTRAINT detalles_venta_impuesto_check CHECK ((impuesto >= (0)::numeric)),
    CONSTRAINT detalles_venta_precio_unitario_check CHECK ((precio_unitario >= (0)::numeric)),
    CONSTRAINT detalles_venta_subtotal_check CHECK ((subtotal >= (0)::numeric)),
    CONSTRAINT detalles_venta_total_check CHECK ((total >= (0)::numeric))
);


--
-- TOC entry 222 (class 1259 OID 27215)
-- Name: direccion_cliente; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.direccion_cliente (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    cliente uuid NOT NULL,
    direccion uuid NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- TOC entry 221 (class 1259 OID 27200)
-- Name: direcciones; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.direcciones (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    descripcion text,
    provincia integer NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- TOC entry 220 (class 1259 OID 27199)
-- Name: direcciones_provincia_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.direcciones_provincia_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 5030 (class 0 OID 0)
-- Dependencies: 220
-- Name: direcciones_provincia_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.direcciones_provincia_seq OWNED BY public.direcciones.provincia;


--
-- TOC entry 233 (class 1259 OID 27391)
-- Name: historial_productos; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.historial_productos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    producto uuid NOT NULL,
    proveedor uuid NOT NULL,
    precio_base_anterior numeric(12,2) NOT NULL,
    precio_total_anterior numeric(12,2) NOT NULL,
    fecha_cambio timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT historial_productos_precio_base_anterior_check CHECK ((precio_base_anterior >= (0)::numeric)),
    CONSTRAINT historial_productos_precio_total_anterior_check CHECK ((precio_total_anterior >= (0)::numeric))
);


--
-- TOC entry 226 (class 1259 OID 27271)
-- Name: imagenes_productos; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.imagenes_productos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    descripcion text,
    url text NOT NULL,
    producto uuid NOT NULL
);


--
-- TOC entry 232 (class 1259 OID 27343)
-- Name: ordenes_compra_proveedor; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.ordenes_compra_proveedor (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    proveedor uuid NOT NULL,
    id_orden text NOT NULL,
    fecha_estimada_entrega date,
    estado character varying(50) NOT NULL,
    CONSTRAINT ordenes_compra_proveedor_fecha_estimada_entrega_check CHECK ((fecha_estimada_entrega >= CURRENT_DATE))
);


--
-- TOC entry 225 (class 1259 OID 27258)
-- Name: productos; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.productos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre text NOT NULL,
    marca text NOT NULL,
    descripcion text,
    categoria uuid
);


--
-- TOC entry 231 (class 1259 OID 27323)
-- Name: productos_proveedores; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.productos_proveedores (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    producto uuid NOT NULL,
    proveedor uuid NOT NULL,
    precio numeric(12,2) NOT NULL,
    impuesto numeric(12,2) NOT NULL,
    total numeric(12,2) NOT NULL,
    stock integer,
    fecha_actualizado date DEFAULT CURRENT_DATE NOT NULL,
    CONSTRAINT productos_proveedores_impuesto_check CHECK ((impuesto >= (0)::numeric)),
    CONSTRAINT productos_proveedores_precio_check CHECK ((precio >= (0)::numeric)),
    CONSTRAINT productos_proveedores_total_check CHECK ((total >= (0)::numeric))
);


--
-- TOC entry 228 (class 1259 OID 27296)
-- Name: proveedores; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.proveedores (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(100) NOT NULL,
    direccion uuid,
    correo character varying(255),
    telefono character varying(15)
);


--
-- TOC entry 219 (class 1259 OID 27193)
-- Name: provincias; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.provincias (
    id integer NOT NULL,
    nombre character varying(100) NOT NULL
);


--
-- TOC entry 218 (class 1259 OID 27192)
-- Name: provincias_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.provincias_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 5031 (class 0 OID 0)
-- Dependencies: 218
-- Name: provincias_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.provincias_id_seq OWNED BY public.provincias.id;


--
-- TOC entry 217 (class 1259 OID 27181)
-- Name: trabajadores; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.trabajadores (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(100) NOT NULL,
    apellido character varying(100) NOT NULL,
    correo character varying(255) NOT NULL,
    telefono character varying(15),
    usuario uuid,
    cargo character varying(100) NOT NULL,
    fecha_ingreso date NOT NULL,
    estado boolean DEFAULT true NOT NULL,
    salario numeric(12,2),
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamp without time zone,
    CONSTRAINT trabajadores_salario_check CHECK ((salario >= (0)::numeric))
);


--
-- TOC entry 215 (class 1259 OID 27152)
-- Name: usuarios; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.usuarios (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    username character varying(50) NOT NULL,
    password character varying(100),
    disabled boolean DEFAULT false NOT NULL,
    rol character varying(50) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamp without time zone,
    CONSTRAINT usuarios_password_check CHECK ((((password)::text ~~ '$2_$_%'::text) AND (length((password)::text) >= 60)))
);


--
-- TOC entry 223 (class 1259 OID 27232)
-- Name: ventas; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.ventas (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    fecha timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    estado character varying(50) NOT NULL,
    cantidad_total_productos integer NOT NULL,
    subtotal numeric(12,2) NOT NULL,
    descuento numeric(12,2) DEFAULT 0.00 NOT NULL,
    impuesto numeric(12,2) NOT NULL,
    total numeric(12,2) NOT NULL,
    direccion_entrega uuid NOT NULL,
    cliente uuid NOT NULL,
    CONSTRAINT ventas_cantidad_total_productos_check CHECK ((cantidad_total_productos >= 0)),
    CONSTRAINT ventas_descuento_check CHECK ((descuento >= (0)::numeric)),
    CONSTRAINT ventas_impuesto_check CHECK ((impuesto >= (0)::numeric)),
    CONSTRAINT ventas_subtotal_check CHECK ((subtotal >= (0)::numeric)),
    CONSTRAINT ventas_total_check CHECK ((total >= (0)::numeric))
);


--
-- TOC entry 4775 (class 2604 OID 27315)
-- Name: contacto_proveedor id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacto_proveedor ALTER COLUMN id SET DEFAULT nextval('public.contacto_proveedor_id_seq'::regclass);


--
-- TOC entry 4762 (class 2604 OID 27204)
-- Name: direcciones provincia; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direcciones ALTER COLUMN provincia SET DEFAULT nextval('public.direcciones_provincia_seq'::regclass);


--
-- TOC entry 4760 (class 2604 OID 27196)
-- Name: provincias id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.provincias ALTER COLUMN id SET DEFAULT nextval('public.provincias_id_seq'::regclass);


--
-- TOC entry 5013 (class 0 OID 27245)
-- Dependencies: 224
-- Data for Name: categorias; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5005 (class 0 OID 27163)
-- Dependencies: 216
-- Data for Name: clientes; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5019 (class 0 OID 27312)
-- Dependencies: 230
-- Data for Name: contacto_proveedor; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5016 (class 0 OID 27284)
-- Dependencies: 227
-- Data for Name: detalles_venta; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5011 (class 0 OID 27215)
-- Dependencies: 222
-- Data for Name: direccion_cliente; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5010 (class 0 OID 27200)
-- Dependencies: 221
-- Data for Name: direcciones; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5022 (class 0 OID 27391)
-- Dependencies: 233
-- Data for Name: historial_productos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5015 (class 0 OID 27271)
-- Dependencies: 226
-- Data for Name: imagenes_productos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5021 (class 0 OID 27343)
-- Dependencies: 232
-- Data for Name: ordenes_compra_proveedor; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5014 (class 0 OID 27258)
-- Dependencies: 225
-- Data for Name: productos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5020 (class 0 OID 27323)
-- Dependencies: 231
-- Data for Name: productos_proveedores; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5017 (class 0 OID 27296)
-- Dependencies: 228
-- Data for Name: proveedores; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5008 (class 0 OID 27193)
-- Dependencies: 219
-- Data for Name: provincias; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.provincias VALUES (1, 'Bocas del Toro');
INSERT INTO public.provincias VALUES (2, 'Coclé');
INSERT INTO public.provincias VALUES (3, 'Colón');
INSERT INTO public.provincias VALUES (4, 'Chiriquí');
INSERT INTO public.provincias VALUES (5, 'Darién');
INSERT INTO public.provincias VALUES (6, 'Herrera');
INSERT INTO public.provincias VALUES (7, 'Los Santos');
INSERT INTO public.provincias VALUES (8, 'Panamá');
INSERT INTO public.provincias VALUES (9, 'Veraguas');
INSERT INTO public.provincias VALUES (10, 'Guna Yala');
INSERT INTO public.provincias VALUES (11, 'Emberá-Wounaan');
INSERT INTO public.provincias VALUES (12, 'Ngäbe-Buglé');
INSERT INTO public.provincias VALUES (13, 'Panamá Oeste');
INSERT INTO public.provincias VALUES (14, 'Naso Tjër Di');
INSERT INTO public.provincias VALUES (15, 'Guna de Madugandí');
INSERT INTO public.provincias VALUES (16, 'Guna de Wargandí');
INSERT INTO public.provincias VALUES (17, 'Extranjero');


--
-- TOC entry 5006 (class 0 OID 27181)
-- Dependencies: 217
-- Data for Name: trabajadores; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5004 (class 0 OID 27152)
-- Dependencies: 215
-- Data for Name: usuarios; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5012 (class 0 OID 27232)
-- Dependencies: 223
-- Data for Name: ventas; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5032 (class 0 OID 0)
-- Dependencies: 229
-- Name: contacto_proveedor_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.contacto_proveedor_id_seq', 1, false);


--
-- TOC entry 5033 (class 0 OID 0)
-- Dependencies: 220
-- Name: direcciones_provincia_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.direcciones_provincia_seq', 1, false);


--
-- TOC entry 5034 (class 0 OID 0)
-- Dependencies: 218
-- Name: provincias_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.provincias_id_seq', 17, true);


--
-- TOC entry 4822 (class 2606 OID 27252)
-- Name: categorias categorias_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT categorias_pkey PRIMARY KEY (id);


--
-- TOC entry 4804 (class 2606 OID 27171)
-- Name: clientes clientes_correo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_correo_key UNIQUE (correo);


--
-- TOC entry 4806 (class 2606 OID 27169)
-- Name: clientes clientes_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_pkey PRIMARY KEY (id);


--
-- TOC entry 4808 (class 2606 OID 27173)
-- Name: clientes clientes_telefono_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_telefono_key UNIQUE (telefono);


--
-- TOC entry 4810 (class 2606 OID 27175)
-- Name: clientes clientes_usuario_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_usuario_key UNIQUE (usuario);


--
-- TOC entry 4836 (class 2606 OID 27317)
-- Name: contacto_proveedor contacto_proveedor_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacto_proveedor
    ADD CONSTRAINT contacto_proveedor_pkey PRIMARY KEY (id);


--
-- TOC entry 4828 (class 2606 OID 27295)
-- Name: detalles_venta detalles_venta_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.detalles_venta
    ADD CONSTRAINT detalles_venta_pkey PRIMARY KEY (id);


--
-- TOC entry 4818 (class 2606 OID 27221)
-- Name: direccion_cliente direccion_cliente_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_pkey PRIMARY KEY (id);


--
-- TOC entry 4816 (class 2606 OID 27209)
-- Name: direcciones direcciones_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direcciones
    ADD CONSTRAINT direcciones_pkey PRIMARY KEY (id);


--
-- TOC entry 4842 (class 2606 OID 27399)
-- Name: historial_productos historial_productos_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_pkey PRIMARY KEY (id);


--
-- TOC entry 4826 (class 2606 OID 27278)
-- Name: imagenes_productos imagenes_productos_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.imagenes_productos
    ADD CONSTRAINT imagenes_productos_pkey PRIMARY KEY (id);


--
-- TOC entry 4840 (class 2606 OID 27351)
-- Name: ordenes_compra_proveedor ordenes_compra_proveedor_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ordenes_compra_proveedor
    ADD CONSTRAINT ordenes_compra_proveedor_pkey PRIMARY KEY (id);


--
-- TOC entry 4824 (class 2606 OID 27265)
-- Name: productos productos_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.productos
    ADD CONSTRAINT productos_pkey PRIMARY KEY (id);


--
-- TOC entry 4838 (class 2606 OID 27332)
-- Name: productos_proveedores productos_proveedores_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_pkey PRIMARY KEY (id);


--
-- TOC entry 4830 (class 2606 OID 27303)
-- Name: proveedores proveedores_correo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_correo_key UNIQUE (correo);


--
-- TOC entry 4832 (class 2606 OID 27301)
-- Name: proveedores proveedores_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_pkey PRIMARY KEY (id);


--
-- TOC entry 4834 (class 2606 OID 27305)
-- Name: proveedores proveedores_telefono_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_telefono_key UNIQUE (telefono);


--
-- TOC entry 4814 (class 2606 OID 27198)
-- Name: provincias provincias_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.provincias
    ADD CONSTRAINT provincias_pkey PRIMARY KEY (id);


--
-- TOC entry 4812 (class 2606 OID 27191)
-- Name: trabajadores trabajadores_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.trabajadores
    ADD CONSTRAINT trabajadores_pkey PRIMARY KEY (id);


--
-- TOC entry 4800 (class 2606 OID 27160)
-- Name: usuarios usuarios_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_pkey PRIMARY KEY (id);


--
-- TOC entry 4802 (class 2606 OID 27162)
-- Name: usuarios usuarios_username_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_username_key UNIQUE (username);


--
-- TOC entry 4820 (class 2606 OID 27244)
-- Name: ventas ventas_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ventas
    ADD CONSTRAINT ventas_pkey PRIMARY KEY (id);


--
-- TOC entry 4858 (class 2620 OID 27375)
-- Name: clientes clientes_actualizar_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER clientes_actualizar_updated_at BEFORE UPDATE ON public.clientes FOR EACH ROW EXECUTE FUNCTION public.actualizar_timestamp();


--
-- TOC entry 4860 (class 2620 OID 27372)
-- Name: productos_proveedores tr_guardar_historial_precio; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER tr_guardar_historial_precio BEFORE UPDATE OF total ON public.productos_proveedores FOR EACH ROW WHEN ((old.total IS DISTINCT FROM new.total)) EXECUTE FUNCTION public.guardar_historial_precio();


--
-- TOC entry 4859 (class 2620 OID 27376)
-- Name: trabajadores trabajadores_actualizar_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trabajadores_actualizar_updated_at BEFORE UPDATE ON public.trabajadores FOR EACH ROW EXECUTE FUNCTION public.actualizar_timestamp();


--
-- TOC entry 4857 (class 2620 OID 27374)
-- Name: usuarios usuarios_actualizar_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER usuarios_actualizar_updated_at BEFORE UPDATE ON public.usuarios FOR EACH ROW EXECUTE FUNCTION public.actualizar_timestamp();


--
-- TOC entry 4847 (class 2606 OID 27253)
-- Name: categorias categorias_categoria_padre_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT categorias_categoria_padre_fkey FOREIGN KEY (categoria_padre) REFERENCES public.categorias(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 4843 (class 2606 OID 27176)
-- Name: clientes clientes_usuario_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_usuario_fkey FOREIGN KEY (usuario) REFERENCES public.usuarios(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 4851 (class 2606 OID 27318)
-- Name: contacto_proveedor contacto_proveedor_proveedor_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacto_proveedor
    ADD CONSTRAINT contacto_proveedor_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4845 (class 2606 OID 27222)
-- Name: direccion_cliente direccion_cliente_cliente_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_cliente_fkey FOREIGN KEY (cliente) REFERENCES public.clientes(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4846 (class 2606 OID 27227)
-- Name: direccion_cliente direccion_cliente_direccion_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_direccion_fkey FOREIGN KEY (direccion) REFERENCES public.direcciones(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4844 (class 2606 OID 27210)
-- Name: direcciones direcciones_provincia_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direcciones
    ADD CONSTRAINT direcciones_provincia_fkey FOREIGN KEY (provincia) REFERENCES public.provincias(id) ON UPDATE CASCADE;


--
-- TOC entry 4855 (class 2606 OID 27400)
-- Name: historial_productos historial_productos_producto_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4856 (class 2606 OID 27405)
-- Name: historial_productos historial_productos_proveedor_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4849 (class 2606 OID 27279)
-- Name: imagenes_productos imagenes_productos_producto_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.imagenes_productos
    ADD CONSTRAINT imagenes_productos_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 4854 (class 2606 OID 27352)
-- Name: ordenes_compra_proveedor ordenes_compra_proveedor_proveedor_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ordenes_compra_proveedor
    ADD CONSTRAINT ordenes_compra_proveedor_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 4848 (class 2606 OID 27266)
-- Name: productos productos_categoria_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.productos
    ADD CONSTRAINT productos_categoria_fkey FOREIGN KEY (categoria) REFERENCES public.categorias(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 4852 (class 2606 OID 27333)
-- Name: productos_proveedores productos_proveedores_producto_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4853 (class 2606 OID 27338)
-- Name: productos_proveedores productos_proveedores_proveedor_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4850 (class 2606 OID 27306)
-- Name: proveedores proveedores_direccion_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_direccion_fkey FOREIGN KEY (direccion) REFERENCES public.direcciones(id) ON UPDATE CASCADE ON DELETE SET NULL;


-- Completed on 2024-11-15 20:02:57

--
-- PostgreSQL database dump complete
--

