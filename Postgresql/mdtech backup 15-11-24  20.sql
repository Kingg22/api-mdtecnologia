PGDMP  )                
    |            mdtecnologia    16.4    16.4 Z    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    27151    mdtecnologia    DATABASE     �   CREATE DATABASE mdtecnologia WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Spanish_Panama.1252';
    DROP DATABASE mdtecnologia;
                postgres    false            �            1255    27373    actualizar_timestamp()    FUNCTION     �   CREATE FUNCTION public.actualizar_timestamp() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$;
 -   DROP FUNCTION public.actualizar_timestamp();
       public          postgres    false            �            1255    27371    guardar_historial_precio()    FUNCTION     (  CREATE FUNCTION public.guardar_historial_precio() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO historial_productos (producto, precio_base_anterior, precio_total_anterior, proveedor)
    VALUES (OLD.producto, OLD.precio, OLD.total, OLD.proveedor);
    RETURN NEW;
END;
$$;
 1   DROP FUNCTION public.guardar_historial_precio();
       public          postgres    false            �            1259    27245 
   categorias    TABLE     �   CREATE TABLE public.categorias (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(50) NOT NULL,
    descripcion text,
    categoria_padre uuid
);
    DROP TABLE public.categorias;
       public         heap    postgres    false            �            1259    27163    clientes    TABLE     �  CREATE TABLE public.clientes (
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
       public         heap    postgres    false            �            1259    27312    contacto_proveedor    TABLE     �   CREATE TABLE public.contacto_proveedor (
    id integer NOT NULL,
    nombre character varying(100),
    correo character varying(255),
    telefono character varying(15),
    proveedor uuid NOT NULL
);
 &   DROP TABLE public.contacto_proveedor;
       public         heap    postgres    false            �            1259    27311    contacto_proveedor_id_seq    SEQUENCE     �   CREATE SEQUENCE public.contacto_proveedor_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 0   DROP SEQUENCE public.contacto_proveedor_id_seq;
       public          postgres    false    230            �           0    0    contacto_proveedor_id_seq    SEQUENCE OWNED BY     W   ALTER SEQUENCE public.contacto_proveedor_id_seq OWNED BY public.contacto_proveedor.id;
          public          postgres    false    229            �            1259    27284    detalles_venta    TABLE       CREATE TABLE public.detalles_venta (
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
       public         heap    postgres    false            �            1259    27215    direccion_cliente    TABLE     �   CREATE TABLE public.direccion_cliente (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    cliente uuid NOT NULL,
    direccion uuid NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);
 %   DROP TABLE public.direccion_cliente;
       public         heap    postgres    false            �            1259    27200    direcciones    TABLE     �   CREATE TABLE public.direcciones (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    descripcion text,
    provincia integer NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);
    DROP TABLE public.direcciones;
       public         heap    postgres    false            �            1259    27199    direcciones_provincia_seq    SEQUENCE     �   CREATE SEQUENCE public.direcciones_provincia_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 0   DROP SEQUENCE public.direcciones_provincia_seq;
       public          postgres    false    221            �           0    0    direcciones_provincia_seq    SEQUENCE OWNED BY     W   ALTER SEQUENCE public.direcciones_provincia_seq OWNED BY public.direcciones.provincia;
          public          postgres    false    220            �            1259    27391    historial_productos    TABLE     '  CREATE TABLE public.historial_productos (
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
       public         heap    postgres    false            �            1259    27271    imagenes_productos    TABLE     �   CREATE TABLE public.imagenes_productos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    descripcion text,
    url text NOT NULL,
    producto uuid NOT NULL
);
 &   DROP TABLE public.imagenes_productos;
       public         heap    postgres    false            �            1259    27343    ordenes_compra_proveedor    TABLE     ]  CREATE TABLE public.ordenes_compra_proveedor (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    proveedor uuid NOT NULL,
    id_orden text NOT NULL,
    fecha_estimada_entrega date,
    estado character varying(50) NOT NULL,
    CONSTRAINT ordenes_compra_proveedor_fecha_estimada_entrega_check CHECK ((fecha_estimada_entrega >= CURRENT_DATE))
);
 ,   DROP TABLE public.ordenes_compra_proveedor;
       public         heap    postgres    false            �            1259    27258 	   productos    TABLE     �   CREATE TABLE public.productos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre text NOT NULL,
    marca text NOT NULL,
    descripcion text,
    categoria uuid
);
    DROP TABLE public.productos;
       public         heap    postgres    false            �            1259    27323    productos_proveedores    TABLE     L  CREATE TABLE public.productos_proveedores (
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
       public         heap    postgres    false            �            1259    27296    proveedores    TABLE     �   CREATE TABLE public.proveedores (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(100) NOT NULL,
    direccion uuid,
    correo character varying(255),
    telefono character varying(15)
);
    DROP TABLE public.proveedores;
       public         heap    postgres    false            �            1259    27193 
   provincias    TABLE     h   CREATE TABLE public.provincias (
    id integer NOT NULL,
    nombre character varying(100) NOT NULL
);
    DROP TABLE public.provincias;
       public         heap    postgres    false            �            1259    27192    provincias_id_seq    SEQUENCE     �   CREATE SEQUENCE public.provincias_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 (   DROP SEQUENCE public.provincias_id_seq;
       public          postgres    false    219            �           0    0    provincias_id_seq    SEQUENCE OWNED BY     G   ALTER SEQUENCE public.provincias_id_seq OWNED BY public.provincias.id;
          public          postgres    false    218            �            1259    27181    trabajadores    TABLE     j  CREATE TABLE public.trabajadores (
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
       public         heap    postgres    false            �            1259    27152    usuarios    TABLE     �  CREATE TABLE public.usuarios (
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
       public         heap    postgres    false            �            1259    27232    ventas    TABLE     P  CREATE TABLE public.ventas (
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
       public         heap    postgres    false            �           2604    27315    contacto_proveedor id    DEFAULT     ~   ALTER TABLE ONLY public.contacto_proveedor ALTER COLUMN id SET DEFAULT nextval('public.contacto_proveedor_id_seq'::regclass);
 D   ALTER TABLE public.contacto_proveedor ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    229    230    230            �           2604    27204    direcciones provincia    DEFAULT     ~   ALTER TABLE ONLY public.direcciones ALTER COLUMN provincia SET DEFAULT nextval('public.direcciones_provincia_seq'::regclass);
 D   ALTER TABLE public.direcciones ALTER COLUMN provincia DROP DEFAULT;
       public          postgres    false    221    220    221            �           2604    27196    provincias id    DEFAULT     n   ALTER TABLE ONLY public.provincias ALTER COLUMN id SET DEFAULT nextval('public.provincias_id_seq'::regclass);
 <   ALTER TABLE public.provincias ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    218    219    219            �          0    27245 
   categorias 
   TABLE DATA                 public          postgres    false    224   w}       �          0    27163    clientes 
   TABLE DATA                 public          postgres    false    216   �}       �          0    27312    contacto_proveedor 
   TABLE DATA                 public          postgres    false    230   �}       �          0    27284    detalles_venta 
   TABLE DATA                 public          postgres    false    227   �}       �          0    27215    direccion_cliente 
   TABLE DATA                 public          postgres    false    222   �}       �          0    27200    direcciones 
   TABLE DATA                 public          postgres    false    221   �}       �          0    27391    historial_productos 
   TABLE DATA                 public          postgres    false    233   ~       �          0    27271    imagenes_productos 
   TABLE DATA                 public          postgres    false    226   -~       �          0    27343    ordenes_compra_proveedor 
   TABLE DATA                 public          postgres    false    232   G~       �          0    27258 	   productos 
   TABLE DATA                 public          postgres    false    225   a~       �          0    27323    productos_proveedores 
   TABLE DATA                 public          postgres    false    231   {~       �          0    27296    proveedores 
   TABLE DATA                 public          postgres    false    228   �~       �          0    27193 
   provincias 
   TABLE DATA                 public          postgres    false    219   �~       �          0    27181    trabajadores 
   TABLE DATA                 public          postgres    false    217   �       �          0    27152    usuarios 
   TABLE DATA                 public          postgres    false    215   �       �          0    27232    ventas 
   TABLE DATA                 public          postgres    false    223   �       �           0    0    contacto_proveedor_id_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.contacto_proveedor_id_seq', 1, false);
          public          postgres    false    229            �           0    0    direcciones_provincia_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.direcciones_provincia_seq', 1, false);
          public          postgres    false    220            �           0    0    provincias_id_seq    SEQUENCE SET     @   SELECT pg_catalog.setval('public.provincias_id_seq', 17, true);
          public          postgres    false    218            �           2606    27252    categorias categorias_pkey 
   CONSTRAINT     X   ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT categorias_pkey PRIMARY KEY (id);
 D   ALTER TABLE ONLY public.categorias DROP CONSTRAINT categorias_pkey;
       public            postgres    false    224            �           2606    27171    clientes clientes_correo_key 
   CONSTRAINT     Y   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_correo_key UNIQUE (correo);
 F   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_correo_key;
       public            postgres    false    216            �           2606    27169    clientes clientes_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_pkey PRIMARY KEY (id);
 @   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_pkey;
       public            postgres    false    216            �           2606    27173    clientes clientes_telefono_key 
   CONSTRAINT     ]   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_telefono_key UNIQUE (telefono);
 H   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_telefono_key;
       public            postgres    false    216            �           2606    27175    clientes clientes_usuario_key 
   CONSTRAINT     [   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_usuario_key UNIQUE (usuario);
 G   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_usuario_key;
       public            postgres    false    216            �           2606    27317 *   contacto_proveedor contacto_proveedor_pkey 
   CONSTRAINT     h   ALTER TABLE ONLY public.contacto_proveedor
    ADD CONSTRAINT contacto_proveedor_pkey PRIMARY KEY (id);
 T   ALTER TABLE ONLY public.contacto_proveedor DROP CONSTRAINT contacto_proveedor_pkey;
       public            postgres    false    230            �           2606    27295 "   detalles_venta detalles_venta_pkey 
   CONSTRAINT     `   ALTER TABLE ONLY public.detalles_venta
    ADD CONSTRAINT detalles_venta_pkey PRIMARY KEY (id);
 L   ALTER TABLE ONLY public.detalles_venta DROP CONSTRAINT detalles_venta_pkey;
       public            postgres    false    227            �           2606    27221 (   direccion_cliente direccion_cliente_pkey 
   CONSTRAINT     f   ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_pkey PRIMARY KEY (id);
 R   ALTER TABLE ONLY public.direccion_cliente DROP CONSTRAINT direccion_cliente_pkey;
       public            postgres    false    222            �           2606    27209    direcciones direcciones_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.direcciones
    ADD CONSTRAINT direcciones_pkey PRIMARY KEY (id);
 F   ALTER TABLE ONLY public.direcciones DROP CONSTRAINT direcciones_pkey;
       public            postgres    false    221            �           2606    27399 ,   historial_productos historial_productos_pkey 
   CONSTRAINT     j   ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_pkey PRIMARY KEY (id);
 V   ALTER TABLE ONLY public.historial_productos DROP CONSTRAINT historial_productos_pkey;
       public            postgres    false    233            �           2606    27278 *   imagenes_productos imagenes_productos_pkey 
   CONSTRAINT     h   ALTER TABLE ONLY public.imagenes_productos
    ADD CONSTRAINT imagenes_productos_pkey PRIMARY KEY (id);
 T   ALTER TABLE ONLY public.imagenes_productos DROP CONSTRAINT imagenes_productos_pkey;
       public            postgres    false    226            �           2606    27351 6   ordenes_compra_proveedor ordenes_compra_proveedor_pkey 
   CONSTRAINT     t   ALTER TABLE ONLY public.ordenes_compra_proveedor
    ADD CONSTRAINT ordenes_compra_proveedor_pkey PRIMARY KEY (id);
 `   ALTER TABLE ONLY public.ordenes_compra_proveedor DROP CONSTRAINT ordenes_compra_proveedor_pkey;
       public            postgres    false    232            �           2606    27265    productos productos_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.productos
    ADD CONSTRAINT productos_pkey PRIMARY KEY (id);
 B   ALTER TABLE ONLY public.productos DROP CONSTRAINT productos_pkey;
       public            postgres    false    225            �           2606    27332 0   productos_proveedores productos_proveedores_pkey 
   CONSTRAINT     n   ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_pkey PRIMARY KEY (id);
 Z   ALTER TABLE ONLY public.productos_proveedores DROP CONSTRAINT productos_proveedores_pkey;
       public            postgres    false    231            �           2606    27303 "   proveedores proveedores_correo_key 
   CONSTRAINT     _   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_correo_key UNIQUE (correo);
 L   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_correo_key;
       public            postgres    false    228            �           2606    27301    proveedores proveedores_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_pkey PRIMARY KEY (id);
 F   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_pkey;
       public            postgres    false    228            �           2606    27305 $   proveedores proveedores_telefono_key 
   CONSTRAINT     c   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_telefono_key UNIQUE (telefono);
 N   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_telefono_key;
       public            postgres    false    228            �           2606    27198    provincias provincias_pkey 
   CONSTRAINT     X   ALTER TABLE ONLY public.provincias
    ADD CONSTRAINT provincias_pkey PRIMARY KEY (id);
 D   ALTER TABLE ONLY public.provincias DROP CONSTRAINT provincias_pkey;
       public            postgres    false    219            �           2606    27191    trabajadores trabajadores_pkey 
   CONSTRAINT     \   ALTER TABLE ONLY public.trabajadores
    ADD CONSTRAINT trabajadores_pkey PRIMARY KEY (id);
 H   ALTER TABLE ONLY public.trabajadores DROP CONSTRAINT trabajadores_pkey;
       public            postgres    false    217            �           2606    27160    usuarios usuarios_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_pkey PRIMARY KEY (id);
 @   ALTER TABLE ONLY public.usuarios DROP CONSTRAINT usuarios_pkey;
       public            postgres    false    215            �           2606    27162    usuarios usuarios_username_key 
   CONSTRAINT     ]   ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_username_key UNIQUE (username);
 H   ALTER TABLE ONLY public.usuarios DROP CONSTRAINT usuarios_username_key;
       public            postgres    false    215            �           2606    27244    ventas ventas_pkey 
   CONSTRAINT     P   ALTER TABLE ONLY public.ventas
    ADD CONSTRAINT ventas_pkey PRIMARY KEY (id);
 <   ALTER TABLE ONLY public.ventas DROP CONSTRAINT ventas_pkey;
       public            postgres    false    223            �           2620    27375 '   clientes clientes_actualizar_updated_at    TRIGGER     �   CREATE TRIGGER clientes_actualizar_updated_at BEFORE UPDATE ON public.clientes FOR EACH ROW EXECUTE FUNCTION public.actualizar_timestamp();
 @   DROP TRIGGER clientes_actualizar_updated_at ON public.clientes;
       public          postgres    false    235    216            �           2620    27372 1   productos_proveedores tr_guardar_historial_precio    TRIGGER     �   CREATE TRIGGER tr_guardar_historial_precio BEFORE UPDATE OF total ON public.productos_proveedores FOR EACH ROW WHEN ((old.total IS DISTINCT FROM new.total)) EXECUTE FUNCTION public.guardar_historial_precio();
 J   DROP TRIGGER tr_guardar_historial_precio ON public.productos_proveedores;
       public          postgres    false    234    231    231    231            �           2620    27376 /   trabajadores trabajadores_actualizar_updated_at    TRIGGER     �   CREATE TRIGGER trabajadores_actualizar_updated_at BEFORE UPDATE ON public.trabajadores FOR EACH ROW EXECUTE FUNCTION public.actualizar_timestamp();
 H   DROP TRIGGER trabajadores_actualizar_updated_at ON public.trabajadores;
       public          postgres    false    217    235            �           2620    27374 '   usuarios usuarios_actualizar_updated_at    TRIGGER     �   CREATE TRIGGER usuarios_actualizar_updated_at BEFORE UPDATE ON public.usuarios FOR EACH ROW EXECUTE FUNCTION public.actualizar_timestamp();
 @   DROP TRIGGER usuarios_actualizar_updated_at ON public.usuarios;
       public          postgres    false    215    235            �           2606    27253 *   categorias categorias_categoria_padre_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT categorias_categoria_padre_fkey FOREIGN KEY (categoria_padre) REFERENCES public.categorias(id) ON UPDATE CASCADE ON DELETE SET NULL;
 T   ALTER TABLE ONLY public.categorias DROP CONSTRAINT categorias_categoria_padre_fkey;
       public          postgres    false    224    224    4822            �           2606    27176    clientes clientes_usuario_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.clientes
    ADD CONSTRAINT clientes_usuario_fkey FOREIGN KEY (usuario) REFERENCES public.usuarios(id) ON UPDATE CASCADE ON DELETE SET NULL;
 H   ALTER TABLE ONLY public.clientes DROP CONSTRAINT clientes_usuario_fkey;
       public          postgres    false    4800    216    215            �           2606    27318 4   contacto_proveedor contacto_proveedor_proveedor_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.contacto_proveedor
    ADD CONSTRAINT contacto_proveedor_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;
 ^   ALTER TABLE ONLY public.contacto_proveedor DROP CONSTRAINT contacto_proveedor_proveedor_fkey;
       public          postgres    false    230    4832    228            �           2606    27222 0   direccion_cliente direccion_cliente_cliente_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_cliente_fkey FOREIGN KEY (cliente) REFERENCES public.clientes(id) ON UPDATE CASCADE ON DELETE CASCADE;
 Z   ALTER TABLE ONLY public.direccion_cliente DROP CONSTRAINT direccion_cliente_cliente_fkey;
       public          postgres    false    216    222    4806            �           2606    27227 2   direccion_cliente direccion_cliente_direccion_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.direccion_cliente
    ADD CONSTRAINT direccion_cliente_direccion_fkey FOREIGN KEY (direccion) REFERENCES public.direcciones(id) ON UPDATE CASCADE ON DELETE CASCADE;
 \   ALTER TABLE ONLY public.direccion_cliente DROP CONSTRAINT direccion_cliente_direccion_fkey;
       public          postgres    false    4816    222    221            �           2606    27210 &   direcciones direcciones_provincia_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.direcciones
    ADD CONSTRAINT direcciones_provincia_fkey FOREIGN KEY (provincia) REFERENCES public.provincias(id) ON UPDATE CASCADE;
 P   ALTER TABLE ONLY public.direcciones DROP CONSTRAINT direcciones_provincia_fkey;
       public          postgres    false    221    4814    219            �           2606    27400 5   historial_productos historial_productos_producto_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE ON DELETE CASCADE;
 _   ALTER TABLE ONLY public.historial_productos DROP CONSTRAINT historial_productos_producto_fkey;
       public          postgres    false    233    4824    225            �           2606    27405 6   historial_productos historial_productos_proveedor_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.historial_productos
    ADD CONSTRAINT historial_productos_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;
 `   ALTER TABLE ONLY public.historial_productos DROP CONSTRAINT historial_productos_proveedor_fkey;
       public          postgres    false    4832    228    233            �           2606    27279 3   imagenes_productos imagenes_productos_producto_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.imagenes_productos
    ADD CONSTRAINT imagenes_productos_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE ON DELETE SET NULL;
 ]   ALTER TABLE ONLY public.imagenes_productos DROP CONSTRAINT imagenes_productos_producto_fkey;
       public          postgres    false    226    225    4824            �           2606    27352 @   ordenes_compra_proveedor ordenes_compra_proveedor_proveedor_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.ordenes_compra_proveedor
    ADD CONSTRAINT ordenes_compra_proveedor_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE SET NULL;
 j   ALTER TABLE ONLY public.ordenes_compra_proveedor DROP CONSTRAINT ordenes_compra_proveedor_proveedor_fkey;
       public          postgres    false    228    232    4832            �           2606    27266 "   productos productos_categoria_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.productos
    ADD CONSTRAINT productos_categoria_fkey FOREIGN KEY (categoria) REFERENCES public.categorias(id) ON UPDATE CASCADE ON DELETE SET NULL;
 L   ALTER TABLE ONLY public.productos DROP CONSTRAINT productos_categoria_fkey;
       public          postgres    false    224    4822    225            �           2606    27333 9   productos_proveedores productos_proveedores_producto_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_producto_fkey FOREIGN KEY (producto) REFERENCES public.productos(id) ON UPDATE CASCADE ON DELETE CASCADE;
 c   ALTER TABLE ONLY public.productos_proveedores DROP CONSTRAINT productos_proveedores_producto_fkey;
       public          postgres    false    4824    231    225            �           2606    27338 :   productos_proveedores productos_proveedores_proveedor_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.productos_proveedores
    ADD CONSTRAINT productos_proveedores_proveedor_fkey FOREIGN KEY (proveedor) REFERENCES public.proveedores(id) ON UPDATE CASCADE ON DELETE CASCADE;
 d   ALTER TABLE ONLY public.productos_proveedores DROP CONSTRAINT productos_proveedores_proveedor_fkey;
       public          postgres    false    4832    228    231            �           2606    27306 &   proveedores proveedores_direccion_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.proveedores
    ADD CONSTRAINT proveedores_direccion_fkey FOREIGN KEY (direccion) REFERENCES public.direcciones(id) ON UPDATE CASCADE ON DELETE SET NULL;
 P   ALTER TABLE ONLY public.proveedores DROP CONSTRAINT proveedores_direccion_fkey;
       public          postgres    false    4816    221    228            �   
   x���          �   
   x���          �   
   x���          �   
   x���          �   
   x���          �   
   x���          �   
   x���          �   
   x���          �   
   x���          �   
   x���          �   
   x���          �   
   x���          �     x���AK�0�{?ŻM��qn*��+Z��غ��5#K�k#�:�]�x�#䋙*��������'I��1�$�g�6�����P����ӧ8�v��.�I�%�t��*J�쩳7��v�o�F�O�o�:�"H���φ�M����9tǉ8�?:wh��P���w�=����K���z���U�ı[��Q<�u�W'��/��i��"imw�M�6�Kf�iy��*�b�!_�w����ߤ*�X�U�;6�7`��:ś�P-����/��6      �   
   x���          �   
   x���          �   
   x���         