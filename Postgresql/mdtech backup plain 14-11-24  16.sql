--
-- PostgreSQL database dump
--

-- Dumped from database version 16.4
-- Dumped by pg_dump version 16.4

-- Started on 2024-11-14 16:32:36

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
-- TOC entry 5023 (class 1262 OID 26693)
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
-- TOC entry 234 (class 1255 OID 26926)
-- Name: guardar_historial_precio(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.guardar_historial_precio() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO historial_productos (producto, precio_base_anterior, precio_total_anterior)
    VALUES (OLD.producto, OLD.precio, OLD.total);
    RETURN NEW;
END;
$$;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 224 (class 1259 OID 26800)
-- Name: categorias; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.categorias (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(50) NOT NULL,
    descripcion text,
    categoria_padre uuid
);


--
-- TOC entry 216 (class 1259 OID 26705)
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
-- TOC entry 230 (class 1259 OID 26867)
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
-- TOC entry 229 (class 1259 OID 26866)
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
-- TOC entry 5024 (class 0 OID 0)
-- Dependencies: 229
-- Name: contacto_proveedor_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.contacto_proveedor_id_seq OWNED BY public.contacto_proveedor.id;


--
-- TOC entry 227 (class 1259 OID 26839)
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
-- TOC entry 222 (class 1259 OID 26757)
-- Name: direccion_cliente; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.direccion_cliente (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    cliente uuid NOT NULL,
    direccion uuid NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- TOC entry 221 (class 1259 OID 26742)
-- Name: direcciones; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.direcciones (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    descripcion text,
    provincia integer NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- TOC entry 220 (class 1259 OID 26741)
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
-- TOC entry 5025 (class 0 OID 0)
-- Dependencies: 220
-- Name: direcciones_provincia_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.direcciones_provincia_seq OWNED BY public.direcciones.provincia;


--
-- TOC entry 233 (class 1259 OID 26912)
-- Name: historial_productos; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.historial_productos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    producto uuid,
    precio_base_anterior numeric(12,2) NOT NULL,
    precio_total_anterior numeric(12,2) NOT NULL,
    fecha_cambio timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT historial_productos_precio_base_anterior_check CHECK ((precio_base_anterior >= (0)::numeric)),
    CONSTRAINT historial_productos_precio_total_anterior_check CHECK ((precio_total_anterior >= (0)::numeric))
);


--
-- TOC entry 226 (class 1259 OID 26826)
-- Name: imagenes_productos; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.imagenes_productos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    descripcion text,
    url text NOT NULL,
    producto uuid NOT NULL
);


--
-- TOC entry 232 (class 1259 OID 26898)
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
-- TOC entry 225 (class 1259 OID 26813)
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
-- TOC entry 231 (class 1259 OID 26878)
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
-- TOC entry 228 (class 1259 OID 26851)
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
-- TOC entry 219 (class 1259 OID 26735)
-- Name: provincias; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.provincias (
    id integer NOT NULL,
    nombre character varying(100)
);


--
-- TOC entry 218 (class 1259 OID 26734)
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
-- TOC entry 5026 (class 0 OID 0)
-- Dependencies: 218
-- Name: provincias_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.provincias_id_seq OWNED BY public.provincias.id;


--
-- TOC entry 217 (class 1259 OID 26723)
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
-- TOC entry 215 (class 1259 OID 26694)
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
-- TOC entry 223 (class 1259 OID 26787)
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
-- TOC entry 4774 (class 2604 OID 26870)
-- Name: contacto_proveedor id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacto_proveedor ALTER COLUMN id SET DEFAULT nextval('public.contacto_proveedor_id_seq'::regclass);


--
-- TOC entry 4761 (class 2604 OID 26746)
-- Name: direcciones provincia; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direcciones ALTER COLUMN provincia SET DEFAULT nextval('public.direcciones_provincia_seq'::regclass);


--
-- TOC entry 4759 (class 2604 OID 26738)
-- Name: provincias id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.provincias ALTER COLUMN id SET DEFAULT nextval('public.provincias_id_seq'::regclass);


--
-- TOC entry 5008 (class 0 OID 26800)
-- Dependencies: 224
-- Data for Name: categorias; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5000 (class 0 OID 26705)
-- Dependencies: 216
-- Data for Name: clientes; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5014 (class 0 OID 26867)
-- Dependencies: 230
-- Data for Name: contacto_proveedor; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5011 (class 0 OID 26839)
-- Dependencies: 227
-- Data for Name: detalles_venta; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5006 (class 0 OID 26757)
-- Dependencies: 222
-- Data for Name: direccion_cliente; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5005 (class 0 OID 26742)
-- Dependencies: 221
-- Data for Name: direcciones; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5017 (class 0 OID 26912)
-- Dependencies: 233
-- Data for Name: historial_productos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5010 (class 0 OID 26826)
-- Dependencies: 226
-- Data for Name: imagenes_productos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5016 (class 0 OID 26898)
-- Dependencies: 232
-- Data for Name: ordenes_compra_proveedor; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5009 (class 0 OID 26813)
-- Dependencies: 225
-- Data for Name: productos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5015 (class 0 OID 26878)
-- Dependencies: 231
-- Data for Name: productos_proveedores; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5012 (class 0 OID 26851)
-- Dependencies: 228
-- Data for Name: proveedores; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5003 (class 0 OID 26735)
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
-- TOC entry 5001 (class 0 OID 26723)
-- Dependencies: 217
-- Data for Name: trabajadores; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 4999 (class 0 OID 26694)
-- Dependencies: 215
-- Data for Name: usuarios; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5007 (class 0 OID 26787)
-- Dependencies: 223
-- Data for Name: ventas; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5027 (class 0 OID 0)
-- Dependencies: 229
-- Name: contacto_proveedor_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.contacto_proveedor_id_seq', 1, false);


--
-- TOC entry 5028 (class 0 OID 0)
-- Dependencies: 220
-- Name: direcciones_provincia_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.direcciones_provincia_seq', 1, false);


--
-- TOC entry 5029 (class 0 OID 0)
-- Dependencies: 218
-- Name: provincias_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.provincias_id_seq', 17, true);


--
-- TOC entry 4821 (class 2606 OID 26807)
-- Name: categorias categorias_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT categorias_pkey PRIMARY KEY (id);


--
-- TOC entry 4803 (class 2606 OID 26713)
-- Name: clientes clientes_correo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_correo_key UNIQUE (correo);


--
-- TOC entry 4805 (class 2606 OID 26711)
-- Name: clientes clientes_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_pkey PRIMARY KEY (id);


--
-- TOC entry 4807 (class 2606 OID 26715)
-- Name: clientes clientes_telefono_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_telefono_key UNIQUE (telefono);


--
-- TOC entry 4809 (class 2606 OID 26717)
-- Name: clientes clientes_usuario_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_usuario_key UNIQUE (usuario);


--
-- TOC entry 4835 (class 2606 OID 26872)
-- Name: contacto_proveedor contacto_proveedor_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacto_proveedor
    ADD CONSTRAINT contacto_proveedor_pkey PRIMARY KEY (id);


--
-- TOC entry 4827 (class 2606 OID 26850)
-- Name: detalles_venta detalles_venta_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.detalles_venta
    ADD CONSTRAINT detalles_venta_pkey PRIMARY KEY (id);


--
-- TOC entry 4817 (class 2606 OID 26763)
-- Name: direccion_cliente direccion_cliente_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_pkey PRIMARY KEY (id);


--
-- TOC entry 4815 (class 2606 OID 26751)
-- Name: direcciones direcciones_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direcciones
    ADD CONSTRAINT direcciones_pkey PRIMARY KEY (id);


--
-- TOC entry 4841 (class 2606 OID 26920)
-- Name: historial_productos historial_productos_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_pkey PRIMARY KEY (id);


--
-- TOC entry 4825 (class 2606 OID 26833)
-- Name: imagenes_productos imagenes_productos_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.imagenes_productos
    ADD CONSTRAINT imagenes_productos_pkey PRIMARY KEY (id);


--
-- TOC entry 4839 (class 2606 OID 26906)
-- Name: ordenes_compra_proveedor ordenes_compra_proveedor_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ordenes_compra_proveedor
    ADD CONSTRAINT ordenes_compra_proveedor_pkey PRIMARY KEY (id);


--
-- TOC entry 4823 (class 2606 OID 26820)
-- Name: productos productos_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.productos
    ADD CONSTRAINT productos_pkey PRIMARY KEY (id);


--
-- TOC entry 4837 (class 2606 OID 26887)
-- Name: productos_proveedores productos_proveedores_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_pkey PRIMARY KEY (id);


--
-- TOC entry 4829 (class 2606 OID 26858)
-- Name: proveedores proveedores_correo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_correo_key UNIQUE (correo);


--
-- TOC entry 4831 (class 2606 OID 26856)
-- Name: proveedores proveedores_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_pkey PRIMARY KEY (id);


--
-- TOC entry 4833 (class 2606 OID 26860)
-- Name: proveedores proveedores_telefono_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_telefono_key UNIQUE (telefono);


--
-- TOC entry 4813 (class 2606 OID 26740)
-- Name: provincias provincias_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.provincias
    ADD CONSTRAINT provincias_pkey PRIMARY KEY (id);


--
-- TOC entry 4811 (class 2606 OID 26733)
-- Name: trabajadores trabajadores_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.trabajadores
    ADD CONSTRAINT trabajadores_pkey PRIMARY KEY (id);


--
-- TOC entry 4799 (class 2606 OID 26702)
-- Name: usuarios usuarios_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_pkey PRIMARY KEY (id);


--
-- TOC entry 4801 (class 2606 OID 26704)
-- Name: usuarios usuarios_username_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_username_key UNIQUE (username);


--
-- TOC entry 4819 (class 2606 OID 26799)
-- Name: ventas ventas_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ventas
    ADD CONSTRAINT ventas_pkey PRIMARY KEY (id);


--
-- TOC entry 4855 (class 2620 OID 26927)
-- Name: productos_proveedores tr_guardar_historial_precio; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER tr_guardar_historial_precio BEFORE UPDATE OF total ON public.productos_proveedores FOR EACH ROW WHEN ((old.total IS DISTINCT FROM new.total)) EXECUTE FUNCTION public.guardar_historial_precio();


--
-- TOC entry 4846 (class 2606 OID 26808)
-- Name: categorias categorias_categoria_padre_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT categorias_categoria_padre_fkey FOREIGN KEY (categoria_padre) REFERENCES public.categorias(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 4842 (class 2606 OID 26718)
-- Name: clientes clientes_usuario_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_usuario_fkey FOREIGN KEY (usuario) REFERENCES public.usuarios(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 4850 (class 2606 OID 26873)
-- Name: contacto_proveedor contacto_proveedor_proveedor_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.contacto_proveedor
    ADD CONSTRAINT contacto_proveedor_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4844 (class 2606 OID 26764)
-- Name: direccion_cliente direccion_cliente_cliente_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_cliente_fkey FOREIGN KEY (cliente) REFERENCES public.clientes(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4845 (class 2606 OID 26769)
-- Name: direccion_cliente direccion_cliente_direccion_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_direccion_fkey FOREIGN KEY (direccion) REFERENCES public.direcciones(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4843 (class 2606 OID 26752)
-- Name: direcciones direcciones_provincia_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.direcciones
    ADD CONSTRAINT direcciones_provincia_fkey FOREIGN KEY (provincia) REFERENCES public.provincias(id) ON UPDATE CASCADE;


--
-- TOC entry 4854 (class 2606 OID 26921)
-- Name: historial_productos historial_productos_producto_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON DELETE CASCADE;


--
-- TOC entry 4848 (class 2606 OID 26834)
-- Name: imagenes_productos imagenes_productos_producto_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.imagenes_productos
    ADD CONSTRAINT imagenes_productos_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE;


--
-- TOC entry 4853 (class 2606 OID 26907)
-- Name: ordenes_compra_proveedor ordenes_compra_proveedor_proveedor_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ordenes_compra_proveedor
    ADD CONSTRAINT ordenes_compra_proveedor_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 4847 (class 2606 OID 26821)
-- Name: productos productos_categoria_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.productos
    ADD CONSTRAINT productos_categoria_fkey FOREIGN KEY (categoria) REFERENCES public.categorias(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- TOC entry 4851 (class 2606 OID 26888)
-- Name: productos_proveedores productos_proveedores_producto_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4852 (class 2606 OID 26893)
-- Name: productos_proveedores productos_proveedores_proveedor_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4849 (class 2606 OID 26861)
-- Name: proveedores proveedores_direccion_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_direccion_fkey FOREIGN KEY (direccion) REFERENCES public.direcciones(id) ON UPDATE CASCADE;


-- Completed on 2024-11-14 16:32:36

--
-- PostgreSQL database dump complete
--

