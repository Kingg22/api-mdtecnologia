PGDMP                  
    |            mdtecnologia    16.4    16.4 [    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    27704    mdtecnologia    DATABASE     �   CREATE DATABASE mdtecnologia WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Spanish_Panama.1252';
    DROP DATABASE mdtecnologia;
                postgres    false            �            1255    27705    actualizar_timestamp()    FUNCTION     �   CREATE FUNCTION public.actualizar_timestamp() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$;
 -   DROP FUNCTION public.actualizar_timestamp();
       public          postgres    false            �            1255    27706    guardar_historial_precio()    FUNCTION     (  CREATE FUNCTION public.guardar_historial_precio() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO historial_productos (producto, precio_base_anterior, precio_total_anterior, proveedor)
    VALUES (OLD.producto, OLD.precio, OLD.total, OLD.proveedor);
    RETURN NEW;
END;
$$;
 1   DROP FUNCTION public.guardar_historial_precio();
       public          postgres    false            �            1259    27707 
   categorias    TABLE     �   CREATE TABLE public.categorias (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(50) NOT NULL,
    descripcion text,
    categoria_padre uuid
);
    DROP TABLE public.categorias;
       public         heap    postgres    false            �            1259    27713    clientes    TABLE     �  CREATE TABLE public.clientes (
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
       public         heap    postgres    false            �            1259    27718    contacto_proveedor    TABLE     �   CREATE TABLE public.contacto_proveedor (
    id integer NOT NULL,
    nombre character varying(100),
    correo character varying(255),
    telefono character varying(15),
    proveedor uuid NOT NULL
);
 &   DROP TABLE public.contacto_proveedor;
       public         heap    postgres    false            �            1259    27721    contacto_proveedor_id_seq    SEQUENCE     �   CREATE SEQUENCE public.contacto_proveedor_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 0   DROP SEQUENCE public.contacto_proveedor_id_seq;
       public          postgres    false    217            �           0    0    contacto_proveedor_id_seq    SEQUENCE OWNED BY     W   ALTER SEQUENCE public.contacto_proveedor_id_seq OWNED BY public.contacto_proveedor.id;
          public          postgres    false    218            �            1259    27722    detalles_venta    TABLE       CREATE TABLE public.detalles_venta (
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
 "   DROP TABLE public.detalles_venta;
       public         heap    postgres    false            �            1259    27732    direccion_cliente    TABLE     �   CREATE TABLE public.direccion_cliente (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    cliente uuid NOT NULL,
    direccion uuid NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);
 %   DROP TABLE public.direccion_cliente;
       public         heap    postgres    false            �            1259    27737    direcciones    TABLE     �   CREATE TABLE public.direcciones (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    descripcion text,
    provincia integer NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);
    DROP TABLE public.direcciones;
       public         heap    postgres    false            �            1259    27744    direcciones_provincia_seq    SEQUENCE     �   CREATE SEQUENCE public.direcciones_provincia_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 0   DROP SEQUENCE public.direcciones_provincia_seq;
       public          postgres    false    221            �           0    0    direcciones_provincia_seq    SEQUENCE OWNED BY     W   ALTER SEQUENCE public.direcciones_provincia_seq OWNED BY public.direcciones.provincia;
          public          postgres    false    222            �            1259    27745    historial_productos    TABLE     '  CREATE TABLE public.historial_productos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    producto uuid NOT NULL,
    proveedor uuid NOT NULL,
    precio_base_anterior numeric(12,2) NOT NULL,
    precio_total_anterior numeric(12,2) NOT NULL,
    fecha_cambio timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT historial_productos_precio_base_anterior_check CHECK ((precio_base_anterior >= (0)::numeric)),
    CONSTRAINT historial_productos_precio_total_anterior_check CHECK ((precio_total_anterior >= (0)::numeric))
);
 '   DROP TABLE public.historial_productos;
       public         heap    postgres    false            �            1259    27752    imagenes_productos    TABLE     �   CREATE TABLE public.imagenes_productos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    descripcion text,
    url text NOT NULL,
    producto uuid NOT NULL
);
 &   DROP TABLE public.imagenes_productos;
       public         heap    postgres    false            �            1259    27758    ordenes_compra_proveedor    TABLE     ]  CREATE TABLE public.ordenes_compra_proveedor (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    proveedor uuid NOT NULL,
    id_orden text NOT NULL,
    fecha_estimada_entrega date,
    estado character varying(50) NOT NULL,
    CONSTRAINT ordenes_compra_proveedor_fecha_estimada_entrega_check CHECK ((fecha_estimada_entrega >= CURRENT_DATE))
);
 ,   DROP TABLE public.ordenes_compra_proveedor;
       public         heap    postgres    false            �            1259    27765 	   productos    TABLE     �   CREATE TABLE public.productos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre text NOT NULL,
    marca text NOT NULL,
    descripcion text,
    categoria uuid
);
    DROP TABLE public.productos;
       public         heap    postgres    false            �            1259    27771    productos_proveedores    TABLE     L  CREATE TABLE public.productos_proveedores (
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
 )   DROP TABLE public.productos_proveedores;
       public         heap    postgres    false            �            1259    27779    proveedores    TABLE     �   CREATE TABLE public.proveedores (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(100) NOT NULL,
    direccion uuid,
    correo character varying(255),
    telefono character varying(15)
);
    DROP TABLE public.proveedores;
       public         heap    postgres    false            �            1259    27783 
   provincias    TABLE     h   CREATE TABLE public.provincias (
    id integer NOT NULL,
    nombre character varying(100) NOT NULL
);
    DROP TABLE public.provincias;
       public         heap    postgres    false            �            1259    27786    provincias_id_seq    SEQUENCE     �   CREATE SEQUENCE public.provincias_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 (   DROP SEQUENCE public.provincias_id_seq;
       public          postgres    false    229            �           0    0    provincias_id_seq    SEQUENCE OWNED BY     G   ALTER SEQUENCE public.provincias_id_seq OWNED BY public.provincias.id;
          public          postgres    false    230            �            1259    27787    trabajadores    TABLE     j  CREATE TABLE public.trabajadores (
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
     DROP TABLE public.trabajadores;
       public         heap    postgres    false            �            1259    27796    usuarios    TABLE     �  CREATE TABLE public.usuarios (
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
       public         heap    postgres    false            �            1259    27803    ventas    TABLE     P  CREATE TABLE public.ventas (
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
    DROP TABLE public.ventas;
       public         heap    postgres    false            �           2604    27814    contacto_proveedor id    DEFAULT     ~   ALTER TABLE ONLY public.contacto_proveedor ALTER COLUMN id SET DEFAULT nextval('public.contacto_proveedor_id_seq'::regclass);
 D   ALTER TABLE public.contacto_proveedor ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    218    217            �           2604    27815    direcciones provincia    DEFAULT     ~   ALTER TABLE ONLY public.direcciones ALTER COLUMN provincia SET DEFAULT nextval('public.direcciones_provincia_seq'::regclass);
 D   ALTER TABLE public.direcciones ALTER COLUMN provincia DROP DEFAULT;
       public          postgres    false    222    221            �           2604    27816    provincias id    DEFAULT     n   ALTER TABLE ONLY public.provincias ALTER COLUMN id SET DEFAULT nextval('public.provincias_id_seq'::regclass);
 <   ALTER TABLE public.provincias ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    230    229            �          0    27707 
   categorias 
   TABLE DATA                 public          postgres    false    215          �          0    27713    clientes 
   TABLE DATA                 public          postgres    false    216   ��       �          0    27718    contacto_proveedor 
   TABLE DATA                 public          postgres    false    217   ��       �          0    27722    detalles_venta 
   TABLE DATA                 public          postgres    false    219   Ձ       �          0    27732    direccion_cliente 
   TABLE DATA                 public          postgres    false    220   �       �          0    27737    direcciones 
   TABLE DATA                 public          postgres    false    221   	�       �          0    27745    historial_productos 
   TABLE DATA                 public          postgres    false    223   H�       �          0    27752    imagenes_productos 
   TABLE DATA                 public          postgres    false    224   b�       �          0    27758    ordenes_compra_proveedor 
   TABLE DATA                 public          postgres    false    225   !�       �          0    27765 	   productos 
   TABLE DATA                 public          postgres    false    226   ;�       �          0    27771    productos_proveedores 
   TABLE DATA                 public          postgres    false    227   ;�       �          0    27779    proveedores 
   TABLE DATA                 public          postgres    false    228   �       �          0    27783 
   provincias 
   TABLE DATA                 public          postgres    false    229   ��       �          0    27787    trabajadores 
   TABLE DATA                 public          postgres    false    231   ��       �          0    27796    usuarios 
   TABLE DATA                 public          postgres    false    232   ��       �          0    27803    ventas 
   TABLE DATA                 public          postgres    false    233   ۮ       �           0    0    contacto_proveedor_id_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.contacto_proveedor_id_seq', 1, false);
          public          postgres    false    218            �           0    0    direcciones_provincia_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.direcciones_provincia_seq', 1, false);
          public          postgres    false    222            �           0    0    provincias_id_seq    SEQUENCE SET     @   SELECT pg_catalog.setval('public.provincias_id_seq', 17, true);
          public          postgres    false    230            �           2606    27818    categorias categorias_pkey 
   CONSTRAINT     X   ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT categorias_pkey PRIMARY KEY (id);
 D   ALTER TABLE ONLY public.categorias DROP CONSTRAINT categorias_pkey;
       public            postgres    false    215            �           2606    27820    clientes clientes_correo_key 
   CONSTRAINT     Y   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_correo_key UNIQUE (correo);
 F   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_correo_key;
       public            postgres    false    216            �           2606    27822    clientes clientes_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_pkey PRIMARY KEY (id);
 @   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_pkey;
       public            postgres    false    216            �           2606    27824    clientes clientes_telefono_key 
   CONSTRAINT     ]   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_telefono_key UNIQUE (telefono);
 H   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_telefono_key;
       public            postgres    false    216            �           2606    27826    clientes clientes_usuario_key 
   CONSTRAINT     [   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_usuario_key UNIQUE (usuario);
 G   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_usuario_key;
       public            postgres    false    216            �           2606    27828 *   contacto_proveedor contacto_proveedor_pkey 
   CONSTRAINT     h   ALTER TABLE ONLY public.contacto_proveedor
    ADD CONSTRAINT contacto_proveedor_pkey PRIMARY KEY (id);
 T   ALTER TABLE ONLY public.contacto_proveedor DROP CONSTRAINT contacto_proveedor_pkey;
       public            postgres    false    217            �           2606    27830 "   detalles_venta detalles_venta_pkey 
   CONSTRAINT     `   ALTER TABLE ONLY public.detalles_venta
    ADD CONSTRAINT detalles_venta_pkey PRIMARY KEY (id);
 L   ALTER TABLE ONLY public.detalles_venta DROP CONSTRAINT detalles_venta_pkey;
       public            postgres    false    219            �           2606    27832 (   direccion_cliente direccion_cliente_pkey 
   CONSTRAINT     f   ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_pkey PRIMARY KEY (id);
 R   ALTER TABLE ONLY public.direccion_cliente DROP CONSTRAINT direccion_cliente_pkey;
       public            postgres    false    220            �           2606    27834    direcciones direcciones_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.direcciones
    ADD CONSTRAINT direcciones_pkey PRIMARY KEY (id);
 F   ALTER TABLE ONLY public.direcciones DROP CONSTRAINT direcciones_pkey;
       public            postgres    false    221            �           2606    27836 ,   historial_productos historial_productos_pkey 
   CONSTRAINT     j   ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_pkey PRIMARY KEY (id);
 V   ALTER TABLE ONLY public.historial_productos DROP CONSTRAINT historial_productos_pkey;
       public            postgres    false    223            �           2606    27838 *   imagenes_productos imagenes_productos_pkey 
   CONSTRAINT     h   ALTER TABLE ONLY public.imagenes_productos
    ADD CONSTRAINT imagenes_productos_pkey PRIMARY KEY (id);
 T   ALTER TABLE ONLY public.imagenes_productos DROP CONSTRAINT imagenes_productos_pkey;
       public            postgres    false    224            �           2606    27840 6   ordenes_compra_proveedor ordenes_compra_proveedor_pkey 
   CONSTRAINT     t   ALTER TABLE ONLY public.ordenes_compra_proveedor
    ADD CONSTRAINT ordenes_compra_proveedor_pkey PRIMARY KEY (id);
 `   ALTER TABLE ONLY public.ordenes_compra_proveedor DROP CONSTRAINT ordenes_compra_proveedor_pkey;
       public            postgres    false    225            �           2606    27842    productos productos_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.productos
    ADD CONSTRAINT productos_pkey PRIMARY KEY (id);
 B   ALTER TABLE ONLY public.productos DROP CONSTRAINT productos_pkey;
       public            postgres    false    226            �           2606    27844 0   productos_proveedores productos_proveedores_pkey 
   CONSTRAINT     n   ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_pkey PRIMARY KEY (id);
 Z   ALTER TABLE ONLY public.productos_proveedores DROP CONSTRAINT productos_proveedores_pkey;
       public            postgres    false    227            �           2606    27846 "   proveedores proveedores_correo_key 
   CONSTRAINT     _   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_correo_key UNIQUE (correo);
 L   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_correo_key;
       public            postgres    false    228            �           2606    27848    proveedores proveedores_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_pkey PRIMARY KEY (id);
 F   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_pkey;
       public            postgres    false    228            �           2606    27850 $   proveedores proveedores_telefono_key 
   CONSTRAINT     c   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_telefono_key UNIQUE (telefono);
 N   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_telefono_key;
       public            postgres    false    228            �           2606    27852    provincias provincias_pkey 
   CONSTRAINT     X   ALTER TABLE ONLY public.provincias
    ADD CONSTRAINT provincias_pkey PRIMARY KEY (id);
 D   ALTER TABLE ONLY public.provincias DROP CONSTRAINT provincias_pkey;
       public            postgres    false    229            �           2606    27854    trabajadores trabajadores_pkey 
   CONSTRAINT     \   ALTER TABLE ONLY public.trabajadores
    ADD CONSTRAINT trabajadores_pkey PRIMARY KEY (id);
 H   ALTER TABLE ONLY public.trabajadores DROP CONSTRAINT trabajadores_pkey;
       public            postgres    false    231            �           2606    27856    usuarios usuarios_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_pkey PRIMARY KEY (id);
 @   ALTER TABLE ONLY public.usuarios DROP CONSTRAINT usuarios_pkey;
       public            postgres    false    232            �           2606    27858    usuarios usuarios_username_key 
   CONSTRAINT     ]   ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_username_key UNIQUE (username);
 H   ALTER TABLE ONLY public.usuarios DROP CONSTRAINT usuarios_username_key;
       public            postgres    false    232            �           2606    27860    ventas ventas_pkey 
   CONSTRAINT     P   ALTER TABLE ONLY public.ventas
    ADD CONSTRAINT ventas_pkey PRIMARY KEY (id);
 <   ALTER TABLE ONLY public.ventas DROP CONSTRAINT ventas_pkey;
       public            postgres    false    233            �           2620    27861 '   clientes clientes_actualizar_updated_at    TRIGGER     �   CREATE TRIGGER clientes_actualizar_updated_at BEFORE UPDATE ON public.clientes FOR EACH ROW EXECUTE FUNCTION public.actualizar_timestamp();
 @   DROP TRIGGER clientes_actualizar_updated_at ON public.clientes;
       public          postgres    false    216    234            �           2620    27862 1   productos_proveedores tr_guardar_historial_precio    TRIGGER     �   CREATE TRIGGER tr_guardar_historial_precio BEFORE UPDATE OF total ON public.productos_proveedores FOR EACH ROW WHEN ((old.total IS DISTINCT FROM new.total)) EXECUTE FUNCTION public.guardar_historial_precio();
 J   DROP TRIGGER tr_guardar_historial_precio ON public.productos_proveedores;
       public          postgres    false    227    235    227    227            �           2620    27863 /   trabajadores trabajadores_actualizar_updated_at    TRIGGER     �   CREATE TRIGGER trabajadores_actualizar_updated_at BEFORE UPDATE ON public.trabajadores FOR EACH ROW EXECUTE FUNCTION public.actualizar_timestamp();
 H   DROP TRIGGER trabajadores_actualizar_updated_at ON public.trabajadores;
       public          postgres    false    231    234            �           2620    27864 '   usuarios usuarios_actualizar_updated_at    TRIGGER     �   CREATE TRIGGER usuarios_actualizar_updated_at BEFORE UPDATE ON public.usuarios FOR EACH ROW EXECUTE FUNCTION public.actualizar_timestamp();
 @   DROP TRIGGER usuarios_actualizar_updated_at ON public.usuarios;
       public          postgres    false    234    232            �           2606    27865 *   categorias categorias_categoria_padre_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT categorias_categoria_padre_fkey FOREIGN KEY (categoria_padre) REFERENCES public.categorias(id) ON UPDATE CASCADE ON DELETE SET NULL;
 T   ALTER TABLE ONLY public.categorias DROP CONSTRAINT categorias_categoria_padre_fkey;
       public          postgres    false    215    215    4800            �           2606    27870    clientes clientes_usuario_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_usuario_fkey FOREIGN KEY (usuario) REFERENCES public.usuarios(id) ON UPDATE CASCADE ON DELETE SET NULL;
 H   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_usuario_fkey;
       public          postgres    false    216    4838    232            �           2606    27875 4   contacto_proveedor contacto_proveedor_proveedor_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.contacto_proveedor
    ADD CONSTRAINT contacto_proveedor_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;
 ^   ALTER TABLE ONLY public.contacto_proveedor DROP CONSTRAINT contacto_proveedor_proveedor_fkey;
       public          postgres    false    217    4830    228            �           2606    27880 (   detalles_venta detalles_venta_venta_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.detalles_venta
    ADD CONSTRAINT detalles_venta_venta_fkey FOREIGN KEY (venta) REFERENCES public.ventas(id) ON UPDATE CASCADE ON DELETE CASCADE;
 R   ALTER TABLE ONLY public.detalles_venta DROP CONSTRAINT detalles_venta_venta_fkey;
       public          postgres    false    233    4842    219            �           2606    27885 0   direccion_cliente direccion_cliente_cliente_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_cliente_fkey FOREIGN KEY (cliente) REFERENCES public.clientes(id) ON UPDATE CASCADE ON DELETE CASCADE;
 Z   ALTER TABLE ONLY public.direccion_cliente DROP CONSTRAINT direccion_cliente_cliente_fkey;
       public          postgres    false    216    4804    220            �           2606    27890 2   direccion_cliente direccion_cliente_direccion_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_direccion_fkey FOREIGN KEY (direccion) REFERENCES public.direcciones(id) ON UPDATE CASCADE ON DELETE CASCADE;
 \   ALTER TABLE ONLY public.direccion_cliente DROP CONSTRAINT direccion_cliente_direccion_fkey;
       public          postgres    false    221    4816    220            �           2606    27895 &   direcciones direcciones_provincia_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.direcciones
    ADD CONSTRAINT direcciones_provincia_fkey FOREIGN KEY (provincia) REFERENCES public.provincias(id) ON UPDATE CASCADE;
 P   ALTER TABLE ONLY public.direcciones DROP CONSTRAINT direcciones_provincia_fkey;
       public          postgres    false    229    221    4834            �           2606    27900 5   historial_productos historial_productos_producto_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE ON DELETE CASCADE;
 _   ALTER TABLE ONLY public.historial_productos DROP CONSTRAINT historial_productos_producto_fkey;
       public          postgres    false    223    226    4824            �           2606    27905 6   historial_productos historial_productos_proveedor_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;
 `   ALTER TABLE ONLY public.historial_productos DROP CONSTRAINT historial_productos_proveedor_fkey;
       public          postgres    false    4830    228    223            �           2606    27910 3   imagenes_productos imagenes_productos_producto_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.imagenes_productos
    ADD CONSTRAINT imagenes_productos_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE ON DELETE SET NULL;
 ]   ALTER TABLE ONLY public.imagenes_productos DROP CONSTRAINT imagenes_productos_producto_fkey;
       public          postgres    false    224    226    4824            �           2606    27915 @   ordenes_compra_proveedor ordenes_compra_proveedor_proveedor_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.ordenes_compra_proveedor
    ADD CONSTRAINT ordenes_compra_proveedor_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE SET NULL;
 j   ALTER TABLE ONLY public.ordenes_compra_proveedor DROP CONSTRAINT ordenes_compra_proveedor_proveedor_fkey;
       public          postgres    false    4830    225    228            �           2606    27920 "   productos productos_categoria_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.productos
    ADD CONSTRAINT productos_categoria_fkey FOREIGN KEY (categoria) REFERENCES public.categorias(id) ON UPDATE CASCADE ON DELETE SET NULL;
 L   ALTER TABLE ONLY public.productos DROP CONSTRAINT productos_categoria_fkey;
       public          postgres    false    226    215    4800            �           2606    27925 9   productos_proveedores productos_proveedores_producto_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE ON DELETE CASCADE;
 c   ALTER TABLE ONLY public.productos_proveedores DROP CONSTRAINT productos_proveedores_producto_fkey;
       public          postgres    false    4824    227    226            �           2606    27930 :   productos_proveedores productos_proveedores_proveedor_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;
 d   ALTER TABLE ONLY public.productos_proveedores DROP CONSTRAINT productos_proveedores_proveedor_fkey;
       public          postgres    false    227    228    4830            �           2606    27935 &   proveedores proveedores_direccion_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_direccion_fkey FOREIGN KEY (direccion) REFERENCES public.direcciones(id) ON UPDATE CASCADE ON DELETE SET NULL;
 P   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_direccion_fkey;
       public          postgres    false    221    4816    228            �   s  x�}��n� �ם��.�*�1u�Q)M��t����c��I����#��za�h&����w���痷g7w����U�]l�#���i�0U�N.��nW���P9�a���1PV3:ϭr9WG'����>M���U����TI��S�X�K�Җ-�P��M���3N@�t��	� k���vhX:�|�ͪ�i�M�f?��5�4�ɾ0f�C꟟��t ���ς�I)�M�1mD-�K��9A�R�d�7�|i�ŉ@q���5Zp>�ڵ��p�;`R{�:����8��8��a���b��~�26��;e+i_����!����Z�Pw� �2k�`6`mx[�Ʉ��O{�
K��n��t���2��휶�0nuk��YP�de���2.:o�
��Ʉ��6LB�<j-����4a��sː�ʉ9Diͭ�%�J(&Df[�z-]����[��d픜A������Rc��/w�G`A����0���L72��c�ۙ�i)&%"����9����P�������M24Zq���I���a$s�e'@��CּMa�	cI����#Š�H$6���u���LԎ7�q&:H�S� Ӝ�3�Ј������&=V��]�O��C�2>�V�?*�Rb      �   
   x���          �   
   x���          �   
   x���          �   
   x���          �   /  x��T�n1<�_�ۂ�����+��	���p��-�X�F�K����,�>	�9x��j_���v����~U���v(�u���a7�^�]~�~u�z�z]kK�Y����Kk�m�@lb	.��x}�������U�#�Y8��he�B��d�w~��l�/>��1����(_��LĘ��6���j3���yT���/�p�U�r�j��)HD\#��DC���z�;>K���u��P��!������Q�F�_�ƣ�T�	�b�ԥ��J>'D����nP���D�Cv�]J1J1'�^}�D�'h^m������� v'k
�;��D)�ēl�էa<�����V�/�{�{�^2k��j���7t�GR�'y��j�ME��G��:�`����("��9����=����"p=��*�O��T�&G��Fo�^��z�^]n���l��s)�n�����\��\����@���\=m�uNmd���C��ne��aS3�R�Ĵ��@�ނwIW��[uc��Շ4L��z���Y��l��$�5�{gp~��u�����j���-O      �   
   x���          �   �  x��X[S�Y}�=�L����>��결UD�l}I�+C�%��YG��=Rt���S-	de��%�'/���w�����8���U8/����I^���{|��w����?��D�d����(*#�*�pFA��z䓧�N���>~r1�O�_z���2���t6�J��&W[��ޢ+�:��������zSh�rz�?�$yi�Q�y.��rf�t%O ��?O:ǅR$�4#i-�VYd�ܑ���ѕq��N�%�y��n�`.�=��+��e� ���׍�d<������x/���z�b����Q��l��k��^9�Y*4o�m�I����G#�������,���FXT�3�YWgT��p+�&��fG��T{i2+C�dO(c�s��"'ŃL�L"����|���P�Q����LLڤ\�^# �b���E�\��)�N>��x�N����(	��W�/^}|�����OGߧ(xv�Rd�
�,kb�hɜ�>[��v	��ʫ��E�#�h�g�r������~�|X��ð��ˏ�է7�gr�h��������|	�kͫΌ� oCj���cPD��,��S(IRL.�R�ŋ����8I!��j��a`�*|��� ���3"����z��b�H2h#qNa�#�SAI�0!��n�eqrJ��\�R��
W&�Rj
?⹾���n;������^
�=����;_t�_���c����p~c�+�ɔ�3n� �@,�����$�װs�D�Z���S0��LJ��k.t���tpv������~w���������}~}��S����v�.�_V��g�� .Dd�zΔ#�*��Y��xM&��J�B�)�r��娤�����XƟ��]
i��$Mz����Y�:���l4�~��q馓q7�R��JW�?5�Jؘ��S281	�f��x+pf:/�'�q��}bd�b�8�DJ��"u܌�Z|���������`����+�驀t���x�D�Ҁ�*�H�\�5�Rx\���j�i�͚��`����F��9��e�}:���dOh~��T	�B,�����c����Ӥ��b%�*פLp�{���V����R�_�l߁lu�[������{S3^��nz�$�������M�<�Fev�gط'؟�gZ9�	R��'+�@[3x�n�1��dK�,� 
;%��'(%�Z����V'�3L��k��	��R޸z�&����oF�{�hJF@�H����TTp��b֫|����Ñ�@H �3�[l@��B��Y��Mg�[r�]OY�9!����$�'�=nz���v�w��$-�8�І )(�� >6	2]eP6(��)�@@���eE�)�XA�!p,`)?�����2[d(-@v��|k\潔ǽ�b2]Io׻����b���i8�}�U(!���C��2i���:B�J��R���f�k�OiP��%��l],'���i�����&f�2�Y��koRbD,�P�\��e��Wi ��r��n3@��^�N%[L���!q3.����`�ɱ��)�[��ON�; #[Hi�_��"=��]��(�`��,����i�B������S���W<��а>���gc.gǟϗI���p˚�v�y��rsQT�����U�4��W���H��,�º"tՍ���-u~��n�ɪq��vۘ$�'����U�V�UV�&�(�-y���tŤ�Iju��A>,;��OgË����4ɹGwl���1q�%!V�"��\!�u5��B�����05��b>��0H!�=��!�V\�~��{��8d՜�Qc>��%+��2�
k4qe텗\=t�`�� ��Q�1џ�)�oB��,����ˇA��tPI!ؑp-�)�S]�L�t�*����dZ�0Rd�!%Z
��6c�|�q��W�/@��ܰ�s���2Aނ.!̰���;�N4e�H��6x)��4j�������������2���|�B���T+L.�9��o����D��|U:T1*P�?r|,�(��*^4l���J'����}���ƗܪPu �0$�tǊu���\�;�Xr�Prӎ,����C����$.6�a����|�7��`j��$F�s�bslwR�_��x[�6�z��?�Þq(��J�7�?+n����ݽr���Y( Cl�p..ѣ�Ff��x_g��c��H�n�2��$e�O5�C��o��a��Fl���7ztx��2\��F���><>m��FlP��j.@��|����q� �Ip(�o%2�DQ
>+W����S�V�ӫI�xt�]�>�T�X���
X����֫����b#����"�G�
�,���䜍�|뻳tu�e��q10��#j���<��AI��j�-���t3��q�F+R�UgO�����ٗ��w�*\T���@�����'�J�S��4�7�lh�}��Bf�f���s��i�)����w���|�1xQ�JmK�-�q�PL�xt��!9��k^I؂i߲	�v�D�z�nK�8�Ƕt��aRe�"��YЙ�n*J�4)k����B�E���s:Y�n����!�MA?}9y{4�+��#I�_Ԧ}CB�A2���I�j�f~�	i�%�BT$�	L����n����۽�����
��P�8oڢ�j!�!�9d;4���v%r���*�H2�'J�4uv�C�.���f��@<����A�<�2�ABH1�@`A�+y]�T�8#���ZL��kR)P!M�o;����c�0�
U�C��pqI�c��f�C��+1��/�J� �X�ӤH,{' �a��<|+~.׻�/@B�Hx�/��v�����&�+��� ���2���ҟװ�V���8p�azv����ɛ��Zlj���:1���L����n��Jm��!�ʴz[`v��&bi��8(���w⧥�IB�Rer��4������턓��G��u�C�      �   
   x���          �   �
  x��X�r�F]����TMO���Y�aˮX�"��l�)#:q���Eʻl�cs�AZ��	V����>�9�_��{~{߼zs�C�����YOc܆y�4��^�}~����.X���X����l�Dgn��4ؐ��w��mڬ\Ss�֩o�dy���]��q���Vn�ǈ����k���b�Ȳ
*&F��� "q����*���w��w��Zf����$["s��j��e&s�+_y5������4�>�W����nx�!}�'M���®&���](蚵�2����g�9kˍ<h���D�`��4�d���5��W^��k_�>����1/�O�p�9�}ypX,&��yl^�7S�t�3�X�����b��S��E�L�&R(Fu�F�NYf�s�����4���܅nX��9���p4�0��8�>�g�?�xr$eNIi�G�dF�'���J-�tf�d���ӋQ�����G(������OMz��2<�z*R*u<H��4D��Fk�u9*�3��¹�F$��h^!y}s1N��lY��lB�v�s�^{H��ݗ��f�v�>-i�NG�'܋@�(�:XB��l#�{���U���i�n9%&��xJ�[Q��:�Ʃs���u���ysyy[��7���ݗ���t�߾t��b�*1��yJGCH
1O��|��[��W��d�o�W^v���(���ˆݟ��t��vx(���+��sp{P��p�X�9�0������5o�]��fØd�m=BjL&.�hJ��l�����s^M�ϥΚ���5�jZ߼{u���x��a[����4����ɡ�k�'���`�min#�-ͻPJ���S���+m�T��0�n�O���wWgG�h��+@�S�67�v�\�}<�jeP�S"OD*0���:�ۚ��J'������!��W5�Ǉ�|�����+���<�۬ӴH�o���v�U�~�=GX�A��)��x�0u+�6Za �2y��蟘[�R��V�����\�ܖB�u���q��4����z.?� ��)�nSО-D�2�2��\��dȰpI�H[�M�ɳ��:��4��6��8רݍç��~�y��^a�!�~�R�[�щP��Q1�CɁ]�Ԁ�\#�ڊTK�G���)��Ŷ�OS}�R���od��^��c����#{:��O	��%g��9d(I���ta�s����|����d�{��uQ�i	����G��.!�j���>dB,�p�P�ᔂ�$�9�"o���!���oT�r�7��HW�k7@�^�ܕ8�u��hl��H6Ep�IJi�x�i)
�%u��V
~�	���\��*s|��_!5�������{<"|yy{"�Vs���$xM��'�N�P.��r��ċ��ql��&TG�r�١�����/B��R���,e�τ�Z^6�1����l�
�n�1�b�T{�����b�=3V'��Ǫ�o���ŉ��>�V�2�E^ri��Ea5�O����Zɏ�w�������A>��2�y[��+�pI�����3g�_��;�)��Z#�ê5�ۼ�Mּ�!�~?<�/���%�맦��0��l %�Y�zf ��-�)O������a�f�En���	��$m�e�&��ڛ�3�z��/��QV����Y`H�0��vK�Oa艨�q��l �e�� !�Vne�{ɜA��t��z��~yOK{ܦ"����ݺ�f'"�"9��$Q�= ���Jka�Z��U����ӸJ�t��%r���E�(��M�y��=����V��8#`V��H�GPY�����{�]ss񭹿xD�89���w�7���(�f�7�$���M(%n���gk��b�.ʈ�0Qn���O�����jƀ0V����K�R���۫��7-������<e1�1��U*��CG��E��	�B�TB�(+&0R�H��o!\o1���2W��|����R_�{�?H+
k\w�ND�k���'�F%�	פ�j3wB;Q�wӏ3Իt�����8��"�m<"K�5�[岸���ǏeX_�n�O���p�T*���1��)���a.�ߢUz.P[}7z���zqy}"$��xKf
�z��c:M1c�����C���1lW�m��|X��u�� g_��;ϐ&����+��kKaڥ52���|ܖ��Խ�������Gk0S?.�CO���63�yKe�N4��5�&��]c��:�Q���0el%h��7��:[����j���n��b������,��������yi�!�'�Lȧ*�+D�"6I��B�`*j�\=�u^���M�/���xq�v��k�j�ʤq".U�QRB�B���ek�#�v!����Y�T���>I���Q���㉰��(ʖu�~{I|p���Z���E�;䤹p�Z�R��7���jȦO�	9+�dM(,qTl1�b�}�O�\�Fmٮdk\�4���/�4-�MUǜ6�������./�s�b W�C�����,=0���4��Hu�PL.���ǁ\l�Qx���V���gC�Ӷ�{�)#��O��3T����o�s � �\0S�`�<##*�t��Y�-J�\@rs���b6-w�{]� du��.n�\M��Ӧ���q l�W�a?�S
��� ��P�S�F�.��OT���D��j��m>��H��^�'�hf��c\��5���	U��R�	W���+l�cy
��ˬzW�W���uF�B4����-X�:������0o���w��={��Yb�      �   �  x��ZM�4�m>g�{{`5�HJ���>0l vr5�	�;���ê��E2�V���Ģ��C����O���?�����o?���������������������O]��������O?�˯����
k��-H�,s�%�)��?~�>*i-l�F�u����A7�t�����e�+��/l� V5�[Xi[�2����G�?~�����&��;E��R���_����eŀH
�WRh�W��c]�+��&���V�f��u
�[[VT�:��/����|0^��7u�<U�lBA(�P��Pr��H_���[r�lu���-�4C�ނ�d�b/ɯ3%��9�b	R�
�'<o強������:��0�9�� �LSt����AڐТ O�dX�3�z�*���P&%$0�Щ����א��z.ꇔ#>J��m|�13*#�1
��Z�3�Xgkȭ�E��)�ÂI\���X��.�uLs�m��=�`rţ�b)bU�R�u|�d�t_�FV7��o�[[
�k���\s�-��oq�R{נ�p]��ÎMr�ٰ&��>��x|�WJ��%��aA&���?t_Q��l�yu(��4����F�X���еe+���������u6�CE�O��o\gg�}�k�W�_^+��I��:�+%Ԏ��2*�����Mxu�V64�w�|�r��`���,��=�@�c���vY5��a0a�R}^ ᔨF�'*�(��Z��cA�$<Q��ɖw��"��H�.�kA@C����u�Xcoh�
��=��.!;��P�i���n�̶�CQϋ���M�ES^��i�]��B1�`�����X`�(v�0x�o� ���!�8��f��c8�|�\�� ?e ~��uVt'�L���p<!P�L^c���:n>v�4��9�^���i��5����!���R�d�sLt�Țkŀ� 	�;0��sҞ �4@�Yφ���%�17�Fk�^��ؕ7:j��Y��PS�r|X��2��MC_m 	k�g�>���_Y����Z>�ӱ��"�A ���1r�,f}O���-��
ϰ���3�w��^���dmp&�΁��6�1�sye5��yVɸ'Q&y�CBI�Zh� E�c�Mc��Ȁ�����kYHx�<��cj~��A�l��eqq)m�:�|�)%�iD�����})U�-�B��;��(��F����*Ƈ�R����G�p�6�I[﫣f2XY��-˽+�RJ���j�qg<EF�C�9��8:(�Ev��^pK���s���n�QrMZQ�K��WD;ǁ4bj@O�M��^��Y���=�Tx E
sxU�4Z��-r�T�2JB#�"�Z���� �@�P�Ƣ\�RN������`@�Q�)�m�]�o�B�lf�� �������Y>
;S������
*���� z�RQ�P���EM9��?��03Q�sgMk���܉�{���싄�Y�	�h/:Vk�ď|�,w"Gnۤ�k}`@ח�W��m�]��:&���I�������*s����R.�9+��eb���2�	K��i�2l	��� �J>}���a�e�qD�!e�
K�K r�����:�-��j 1C���:��������)o�D�/@aB�qGp�t���c ;��`�ћ2�꧉��z��H8ijG��5�P5�模�@���`���p�۠�&�"�3(�xJ����g��%�=���bU[3�B8�

'�����vV�� �	�
1��+���D ;&پ�p8JC[L@�<2UƋR���a�1L\�	8)���t)�s���)��ж�QswM����9���֛�I��N��B9�[���� X
��܀d��[$рm������[��U�{-���+�1s��"��)����۹eƄg�2�����2�:�
��@��1�Ȉ:?����]`S� �	!vP� 
�^|wiU���bt<��Y�h x��h{�V��DC�s�m�� (�P�OZr���F���sa��t�K�b�t�M*�Xn�p��M��DO�����@]j<�~��!�������	5�j�<�Ț���U��!�k�w+�%'W}Is�� ��M������!����,BJ�Xo}i������j ���F�7��.Y��G;6H�SL=���`��U�J�/�%_i�<_�ކX�г�Y�b�K� �	�
+���oB|H/r���}��9�c�}þ]��ٷG\y�J_��\G�{�ntJ0�mȎ�s���6j��o�ӧ�a�ll�����7��7�{qP�@��+�M�O)��I�|1�-<��� JC�N2�J��3=�	tTdݥ��ft�+fO���Z�E���s��d��-��ͽ�U*%�	��]�/̎8L5�����D���*���tb��RKQ[≻��(�h��w�ܲ�~�C�L�>����T�7|���3,q������������Yѭ�X@��� +.�6�:D�����{(�ŠY�vZ��D_ �P3\B�؉Ek�7!>-�{$�7�c�:��q+�Z�YbN�7��S�\+3�E����L1˒���j��j�|�t�_ؾ�k�!U �����Q���� �Gy�Õf�bL�������1�Zx��Gh%b(=�΂�9Z&B]L<.��k�x t�Xr�o�\�4���t��lS��}�������,�	-���X�������v�3��"]�v=O��v��ݷ�8��F�	!j��c ��#�2r�B8�qaxDh"�����vB��6�Q�W�L�~-}�(��5�K{X��NY�؄֛6]���lm-_����
�� ��+j�!)#��>$�p������PqM܌�}c�{�����Ol�כ[���X�16}7y!6WE����ȫ������r^�zseA�
9U��(��FK#�	� �E�e�w�R��C��\��/�q?fst�>�6vyl�R.CPi�O�l��s��y��w��|�.�4?�l	��q$?� m�7Y��B�!��I(�Ȝ�ՉS�	E�*����=p���q�7W��V4�I5�G/otϛ��s�V��bg��uVC���`��^��\�ޝ���	X}��X�=�|g��h��B��s��i�O�?���-��ܥ�Ep�F����ApDh0� ^��Gm)N�
�ۊ8e��;����]�,ڧ����-� s=+��(��n�´�?8������4��[�r�_]���0mw� ]�&�Z��L����q�cj4n��yf;ΰ��%�.6kF��j/P��KL\dx�G�;S� �A%�5Q��ϟ57��]�E�eʔ���J\^��([���F��~�p��u#��1ji	%�I]˽�!��0�����yy�ƀ�@~d���G-�O�������ݟ XC�MϾs�?���|���^P7�.<�l8�0�Df��������*��]�[�k��(u�@{�P�@Ď
��BloB|�������g�X�g�/� ��#��P��2�E�ކP��wo���a���}4d��?�U�*X�1����2��UE��=f֩�Tݪ���c�N.�8�G�2�{Cü��C�{��1��I��k���B&� 
d���`�.D4r��<��i��r�S�7���H�Bv�Z�9��p��_���<���T�C��*ˏ�2�9��*h�m���<�x�������?���? �hqB      �   �  x�ETKo7>׿bon �Ѓz��mR�M�@�4葒(���3���F��KMm����H����Ǜ۫O�wo>��{x��P.��������~��ru{�Ï�kv
B�	0� �+�-挕�n����>���;����t���
�0@,1�3��V���i�Oj_�����,�w�����EG�	#g$��:��D�3D�>�~a���j�G�v����	�%[����
Ԭ�ՆLJ����z��~.�nR�x���9X���@Y3�d]k!צ}o}��4�r����eXw4����;�3��M7(�1 q������R�b6R�Ƒ�y��t�#����r�S?F�C�m�:�����W�� �>bPB�S\�E��_�F���̏����XC] ��R���kD�"r�u�le�y|��,@�|��}n��kju-�AZ��� EG���Hb��oO4���b��ܷ�tu<�2�C?m�B�J�{��X�vN~�*W�v��|��޶��0B����a�

�^�(�Ҧ]N.F����D^N�S�%�cVQF#6 =ؤ,�l捒q{����a=�؎������,Jh�+�R��XЊ#�r����*9뭿�R����
�J2��
���g�Up�c�u��b����2�t�Ph㓦WP}xPF���T��-�J��"�NNB*D����/�P��no�Z$��S=���Wo}ݘz��|���_Eu�r}�Y��K^&��6F�JTR���I��l3��
̲�z?l^1�Y�6M^(���dUO��Y��������};�˻�iK��y!|��ٱg�y��pA���2� /�曯�8׷����IW�E	���$�*��E2��z^�uM�o��B�$@<�9����ٿ�~:      �   	  x�U�KN�0�59��
R�0�Ċ�*��Њ�$����.N��N%6b�|1��屜ol���$i|��$�f�6y-�õV/B�_N�4��g]�TAT�2�U�K~L~���n}���k�)}}��'�ų��Dc��nÙ!�ךk�pJ0U�([�x;#�C�+��pN0�+��p�]���ux����Uε�����2�zRٷ��F��I����`ƛ�wQldK��a,�vSK�X�
e��ʆ���z.f��j�K���"��/Hp�      �   
   x���          �   
   x���          �   
   x���         