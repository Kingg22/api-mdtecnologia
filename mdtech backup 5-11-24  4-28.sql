PGDMP                  
    |            mdtecnologia    16.4    16.4 U    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    17145    mdtecnologia    DATABASE     �   CREATE DATABASE mdtecnologia WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Spanish_Panama.1252';
    DROP DATABASE mdtecnologia;
                postgres    false            �            1255    17489    guardar_historial_precio()    FUNCTION       CREATE FUNCTION public.guardar_historial_precio() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO historial_productos (producto, precio_base_anterior, precio_total_anterior)
    VALUES (OLD.producto, OLD.precio, OLD.total);
    RETURN NEW;
END;
$$;
 1   DROP FUNCTION public.guardar_historial_precio();
       public          postgres    false            �            1259    17344 
   categorias    TABLE     �   CREATE TABLE public.categorias (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(50) NOT NULL,
    descripcion text
);
    DROP TABLE public.categorias;
       public         heap    postgres    false            �            1259    17258    clientes    TABLE     �  CREATE TABLE public.clientes (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(100) NOT NULL,
    apellido character varying(100) NOT NULL,
    correo character varying(255) NOT NULL,
    telefono character varying(15),
    usuario uuid,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamp without time zone
);
    DROP TABLE public.clientes;
       public         heap    postgres    false            �            1259    17403    contacto_proveedor    TABLE     �   CREATE TABLE public.contacto_proveedor (
    id integer NOT NULL,
    nombre character varying(100),
    correo character varying(255),
    telefono character varying(15),
    proveedor uuid NOT NULL
);
 &   DROP TABLE public.contacto_proveedor;
       public         heap    postgres    false            �            1259    17402    contacto_proveedor_id_seq    SEQUENCE     �   CREATE SEQUENCE public.contacto_proveedor_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 0   DROP SEQUENCE public.contacto_proveedor_id_seq;
       public          postgres    false    229            �           0    0    contacto_proveedor_id_seq    SEQUENCE OWNED BY     W   ALTER SEQUENCE public.contacto_proveedor_id_seq OWNED BY public.contacto_proveedor.id;
          public          postgres    false    228            �            1259    17365    detalles_venta    TABLE     /  CREATE TABLE public.detalles_venta (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    cantidad integer NOT NULL,
    precio_unitario money NOT NULL,
    subtotal money NOT NULL,
    descuento money DEFAULT 'B/.0.00'::money NOT NULL,
    impuesto money NOT NULL,
    total money NOT NULL,
    producto uuid NOT NULL,
    venta uuid NOT NULL,
    CONSTRAINT detalles_venta_descuento_check CHECK (((descuento)::numeric < (0)::numeric)),
    CONSTRAINT detalles_venta_impuesto_check CHECK (((impuesto)::numeric >= (0)::numeric)),
    CONSTRAINT detalles_venta_precio_unitario_check CHECK (((precio_unitario)::numeric >= (0)::numeric)),
    CONSTRAINT detalles_venta_subtotal_check CHECK (((subtotal)::numeric >= (0)::numeric)),
    CONSTRAINT detalles_venta_total_check CHECK (((total)::numeric >= (0)::numeric))
);
 "   DROP TABLE public.detalles_venta;
       public         heap    postgres    false            �            1259    17296    direccion_cliente    TABLE     �   CREATE TABLE public.direccion_cliente (
    cliente uuid NOT NULL,
    direccion uuid NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);
 %   DROP TABLE public.direccion_cliente;
       public         heap    postgres    false            �            1259    17204    direcciones    TABLE     �   CREATE TABLE public.direcciones (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    descripcion text,
    provincia integer NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);
    DROP TABLE public.direcciones;
       public         heap    postgres    false            �            1259    17203    direcciones_provincia_seq    SEQUENCE     �   CREATE SEQUENCE public.direcciones_provincia_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 0   DROP SEQUENCE public.direcciones_provincia_seq;
       public          postgres    false    218            �           0    0    direcciones_provincia_seq    SEQUENCE OWNED BY     W   ALTER SEQUENCE public.direcciones_provincia_seq OWNED BY public.direcciones.provincia;
          public          postgres    false    217            �            1259    17478    historial_productos    TABLE     �  CREATE TABLE public.historial_productos (
    producto uuid,
    precio_base_anterior money NOT NULL,
    precio_total_anterior money NOT NULL,
    fecha_cambio timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT historial_productos_precio_base_anterior_check CHECK (((precio_base_anterior)::numeric >= (0)::numeric)),
    CONSTRAINT historial_productos_precio_total_anterior_check CHECK (((precio_total_anterior)::numeric >= (0)::numeric))
);
 '   DROP TABLE public.historial_productos;
       public         heap    postgres    false            �            1259    17464    ordenes_compra_proveedor    TABLE     ]  CREATE TABLE public.ordenes_compra_proveedor (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    proveedor uuid NOT NULL,
    id_orden text NOT NULL,
    fecha_estimada_entrega date,
    estado character varying(50) NOT NULL,
    CONSTRAINT ordenes_compra_proveedor_fecha_estimada_entrega_check CHECK ((fecha_estimada_entrega >= CURRENT_DATE))
);
 ,   DROP TABLE public.ordenes_compra_proveedor;
       public         heap    postgres    false            �            1259    17352 	   productos    TABLE     �   CREATE TABLE public.productos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre text NOT NULL,
    marca text NOT NULL,
    descripcion text,
    categoria uuid,
    imagen_file bytea,
    imagen_url text
);
    DROP TABLE public.productos;
       public         heap    postgres    false            �            1259    17491    productos_proveedores    TABLE     %  CREATE TABLE public.productos_proveedores (
    producto uuid NOT NULL,
    proveedor uuid NOT NULL,
    precio money NOT NULL,
    impuesto money NOT NULL,
    total money NOT NULL,
    stock integer,
    fecha_actualizado date DEFAULT CURRENT_DATE NOT NULL,
    CONSTRAINT productos_proveedores_impuesto_check CHECK (((impuesto)::numeric >= (0)::numeric)),
    CONSTRAINT productos_proveedores_precio_check CHECK (((precio)::numeric >= (0)::numeric)),
    CONSTRAINT productos_proveedores_total_check CHECK (((total)::numeric >= (0)::numeric))
);
 )   DROP TABLE public.productos_proveedores;
       public         heap    postgres    false            �            1259    17387    proveedores    TABLE     �   CREATE TABLE public.proveedores (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(100) NOT NULL,
    direccion uuid,
    correo character varying(255),
    telefono character varying(15)
);
    DROP TABLE public.proveedores;
       public         heap    postgres    false            �            1259    17182 
   provincias    TABLE     _   CREATE TABLE public.provincias (
    id integer NOT NULL,
    nombre character varying(100)
);
    DROP TABLE public.provincias;
       public         heap    postgres    false            �            1259    17181    provincias_id_seq    SEQUENCE     �   CREATE SEQUENCE public.provincias_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 (   DROP SEQUENCE public.provincias_id_seq;
       public          postgres    false    216            �           0    0    provincias_id_seq    SEQUENCE OWNED BY     G   ALTER SEQUENCE public.provincias_id_seq OWNED BY public.provincias.id;
          public          postgres    false    215            �            1259    17274    trabajadores    TABLE       CREATE TABLE public.trabajadores (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(100) NOT NULL,
    apellido character varying(100) NOT NULL,
    correo character varying(255) NOT NULL,
    telefono character varying(15),
    usuario uuid,
    cargo character varying(100) NOT NULL,
    fecha_ingreso date NOT NULL,
    estado boolean DEFAULT true NOT NULL,
    salario money,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamp without time zone
);
     DROP TABLE public.trabajadores;
       public         heap    postgres    false            �            1259    17247    usuarios    TABLE     �  CREATE TABLE public.usuarios (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    username character varying(50) NOT NULL,
    password character varying(100),
    disabled boolean DEFAULT false NOT NULL,
    rol character varying(50) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamp without time zone,
    CONSTRAINT usuarios_password_check CHECK ((((password)::text ~~ '$2_$_%'::text) AND (length((password)::text) >= 60)))
);
    DROP TABLE public.usuarios;
       public         heap    postgres    false            �            1259    17310    ventas    TABLE     	  CREATE TABLE public.ventas (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    fecha timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    estado character varying(50) NOT NULL,
    cantidad_total_productos integer NOT NULL,
    subtotal money NOT NULL,
    descuento money DEFAULT 'B/.0.00'::money NOT NULL,
    impuesto money NOT NULL,
    total money NOT NULL,
    direccion_entrega uuid NOT NULL,
    cliente uuid NOT NULL,
    CONSTRAINT ventas_descuento_check CHECK (((descuento)::numeric < (0)::numeric)),
    CONSTRAINT ventas_impuesto_check CHECK (((impuesto)::numeric >= (0)::numeric)),
    CONSTRAINT ventas_subtotal_check CHECK (((subtotal)::numeric >= (0)::numeric)),
    CONSTRAINT ventas_total_check CHECK (((total)::numeric >= (0)::numeric))
);
    DROP TABLE public.ventas;
       public         heap    postgres    false            �           2604    17406    contacto_proveedor id    DEFAULT     ~   ALTER TABLE ONLY public.contacto_proveedor ALTER COLUMN id SET DEFAULT nextval('public.contacto_proveedor_id_seq'::regclass);
 D   ALTER TABLE public.contacto_proveedor ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    229    228    229            �           2604    17208    direcciones provincia    DEFAULT     ~   ALTER TABLE ONLY public.direcciones ALTER COLUMN provincia SET DEFAULT nextval('public.direcciones_provincia_seq'::regclass);
 D   ALTER TABLE public.direcciones ALTER COLUMN provincia DROP DEFAULT;
       public          postgres    false    218    217    218            �           2604    17185    provincias id    DEFAULT     n   ALTER TABLE ONLY public.provincias ALTER COLUMN id SET DEFAULT nextval('public.provincias_id_seq'::regclass);
 <   ALTER TABLE public.provincias ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    216    215    216            �          0    17344 
   categorias 
   TABLE DATA           =   COPY public.categorias (id, nombre, descripcion) FROM stdin;
    public          postgres    false    224   �{       �          0    17258    clientes 
   TABLE DATA           k   COPY public.clientes (id, nombre, apellido, correo, telefono, usuario, created_at, updated_at) FROM stdin;
    public          postgres    false    220   �{       �          0    17403    contacto_proveedor 
   TABLE DATA           U   COPY public.contacto_proveedor (id, nombre, correo, telefono, proveedor) FROM stdin;
    public          postgres    false    229   |       �          0    17365    detalles_venta 
   TABLE DATA           ~   COPY public.detalles_venta (id, cantidad, precio_unitario, subtotal, descuento, impuesto, total, producto, venta) FROM stdin;
    public          postgres    false    226   #|       �          0    17296    direccion_cliente 
   TABLE DATA           K   COPY public.direccion_cliente (cliente, direccion, created_at) FROM stdin;
    public          postgres    false    222   @|       �          0    17204    direcciones 
   TABLE DATA           M   COPY public.direcciones (id, descripcion, provincia, created_at) FROM stdin;
    public          postgres    false    218   ]|       �          0    17478    historial_productos 
   TABLE DATA           r   COPY public.historial_productos (producto, precio_base_anterior, precio_total_anterior, fecha_cambio) FROM stdin;
    public          postgres    false    231   z|       �          0    17464    ordenes_compra_proveedor 
   TABLE DATA           k   COPY public.ordenes_compra_proveedor (id, proveedor, id_orden, fecha_estimada_entrega, estado) FROM stdin;
    public          postgres    false    230   �|       �          0    17352 	   productos 
   TABLE DATA           g   COPY public.productos (id, nombre, marca, descripcion, categoria, imagen_file, imagen_url) FROM stdin;
    public          postgres    false    225   �|       �          0    17491    productos_proveedores 
   TABLE DATA           w   COPY public.productos_proveedores (producto, proveedor, precio, impuesto, total, stock, fecha_actualizado) FROM stdin;
    public          postgres    false    232   �|       �          0    17387    proveedores 
   TABLE DATA           N   COPY public.proveedores (id, nombre, direccion, correo, telefono) FROM stdin;
    public          postgres    false    227   �|                 0    17182 
   provincias 
   TABLE DATA           0   COPY public.provincias (id, nombre) FROM stdin;
    public          postgres    false    216   }       �          0    17274    trabajadores 
   TABLE DATA           �   COPY public.trabajadores (id, nombre, apellido, correo, telefono, usuario, cargo, fecha_ingreso, estado, salario, created_at, updated_at) FROM stdin;
    public          postgres    false    221   (}       �          0    17247    usuarios 
   TABLE DATA           a   COPY public.usuarios (id, username, password, disabled, rol, created_at, updated_at) FROM stdin;
    public          postgres    false    219   E}       �          0    17310    ventas 
   TABLE DATA           �   COPY public.ventas (id, fecha, estado, cantidad_total_productos, subtotal, descuento, impuesto, total, direccion_entrega, cliente) FROM stdin;
    public          postgres    false    223   b}       �           0    0    contacto_proveedor_id_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.contacto_proveedor_id_seq', 1, false);
          public          postgres    false    228            �           0    0    direcciones_provincia_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.direcciones_provincia_seq', 1, false);
          public          postgres    false    217            �           0    0    provincias_id_seq    SEQUENCE SET     @   SELECT pg_catalog.setval('public.provincias_id_seq', 1, false);
          public          postgres    false    215            �           2606    17351    categorias categorias_pkey 
   CONSTRAINT     X   ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT categorias_pkey PRIMARY KEY (id);
 D   ALTER TABLE ONLY public.categorias DROP CONSTRAINT categorias_pkey;
       public            postgres    false    224            �           2606    17266    clientes clientes_correo_key 
   CONSTRAINT     Y   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_correo_key UNIQUE (correo);
 F   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_correo_key;
       public            postgres    false    220            �           2606    17264    clientes clientes_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_pkey PRIMARY KEY (id);
 @   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_pkey;
       public            postgres    false    220            �           2606    17268    clientes clientes_telefono_key 
   CONSTRAINT     p   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_telefono_key UNIQUE NULLS NOT DISTINCT (telefono);
 H   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_telefono_key;
       public            postgres    false    220            �           2606    17408 *   contacto_proveedor contacto_proveedor_pkey 
   CONSTRAINT     h   ALTER TABLE ONLY public.contacto_proveedor
    ADD CONSTRAINT contacto_proveedor_pkey PRIMARY KEY (id);
 T   ALTER TABLE ONLY public.contacto_proveedor DROP CONSTRAINT contacto_proveedor_pkey;
       public            postgres    false    229            �           2606    17376 "   detalles_venta detalles_venta_pkey 
   CONSTRAINT     `   ALTER TABLE ONLY public.detalles_venta
    ADD CONSTRAINT detalles_venta_pkey PRIMARY KEY (id);
 L   ALTER TABLE ONLY public.detalles_venta DROP CONSTRAINT detalles_venta_pkey;
       public            postgres    false    226            �           2606    17213    direcciones direcciones_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.direcciones
    ADD CONSTRAINT direcciones_pkey PRIMARY KEY (id);
 F   ALTER TABLE ONLY public.direcciones DROP CONSTRAINT direcciones_pkey;
       public            postgres    false    218            �           2606    17472 6   ordenes_compra_proveedor ordenes_compra_proveedor_pkey 
   CONSTRAINT     t   ALTER TABLE ONLY public.ordenes_compra_proveedor
    ADD CONSTRAINT ordenes_compra_proveedor_pkey PRIMARY KEY (id);
 `   ALTER TABLE ONLY public.ordenes_compra_proveedor DROP CONSTRAINT ordenes_compra_proveedor_pkey;
       public            postgres    false    230            �           2606    17359    productos productos_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.productos
    ADD CONSTRAINT productos_pkey PRIMARY KEY (id);
 B   ALTER TABLE ONLY public.productos DROP CONSTRAINT productos_pkey;
       public            postgres    false    225            �           2606    17499 0   productos_proveedores productos_proveedores_pkey 
   CONSTRAINT        ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_pkey PRIMARY KEY (producto, proveedor);
 Z   ALTER TABLE ONLY public.productos_proveedores DROP CONSTRAINT productos_proveedores_pkey;
       public            postgres    false    232    232            �           2606    17394 "   proveedores proveedores_correo_key 
   CONSTRAINT     r   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_correo_key UNIQUE NULLS NOT DISTINCT (correo);
 L   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_correo_key;
       public            postgres    false    227            �           2606    17392    proveedores proveedores_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_pkey PRIMARY KEY (id);
 F   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_pkey;
       public            postgres    false    227            �           2606    17396 $   proveedores proveedores_telefono_key 
   CONSTRAINT     v   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_telefono_key UNIQUE NULLS NOT DISTINCT (telefono);
 N   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_telefono_key;
       public            postgres    false    227            �           2606    17187    provincias provincias_pkey 
   CONSTRAINT     X   ALTER TABLE ONLY public.provincias
    ADD CONSTRAINT provincias_pkey PRIMARY KEY (id);
 D   ALTER TABLE ONLY public.provincias DROP CONSTRAINT provincias_pkey;
       public            postgres    false    216            �           2606    17285 $   trabajadores trabajadores_correo_key 
   CONSTRAINT     a   ALTER TABLE ONLY public.trabajadores
    ADD CONSTRAINT trabajadores_correo_key UNIQUE (correo);
 N   ALTER TABLE ONLY public.trabajadores DROP CONSTRAINT trabajadores_correo_key;
       public            postgres    false    221            �           2606    17283    trabajadores trabajadores_pkey 
   CONSTRAINT     \   ALTER TABLE ONLY public.trabajadores
    ADD CONSTRAINT trabajadores_pkey PRIMARY KEY (id);
 H   ALTER TABLE ONLY public.trabajadores DROP CONSTRAINT trabajadores_pkey;
       public            postgres    false    221            �           2606    17287 &   trabajadores trabajadores_telefono_key 
   CONSTRAINT     x   ALTER TABLE ONLY public.trabajadores
    ADD CONSTRAINT trabajadores_telefono_key UNIQUE NULLS NOT DISTINCT (telefono);
 P   ALTER TABLE ONLY public.trabajadores DROP CONSTRAINT trabajadores_telefono_key;
       public            postgres    false    221            �           2606    17289 %   trabajadores trabajadores_usuario_key 
   CONSTRAINT     v   ALTER TABLE ONLY public.trabajadores
    ADD CONSTRAINT trabajadores_usuario_key UNIQUE NULLS NOT DISTINCT (usuario);
 O   ALTER TABLE ONLY public.trabajadores DROP CONSTRAINT trabajadores_usuario_key;
       public            postgres    false    221            �           2606    17255    usuarios usuarios_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_pkey PRIMARY KEY (id);
 @   ALTER TABLE ONLY public.usuarios DROP CONSTRAINT usuarios_pkey;
       public            postgres    false    219            �           2606    17257    usuarios usuarios_username_key 
   CONSTRAINT     ]   ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_username_key UNIQUE (username);
 H   ALTER TABLE ONLY public.usuarios DROP CONSTRAINT usuarios_username_key;
       public            postgres    false    219            �           2606    17321    ventas ventas_pkey 
   CONSTRAINT     P   ALTER TABLE ONLY public.ventas
    ADD CONSTRAINT ventas_pkey PRIMARY KEY (id);
 <   ALTER TABLE ONLY public.ventas DROP CONSTRAINT ventas_pkey;
       public            postgres    false    223            �           2620    17510 1   productos_proveedores tr_guardar_historial_precio    TRIGGER     �   CREATE TRIGGER tr_guardar_historial_precio BEFORE UPDATE OF total ON public.productos_proveedores FOR EACH ROW WHEN ((old.total IS DISTINCT FROM new.total)) EXECUTE FUNCTION public.guardar_historial_precio();
 J   DROP TRIGGER tr_guardar_historial_precio ON public.productos_proveedores;
       public          postgres    false    232    233    232    232            �           2606    17269    clientes clientes_usuario_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_usuario_fkey FOREIGN KEY (usuario) REFERENCES public.usuarios(id) ON UPDATE CASCADE ON DELETE SET NULL;
 H   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_usuario_fkey;
       public          postgres    false    4793    220    219            �           2606    17409 4   contacto_proveedor contacto_proveedor_proveedor_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.contacto_proveedor
    ADD CONSTRAINT contacto_proveedor_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;
 ^   ALTER TABLE ONLY public.contacto_proveedor DROP CONSTRAINT contacto_proveedor_proveedor_fkey;
       public          postgres    false    229    4821    227            �           2606    17377 +   detalles_venta detalles_venta_producto_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.detalles_venta
    ADD CONSTRAINT detalles_venta_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON DELETE CASCADE;
 U   ALTER TABLE ONLY public.detalles_venta DROP CONSTRAINT detalles_venta_producto_fkey;
       public          postgres    false    4815    225    226            �           2606    17382 (   detalles_venta detalles_venta_venta_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.detalles_venta
    ADD CONSTRAINT detalles_venta_venta_fkey FOREIGN KEY (venta) REFERENCES public.ventas(id) ON DELETE CASCADE;
 R   ALTER TABLE ONLY public.detalles_venta DROP CONSTRAINT detalles_venta_venta_fkey;
       public          postgres    false    4811    223    226            �           2606    17300 0   direccion_cliente direccion_cliente_cliente_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_cliente_fkey FOREIGN KEY (cliente) REFERENCES public.clientes(id) ON UPDATE CASCADE ON DELETE CASCADE;
 Z   ALTER TABLE ONLY public.direccion_cliente DROP CONSTRAINT direccion_cliente_cliente_fkey;
       public          postgres    false    220    222    4799            �           2606    17305 2   direccion_cliente direccion_cliente_direccion_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_direccion_fkey FOREIGN KEY (direccion) REFERENCES public.direcciones(id) ON UPDATE CASCADE ON DELETE CASCADE;
 \   ALTER TABLE ONLY public.direccion_cliente DROP CONSTRAINT direccion_cliente_direccion_fkey;
       public          postgres    false    222    4791    218            �           2606    17214 &   direcciones direcciones_provincia_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.direcciones
    ADD CONSTRAINT direcciones_provincia_fkey FOREIGN KEY (provincia) REFERENCES public.provincias(id) ON UPDATE CASCADE;
 P   ALTER TABLE ONLY public.direcciones DROP CONSTRAINT direcciones_provincia_fkey;
       public          postgres    false    4789    216    218            �           2606    17484 5   historial_productos historial_productos_producto_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON DELETE CASCADE;
 _   ALTER TABLE ONLY public.historial_productos DROP CONSTRAINT historial_productos_producto_fkey;
       public          postgres    false    4815    231    225            �           2606    17473 @   ordenes_compra_proveedor ordenes_compra_proveedor_proveedor_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.ordenes_compra_proveedor
    ADD CONSTRAINT ordenes_compra_proveedor_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE SET NULL;
 j   ALTER TABLE ONLY public.ordenes_compra_proveedor DROP CONSTRAINT ordenes_compra_proveedor_proveedor_fkey;
       public          postgres    false    4821    230    227            �           2606    17360 "   productos productos_categoria_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.productos
    ADD CONSTRAINT productos_categoria_fkey FOREIGN KEY (categoria) REFERENCES public.categorias(id) ON UPDATE CASCADE ON DELETE SET NULL;
 L   ALTER TABLE ONLY public.productos DROP CONSTRAINT productos_categoria_fkey;
       public          postgres    false    4813    224    225            �           2606    17500 9   productos_proveedores productos_proveedores_producto_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE ON DELETE CASCADE;
 c   ALTER TABLE ONLY public.productos_proveedores DROP CONSTRAINT productos_proveedores_producto_fkey;
       public          postgres    false    4815    225    232            �           2606    17505 :   productos_proveedores productos_proveedores_proveedor_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;
 d   ALTER TABLE ONLY public.productos_proveedores DROP CONSTRAINT productos_proveedores_proveedor_fkey;
       public          postgres    false    232    227    4821            �           2606    17397 &   proveedores proveedores_direccion_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_direccion_fkey FOREIGN KEY (direccion) REFERENCES public.direcciones(id) ON UPDATE CASCADE;
 P   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_direccion_fkey;
       public          postgres    false    227    4791    218            �           2606    17290 &   trabajadores trabajadores_usuario_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.trabajadores
    ADD CONSTRAINT trabajadores_usuario_fkey FOREIGN KEY (usuario) REFERENCES public.usuarios(id) ON UPDATE CASCADE ON DELETE SET NULL;
 P   ALTER TABLE ONLY public.trabajadores DROP CONSTRAINT trabajadores_usuario_fkey;
       public          postgres    false    219    221    4793            �           2606    17327    ventas ventas_cliente_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.ventas
    ADD CONSTRAINT ventas_cliente_fkey FOREIGN KEY (cliente) REFERENCES public.clientes(id) ON UPDATE CASCADE;
 D   ALTER TABLE ONLY public.ventas DROP CONSTRAINT ventas_cliente_fkey;
       public          postgres    false    223    220    4799            �           2606    17322 $   ventas ventas_direccion_entrega_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.ventas
    ADD CONSTRAINT ventas_direccion_entrega_fkey FOREIGN KEY (direccion_entrega) REFERENCES public.direcciones(id) ON UPDATE CASCADE ON DELETE SET NULL;
 N   ALTER TABLE ONLY public.ventas DROP CONSTRAINT ventas_direccion_entrega_fkey;
       public          postgres    false    218    4791    223            �      x������ � �      �      x������ � �      �      x������ � �      �      x������ � �      �      x������ � �      �      x������ � �      �      x������ � �      �      x������ � �      �      x������ � �      �      x������ � �      �      x������ � �            x������ � �      �      x������ � �      �      x������ � �      �      x������ � �     