PGDMP      4            	    |            farmarey    16.4    16.4 "    $           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            %           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            &           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            '           1262    16893    farmarey    DATABASE     |   CREATE DATABASE farmarey WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Spanish_Panama.1252';
    DROP DATABASE farmarey;
                postgres    false            �            1255    17035 "   fn_farmacia_abastecer_inventario()    FUNCTION     �   CREATE FUNCTION public.fn_farmacia_abastecer_inventario() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
	UPDATE medicamentos 
	SET cantidad_disponible = cantidad_disponible + NEW.cantidad
	WHERE id = NEW.medicamento;

	RETURN NEW;
END;
$$;
 9   DROP FUNCTION public.fn_farmacia_abastecer_inventario();
       public          postgres    false            �            1255    17037 &   fn_farmacia_actualizar_detalle_venta()    FUNCTION     �  CREATE FUNCTION public.fn_farmacia_actualizar_detalle_venta() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 
	precio MONEY;
BEGIN
	-- Verificar stock disponible
	IF (SELECT cantidad_disponible FROM medicamentos WHERE id = NEW.medicamento) < NEW.cantidad THEN
		RAISE EXCEPTION 'No hay suficiente cantidad disponible para el medicamento con ID: %', NEW.medicamento;
	END IF;
	
	UPDATE medicamentos
	SET cantidad_disponible = cantidad_disponible - NEW.cantidad
	WHERE id = NEW.medicamento;
	-- Calcular subtotal
	SELECT precio_unidad INTO precio
	FROM medicamentos m
	WHERE m.id = NEW.medicamento;

	NEW.sub_total := precio * NEW.cantidad;
	
	RETURN NEW;
END;
$$;
 =   DROP FUNCTION public.fn_farmacia_actualizar_detalle_venta();
       public          postgres    false            �            1255    17039    fn_farmacia_calcular_total()    FUNCTION     s  CREATE FUNCTION public.fn_farmacia_calcular_total() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
	-- Se suma las subtotales de los detalles de cada venta, si aún no hay subtotal es 0.00
	UPDATE ventas
	SET total = (
		SELECT COALESCE(SUM(sub_total), '0.00'::MONEY)
		FROM detalles_venta
		WHERE venta = NEW.venta
	)
	WHERE id = NEW.venta;

	RETURN NEW;
END;
$$;
 3   DROP FUNCTION public.fn_farmacia_calcular_total();
       public          postgres    false            �            1255    17041 !   fn_farmacia_medicamentos_update()    FUNCTION     �   CREATE FUNCTION public.fn_farmacia_medicamentos_update() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
	NEW.updated_at := CURRENT_TIMESTAMP;
	RETURN NEW;
END;
$$;
 8   DROP FUNCTION public.fn_farmacia_medicamentos_update();
       public          postgres    false            �            1259    17022    abastecer_inventarios    TABLE     +  CREATE TABLE public.abastecer_inventarios (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    medicamento uuid NOT NULL,
    cantidad integer NOT NULL,
    created_at timestamp without time zone DEFAULT now() NOT NULL,
    CONSTRAINT abastecer_inventarios_cantidad_check CHECK ((cantidad > 0))
);
 )   DROP TABLE public.abastecer_inventarios;
       public         heap    postgres    false            �            1259    16973 
   categorias    TABLE        CREATE TABLE public.categorias (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(100) NOT NULL
);
    DROP TABLE public.categorias;
       public         heap    postgres    false            �            1259    17004    detalles_venta    TABLE     e  CREATE TABLE public.detalles_venta (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    venta uuid NOT NULL,
    medicamento uuid NOT NULL,
    cantidad integer NOT NULL,
    sub_total money,
    CONSTRAINT detalles_venta_cantidad_check CHECK ((cantidad > 0)),
    CONSTRAINT detalles_venta_cantidad_check1 CHECK (((cantidad)::numeric >= (0)::numeric))
);
 "   DROP TABLE public.detalles_venta;
       public         heap    postgres    false            �            1259    16979    medicamentos    TABLE     -  CREATE TABLE public.medicamentos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre character varying(150) NOT NULL,
    cantidad_disponible integer DEFAULT 0 NOT NULL,
    precio_unidad money NOT NULL,
    imagen_url text,
    categoria uuid,
    created_at timestamp without time zone DEFAULT now() NOT NULL,
    updated_at timestamp without time zone,
    CONSTRAINT medicamentos_cantidad_disponible_check CHECK ((cantidad_disponible >= 0)),
    CONSTRAINT medicamentos_precio_unidad_check CHECK (((precio_unidad)::numeric >= (0)::numeric))
);
     DROP TABLE public.medicamentos;
       public         heap    postgres    false            �            1259    16996    ventas    TABLE     �   CREATE TABLE public.ventas (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    fecha timestamp without time zone DEFAULT now(),
    total money,
    CONSTRAINT ventas_total_check CHECK (((total)::numeric >= (0)::numeric))
);
    DROP TABLE public.ventas;
       public         heap    postgres    false            �            1259    17043    view_medicamentos_categoria    VIEW     @  CREATE VIEW public.view_medicamentos_categoria AS
 SELECT m.nombre AS medicamento,
    m.cantidad_disponible AS "cantidad disponible",
    m.precio_unidad AS "precio por unidad",
    m.imagen_url,
    c.nombre AS "categoría"
   FROM (public.medicamentos m
     LEFT JOIN public.categorias c ON ((m.categoria = c.id)));
 .   DROP VIEW public.view_medicamentos_categoria;
       public          postgres    false    216    216    216    216    216    215    215            �            1259    17047    view_ventas_detalles    VIEW       CREATE VIEW public.view_ventas_detalles AS
SELECT
    NULL::timestamp without time zone AS fecha,
    NULL::character varying(150) AS medicamento,
    NULL::money AS "precio por unidad",
    NULL::integer AS cantidad,
    NULL::money AS subtotal,
    NULL::money AS total;
 '   DROP VIEW public.view_ventas_detalles;
       public          postgres    false            !          0    17022    abastecer_inventarios 
   TABLE DATA           V   COPY public.abastecer_inventarios (id, medicamento, cantidad, created_at) FROM stdin;
    public          postgres    false    219   6                 0    16973 
   categorias 
   TABLE DATA           0   COPY public.categorias (id, nombre) FROM stdin;
    public          postgres    false    215   %6                  0    17004    detalles_venta 
   TABLE DATA           U   COPY public.detalles_venta (id, venta, medicamento, cantidad, sub_total) FROM stdin;
    public          postgres    false    218   7                 0    16979    medicamentos 
   TABLE DATA           �   COPY public.medicamentos (id, nombre, cantidad_disponible, precio_unidad, imagen_url, categoria, created_at, updated_at) FROM stdin;
    public          postgres    false    216   *7                 0    16996    ventas 
   TABLE DATA           2   COPY public.ventas (id, fecha, total) FROM stdin;
    public          postgres    false    217   \9       �           2606    17029 0   abastecer_inventarios abastecer_inventarios_pkey 
   CONSTRAINT     n   ALTER TABLE ONLY public.abastecer_inventarios
    ADD CONSTRAINT abastecer_inventarios_pkey PRIMARY KEY (id);
 Z   ALTER TABLE ONLY public.abastecer_inventarios DROP CONSTRAINT abastecer_inventarios_pkey;
       public            postgres    false    219            {           2606    16978    categorias categorias_pkey 
   CONSTRAINT     X   ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT categorias_pkey PRIMARY KEY (id);
 D   ALTER TABLE ONLY public.categorias DROP CONSTRAINT categorias_pkey;
       public            postgres    false    215            �           2606    17011 "   detalles_venta detalles_venta_pkey 
   CONSTRAINT     `   ALTER TABLE ONLY public.detalles_venta
    ADD CONSTRAINT detalles_venta_pkey PRIMARY KEY (id);
 L   ALTER TABLE ONLY public.detalles_venta DROP CONSTRAINT detalles_venta_pkey;
       public            postgres    false    218            }           2606    16990    medicamentos medicamentos_pkey 
   CONSTRAINT     \   ALTER TABLE ONLY public.medicamentos
    ADD CONSTRAINT medicamentos_pkey PRIMARY KEY (id);
 H   ALTER TABLE ONLY public.medicamentos DROP CONSTRAINT medicamentos_pkey;
       public            postgres    false    216                       2606    17003    ventas ventas_pkey 
   CONSTRAINT     P   ALTER TABLE ONLY public.ventas
    ADD CONSTRAINT ventas_pkey PRIMARY KEY (id);
 <   ALTER TABLE ONLY public.ventas DROP CONSTRAINT ventas_pkey;
       public            postgres    false    217                       2618    17050    view_ventas_detalles _RETURN    RULE     �  CREATE OR REPLACE VIEW public.view_ventas_detalles AS
 SELECT v.fecha,
    m.nombre AS medicamento,
    m.precio_unidad AS "precio por unidad",
    d.cantidad,
    d.sub_total AS subtotal,
    v.total
   FROM ((public.ventas v
     JOIN public.detalles_venta d ON ((v.id = d.venta)))
     JOIN public.medicamentos m ON ((d.medicamento = m.id)))
  GROUP BY v.id, v.total, d.medicamento, d.cantidad, d.sub_total, m.nombre, m.precio_unidad
  ORDER BY v.fecha DESC;
   CREATE OR REPLACE VIEW public.view_ventas_detalles AS
SELECT
    NULL::timestamp without time zone AS fecha,
    NULL::character varying(150) AS medicamento,
    NULL::money AS "precio por unidad",
    NULL::integer AS cantidad,
    NULL::money AS subtotal,
    NULL::money AS total;
       public          postgres    false    216    218    218    4735    218    217    217    217    216    216    218    221            �           2620    17036 -   abastecer_inventarios tr_abastecer_inventario    TRIGGER     �   CREATE TRIGGER tr_abastecer_inventario AFTER INSERT ON public.abastecer_inventarios FOR EACH ROW EXECUTE FUNCTION public.fn_farmacia_abastecer_inventario();
 F   DROP TRIGGER tr_abastecer_inventario ON public.abastecer_inventarios;
       public          postgres    false    219    222            �           2620    17038 *   detalles_venta tr_actualizar_detalle_venta    TRIGGER     �   CREATE TRIGGER tr_actualizar_detalle_venta BEFORE INSERT ON public.detalles_venta FOR EACH ROW EXECUTE FUNCTION public.fn_farmacia_actualizar_detalle_venta();
 C   DROP TRIGGER tr_actualizar_detalle_venta ON public.detalles_venta;
       public          postgres    false    218    223            �           2620    17040     detalles_venta tr_calcular_total    TRIGGER     �   CREATE TRIGGER tr_calcular_total AFTER INSERT ON public.detalles_venta FOR EACH ROW EXECUTE FUNCTION public.fn_farmacia_calcular_total();
 9   DROP TRIGGER tr_calcular_total ON public.detalles_venta;
       public          postgres    false    224    218            �           2620    17042 &   medicamentos tr_medicament_actualizado    TRIGGER     �   CREATE TRIGGER tr_medicament_actualizado BEFORE UPDATE ON public.medicamentos FOR EACH ROW EXECUTE FUNCTION public.fn_farmacia_medicamentos_update();
 ?   DROP TRIGGER tr_medicament_actualizado ON public.medicamentos;
       public          postgres    false    225    216            �           2606    17030 <   abastecer_inventarios abastecer_inventarios_medicamento_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.abastecer_inventarios
    ADD CONSTRAINT abastecer_inventarios_medicamento_fkey FOREIGN KEY (medicamento) REFERENCES public.medicamentos(id) ON DELETE CASCADE;
 f   ALTER TABLE ONLY public.abastecer_inventarios DROP CONSTRAINT abastecer_inventarios_medicamento_fkey;
       public          postgres    false    216    219    4733            �           2606    17017 .   detalles_venta detalles_venta_medicamento_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.detalles_venta
    ADD CONSTRAINT detalles_venta_medicamento_fkey FOREIGN KEY (medicamento) REFERENCES public.medicamentos(id) ON UPDATE CASCADE ON DELETE RESTRICT;
 X   ALTER TABLE ONLY public.detalles_venta DROP CONSTRAINT detalles_venta_medicamento_fkey;
       public          postgres    false    216    218    4733            �           2606    17012 (   detalles_venta detalles_venta_venta_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.detalles_venta
    ADD CONSTRAINT detalles_venta_venta_fkey FOREIGN KEY (venta) REFERENCES public.ventas(id) ON UPDATE CASCADE ON DELETE RESTRICT;
 R   ALTER TABLE ONLY public.detalles_venta DROP CONSTRAINT detalles_venta_venta_fkey;
       public          postgres    false    4735    218    217            �           2606    16991 (   medicamentos medicamentos_categoria_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.medicamentos
    ADD CONSTRAINT medicamentos_categoria_fkey FOREIGN KEY (categoria) REFERENCES public.categorias(id) ON UPDATE CASCADE ON DELETE SET NULL;
 R   ALTER TABLE ONLY public.medicamentos DROP CONSTRAINT medicamentos_categoria_fkey;
       public          postgres    false    216    215    4731            !      x������ � �         �   x�%Ͻm�0���<E``I�_y;�LC��E��XZ*E����"8<x�/� �@b��b�R@	�k	o����󻷲��;J)F#��6Y0XMAv�O1Z��^4��M�9�����\�f(B0l�XJ���w\%�I��y��2�G�jt�TlR��W�^��v�9ۨ�
��h� g��E�[+����q�$M�����q�m�G�_�/��˲��lW�             x������ � �         "  x����j1����̮�r�ICqq␸Ћ܌�Q��.^�>}e�6)��\I�a�|s�|.����A���s"�8�!�%[nh�Ꮱg�sv�6���}����m��K��M{!�=m������Vr)[!�?-@��A4����L��E&ВB@1e�x��FSg�Z�A��^~!T��*���'��-\�5�HY�+�*�^���0�7A�F0���4M����
Ѿi�|��s6g�2���|�D�	�չ��d�>��SR%�ȕ�����R��2�1�E���y���3' ��wz��4�'�B�� 2�	mu�K�FIG�`"�&Z>8Q� =Be�֑��6��:����:ɗo�8�'���0�'ڕuOG���w7˯��b~ہ���\u�;�[��g��=|Ytp������au}�C�6�X�R��՟�u�{��$zm�&�1G[�jq�����Br%G��d�a�{�G@g~���\,�U���'�«�V���z���<y%�"r�3bT�PKYz:��f6��*��            x������ � �     