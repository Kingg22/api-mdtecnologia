-- Datos mocks (falsos) de pruebas 
INSERT INTO public.categorias(id, nombre, descripcion, categoria_padre, imagen_url)
VALUES
('1f6c6de1-1a2b-4c3d-a6b8-9f7e1b6d4c1a', 'Artículos de Oficina', 'Materiales y equipos para oficina', NULL, 'https://jadetec.net/site/assets/files/1055/utiles-de-oficina.jpg'),
('2a7d8fe2-2b3c-5d4e-b7c9-0a8f2c7e5d2b', 'Componentes', 'Partes para equipos tecnológicos', NULL, 'https://geekelectronica.com/wp-content/uploads/2020/09/Componentes.jpg'),
('47806a2e-fb2d-4449-b357-89202ce4b3bb', 'Accesorios', 'Complementos para dispositivos tecnológicos', NULL, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQaDsb_5DGdr7B-iOM_f4pOOYrDyI_euSTheA&s'),
('3af5adef-0d53-4f49-9eda-48e667162ebe', 'Monitores', 'Pantallas de diversos tamaños y resoluciones', NULL, 'https://www.hergotec.com/udecontrol_datos/objetos/48.jpg'),
('0a88ebb9-c879-486b-bb12-bfc09153af5a', 'Equipos', 'Equipos completos como computadoras y laptops', NULL, 'https://www.claroshop.com/c/algolia/assets/portada/pc.webp'),
('1b85bd80-f6ac-4438-a93b-127e966f80a8', 'Impresoras', 'Equipos de impresión y multifuncionales', NULL, 'https://quecartucho.es/blog/wp-content/uploads/2018/02/mejores-impresoras.png'),
('1881b904-8626-43cc-b5ac-e84d0af7a8fb', 'Redes y Conectividad', 'Equipos de comunicación y redes', NULL, 'https://incuatro.com/wp-content/uploads/2019/06/Redes-informaticas.jpg'),
('f21714f3-ac33-4834-8927-a32b79535ce4', 'Almacenamiento', 'Dispositivos para guardar datos', NULL, 'https://s3.amazonaws.com/s3.timetoast.com/public/uploads/photo/18058726/image/medium-667232431702a487c11eb426a06d2cd1.jpg?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAJB6ZCNNAN7BE7WDQ%2F20241117%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20241117T035158Z&X-Amz-Expires=604800&X-Amz-SignedHeaders=host&X-Amz-Signature=b37c11c1873b6a412281737902a1b4676dc3814dcdfdac7ce92d800589567e6c'),
('87e561ea-1ea0-49c2-9e94-dc1472a40cef', 'Software', 'Programas y licencias digitales', NULL, 'https://redtiseg.com/wp-content/uploads/2023/08/Licencia-de-software.jpg'),
('20d134d1-27bf-49e9-b9aa-812179cdc92e', 'Gaming', 'Equipos y accesorios para videojuegos', NULL, 'https://www.ikea.com/ext/ingkadam/m/3cf80d93ffee0337/original/PH187075.jpg?f=s');

INSERT INTO public.productos VALUES
	('c9279638-902a-4bac-a9f0-7f284b20c9ce', 'Resma de Papel A4', 'HP', 'Papel bond tamaño A4 de alta calidad', '1f6c6de1-1a2b-4c3d-a6b8-9f7e1b6d4c1a'),
	('6519389d-ce45-4fed-97ba-823518f20b61', 'Grapadora Metálica', 'Swingline', 'Grapadora ergonómica con capacidad para 20 hojas', '1f6c6de1-1a2b-4c3d-a6b8-9f7e1b6d4c1a'),
	('528bc2c7-6a72-47c8-840e-3a4d092a6b47', 'Caja de Lápices', 'Faber-Castell', 'Lápices de grafito HB resistentes y duraderos', '1f6c6de1-1a2b-4c3d-a6b8-9f7e1b6d4c1a'),
	('7c73cb17-1d97-4361-a0ae-c10c7ef4f72f', 'Tijeras de Oficina', 'Scotch', 'Tijeras de acero inoxidable de 8 pulgadas', '1f6c6de1-1a2b-4c3d-a6b8-9f7e1b6d4c1a'),
	('801a6449-b3a4-4184-bd3b-a5f1947444a8', 'Calculadora Científica', 'Casio', 'Calculadora avanzada con funciones científicas', '1f6c6de1-1a2b-4c3d-a6b8-9f7e1b6d4c1a'),
	('0430a2c4-6c48-480e-9d99-9afd632f2b0a', 'Procesador Intel Core i9', 'Intel', 'Procesador de última generación con 12 núcleos', '2a7d8fe2-2b3c-5d4e-b7c9-0a8f2c7e5d2b'),
	('31c02657-3da7-44a9-b520-8e2fa2ee0b93', 'Memoria RAM 16GB DDR4', 'Corsair', 'Módulo de memoria de alto rendimiento DDR4', '2a7d8fe2-2b3c-5d4e-b7c9-0a8f2c7e5d2b'),
	('a0cce63d-e945-4d37-86f5-7db672be99b5', 'Disco Duro SSD 1TB', 'Samsung', 'Unidad de almacenamiento sólido con tecnología NVMe', '2a7d8fe2-2b3c-5d4e-b7c9-0a8f2c7e5d2b'),
	('f811419a-79b7-488f-aec4-0e366ada9ccd', 'Tarjeta Gráfica RTX 4070', 'NVIDIA', 'Tarjeta gráfica con soporte para trazado de rayos', '2a7d8fe2-2b3c-5d4e-b7c9-0a8f2c7e5d2b'),
	('19550f5d-451a-4ac9-9a52-b0ba344f9dd2', 'Fuente de Poder 750W', 'EVGA', 'Fuente certificada 80 Plus Gold', '2a7d8fe2-2b3c-5d4e-b7c9-0a8f2c7e5d2b'),
	('654c61b0-4e2e-4617-9318-3b380eb8bd5e', 'Teclado Mecánico RGB', 'Logitech', 'Teclado con iluminación RGB personalizable', '47806a2e-fb2d-4449-b357-89202ce4b3bb'),
	('75367873-38ac-4ebb-9070-8df571ef0c07', 'Mouse Gaming', 'Razer', 'Mouse con sensor óptico de alta precisión', '47806a2e-fb2d-4449-b357-89202ce4b3bb'),
	('cd751b1f-8def-4514-b11a-3ae6dd050b81', 'Auriculares Bluetooth', 'Sony', 'Auriculares inalámbricos con cancelación de ruido', '47806a2e-fb2d-4449-b357-89202ce4b3bb'),
	('ab9d613a-aba3-4c88-b1ca-7d27b8b793e2', 'Cámara Web Full HD', 'Logitech', 'Cámara con resolución 1080p para videollamadas', '47806a2e-fb2d-4449-b357-89202ce4b3bb'),
	('86448b06-cc2e-4216-acd2-a5e4869d90b5', 'Base de Enfriamiento', 'Cooler Master', 'Soporte con ventiladores para laptops', '47806a2e-fb2d-4449-b357-89202ce4b3bb'),
	('e613e3af-00b8-4c43-8bf0-d1bd31a0ed25', 'Monitor 24 pulgadas', 'Dell', 'Monitor Full HD con panel IPS', '3af5adef-0d53-4f49-9eda-48e667162ebe'),
	('d38b40aa-fb81-4f02-bde4-6f33926cd568', 'Monitor 32 pulgadas 4K', 'LG', 'Monitor UHD con soporte HDR', '3af5adef-0d53-4f49-9eda-48e667162ebe'),
	('57247ea2-cbb5-445b-8832-636aeecafe31', 'Monitor Curvo 27 pulgadas', 'Samsung', 'Pantalla curva con tecnología QHD', '3af5adef-0d53-4f49-9eda-48e667162ebe'),
	('def8c2bf-1576-4ac8-a11e-a9b45c6c5757', 'Monitor Portátil', 'ASUS', 'Monitor ligero con conectividad USB-C', '3af5adef-0d53-4f49-9eda-48e667162ebe'),
	('317bda56-0cc6-4d22-bd89-91d39719a65d', 'Monitor para Gaming', 'BenQ', 'Monitor con frecuencia de actualización de 240 Hz', '3af5adef-0d53-4f49-9eda-48e667162ebe'),
	('ffab82ef-4512-4f82-a4f8-1e584da96583', 'Laptop Ultrabook', 'HP', 'Laptop ligera con procesador Intel i7 y SSD de 512GB', '0a88ebb9-c879-486b-bb12-bfc09153af5a'),
	('9f69ed52-bf8e-4372-81c0-e7899130e2a8', 'PC Gamer', 'Alienware', 'Computadora de escritorio con RTX 4080', '0a88ebb9-c879-486b-bb12-bfc09153af5a'),
	('b8af10e2-3447-4649-8623-cf453c294e4d', 'Tablet 10 pulgadas', 'Apple', 'Tablet con pantalla Retina y chip M1', '0a88ebb9-c879-486b-bb12-bfc09153af5a'),
	('43eaa734-d64e-4518-9677-d48854586b1a', 'Chromebook', 'Acer', 'Laptop compacta ideal para estudios', '0a88ebb9-c879-486b-bb12-bfc09153af5a'),
	('1c2a5627-a830-424b-ac2a-3a3223065993', 'Mini PC', 'Intel', 'PC compacta para tareas básicas', '0a88ebb9-c879-486b-bb12-bfc09153af5a'),
	('79f3afe5-f886-4427-a0e0-e29e34f2f999', 'Impresora Láser', 'Brother', 'Impresora monocromática rápida y eficiente', '1b85bd80-f6ac-4438-a93b-127e966f80a8'),
	('f7893c0b-b40d-46eb-a01e-b7d0af84450b', 'Impresora Multifuncional', 'Epson', 'Impresora con escáner y copiadora', '1b85bd80-f6ac-4438-a93b-127e966f80a8'),
	('6a156401-ca31-4fea-a414-6465f2a37a39', 'Plotter de Gran Formato', 'HP', 'Impresora especializada para gráficos y planos', '1b85bd80-f6ac-4438-a93b-127e966f80a8'),
	('7f0f5046-2b5e-47f5-8c71-672bcf4aa6bd', 'Impresora 3D', 'Creality', 'Impresora 3D con tecnología FDM', '1b85bd80-f6ac-4438-a93b-127e966f80a8'),
	('b70a811a-1812-4bbf-a17d-70c1a9cce78c', 'Escáner de Documentos', 'Canon', 'Escáner portátil de alta resolución', '1b85bd80-f6ac-4438-a93b-127e966f80a8'),
	('2f1d3310-d301-411b-8790-384984f30cb6', 'Router WiFi 6', 'TP-Link', 'Router de alta velocidad con tecnología WiFi 6', '1881b904-8626-43cc-b5ac-e84d0af7a8fb'),
	('10d8074b-f710-4d54-9652-8629a67f93f7', 'Switch Gigabit', 'Netgear', 'Switch de 8 puertos con tecnología Gigabit Ethernet', '1881b904-8626-43cc-b5ac-e84d0af7a8fb'),
	('e1a96def-3386-4834-8e47-a8f3758eb098', 'Extensor de Red', 'D-Link', 'Extensor de señal WiFi para hogares', '1881b904-8626-43cc-b5ac-e84d0af7a8fb'),
	('2d6d4d40-0661-4ea4-b5a4-bdb2adec1d93', 'Adaptador USB WiFi', 'TP-Link', 'Adaptador inalámbrico compacto', '1881b904-8626-43cc-b5ac-e84d0af7a8fb'),
	('a2ee03cb-150e-4326-96ae-bcaca5499bbd', 'Cable Ethernet Cat6', 'Belkin', 'Cable de alta velocidad para redes', '1881b904-8626-43cc-b5ac-e84d0af7a8fb'),
	('8cff63da-98ce-47c3-ad1e-97879d933f02', 'Software de Diseño', 'Adobe', 'Herramienta profesional para edición gráfica', '87e561ea-1ea0-49c2-9e94-dc1472a40cef'),
	('7f0880f0-e02d-4a12-9f5b-349cdef854f9', 'Antivirus', 'Kaspersky', 'Protección avanzada contra virus y malware', '87e561ea-1ea0-49c2-9e94-dc1472a40cef'),
	('7977e94d-0686-4622-865d-4f465e29f5d7', 'Suite Ofimática', 'Microsoft', 'Aplicaciones esenciales para la productividad', '87e561ea-1ea0-49c2-9e94-dc1472a40cef'),
	('a66d0539-06c9-4e36-b05e-c9ad585bacab', 'Software de Contabilidad', 'QuickBooks', 'Gestión financiera para negocios', '87e561ea-1ea0-49c2-9e94-dc1472a40cef'),
	('6c4a9097-26e3-4c56-95b9-92ecf8698025', 'Sistema Operativo', 'Linux', 'Distribución Ubuntu LTS', '87e561ea-1ea0-49c2-9e94-dc1472a40cef');

INSERT INTO public.direcciones VALUES
	('ddfad650-c041-4cff-b5c9-3429c869cb64', '20668 Atwood Way', 8),
	('c798f108-7d34-40ef-bea3-7143340a824f', '86000 Pine View Pass', 8),
	('686d029c-d1e5-4aa2-a339-6f3609c9231c', '486 Boyd Way', 8),
	('1e971d4c-f23d-4547-8c8b-52a0e49ea100', '86000 Pine View Pass', 8),
	('270593ce-728c-4d0d-af36-24d37ba00488', '486 Boyd Way', 8),
	('36826f1f-c25e-4ae9-84ff-6146aa99ec20', '7 Green Way', 4),
	('555185e1-952a-4647-8cc3-115faa3a84e9', '44399 Hintze Pass', 3),
	('fca7cb00-f2b5-47eb-855b-8d170b6aa23f', '703 Drewry Place', 3),
	('863a6689-e085-444f-a586-390346b394b9', '83 Grover Way', 3),
	('48207441-cd00-4ccd-92b9-28c31075b64b', '32707 Alpine Park', 3),
	('1fcd357b-54da-44de-a34e-689d52ed636d', '2566 Pennsylvania Terrace', 3),
	('f2553ab2-e2f9-410c-8775-76a1d20ebfd6', '70554 Fairview Street', 2),
	('2a680f1d-3c09-4d6f-8f77-e6209c862b92', '20668 Atwood Way', 8);

INSERT INTO public.proveedores VALUES 
	('54fe1e50-78d9-4897-960a-e1f8bb4da31f', 'Casper-Bogan', '1e971d4c-f23d-4547-8c8b-52a0e49ea100', 'hwaldren0@de.vu', '534-655-9388'),
	('ba48eb4a-dbd5-4bb2-836b-8e86770e49c3', 'Erdman LLC', '270593ce-728c-4d0d-af36-24d37ba00488', 'cbrawn1@si.edu', '947-872-6070'),
	('8723b735-2e34-49de-ab1e-d935ff7bdf16', 'Bernier, Harris and Wiza', '36826f1f-c25e-4ae9-84ff-6146aa99ec20', 'bhallewell2@themeforest.net', '450-677-1230'),
	('8e62f2b6-6d21-4b5e-b2ae-08eb50ec4501', 'Doyle Group', '555185e1-952a-4647-8cc3-115faa3a84e9', 'svern3@bluehost.com', '567-725-2671'),
	('32dd5f8a-efd6-4859-985a-06648a1f8110', 'Stanton, Yost and Emmerich', 'fca7cb00-f2b5-47eb-855b-8d170b6aa23f', NULL, '558-548-8120'),
	('e39549b0-c47c-40c8-af25-f0a45b95880f', 'Reichert Group', '863a6689-e085-444f-a586-390346b394b9', 'lsleicht5@list-manage.com', '611-248-7850'),
	('cc549435-b867-438c-8053-a475d7d8dfe1', 'Jerde LLC', '48207441-cd00-4ccd-92b9-28c31075b64b', 'kwestnage6@scientificamerican.com', '435-702-2619'),
	('9d8f7383-2da6-4e5c-9694-5a0e8632b751', 'Schaefer and Sons', '1fcd357b-54da-44de-a34e-689d52ed636d', 'lpietrzak7@stanford.edu', NULL),
	('cf6cb764-5c4b-4852-a39f-090bab2a98bb', 'Stokes-Mosciski', 'f2553ab2-e2f9-410c-8775-76a1d20ebfd6', 'emusk8@ftc.gov', '904-574-7741'),
	('6715e6e0-4f2d-4c94-a26e-68be07960e4f', 'Willms LLC', '2a680f1d-3c09-4d6f-8f77-e6209c862b92', 'boshields9@qq.com', '505-247-3562');

INSERT INTO public.productos_proveedores (producto, proveedor, precio, impuesto, total, stock) VALUES
-- Producto: Resma de Papel A4
('c9279638-902a-4bac-a9f0-7f284b20c9ce', '54fe1e50-78d9-4897-960a-e1f8bb4da31f', 5.00, 0.75, 5.75, 100),
('c9279638-902a-4bac-a9f0-7f284b20c9ce', 'ba48eb4a-dbd5-4bb2-836b-8e86770e49c3', 5.50, 0.83, 6.33, 120),
-- Producto: Grapadora Metálica
('6519389d-ce45-4fed-97ba-823518f20b61', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 10.00, 1.50, 11.50, 50),
('6519389d-ce45-4fed-97ba-823518f20b61', '8e62f2b6-6d21-4b5e-b2ae-08eb50ec4501', 9.75, 1.46, 11.21, 60),
-- Producto: Caja de Lápices
('528bc2c7-6a72-47c8-840e-3a4d092a6b47', '32dd5f8a-efd6-4859-985a-06648a1f8110', 3.00, 0.45, 3.45, 200),
('528bc2c7-6a72-47c8-840e-3a4d092a6b47', 'e39549b0-c47c-40c8-af25-f0a45b95880f', 2.90, 0.44, 3.34, 210),
-- Producto: Tijeras de Oficina
('7c73cb17-1d97-4361-a0ae-c10c7ef4f72f', 'cc549435-b867-438c-8053-a475d7d8dfe1', 8.50, 1.28, 9.78, 80),
('7c73cb17-1d97-4361-a0ae-c10c7ef4f72f', '9d8f7383-2da6-4e5c-9694-5a0e8632b751', 9.00, 1.35, 10.35, 70),
-- Producto: Calculadora Científica
('801a6449-b3a4-4184-bd3b-a5f1947444a8', 'cf6cb764-5c4b-4852-a39f-090bab2a98bb', 25.00, 3.75, 28.75, 40),
('801a6449-b3a4-4184-bd3b-a5f1947444a8', '6715e6e0-4f2d-4c94-a26e-68be07960e4f', 24.50, 3.68, 28.18, 35),
-- Producto: Procesador Intel Core i9
('0430a2c4-6c48-480e-9d99-9afd632f2b0a', '54fe1e50-78d9-4897-960a-e1f8bb4da31f', 300.00, 45.00, 345.00, 15),
('0430a2c4-6c48-480e-9d99-9afd632f2b0a', 'ba48eb4a-dbd5-4bb2-836b-8e86770e49c3', 310.00, 46.50, 356.50, 12),
-- Producto: Memoria RAM 16GB DDR4
('31c02657-3da7-44a9-b520-8e2fa2ee0b93', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 85.00, 12.75, 97.75, 20),
('31c02657-3da7-44a9-b520-8e2fa2ee0b93', '8e62f2b6-6d21-4b5e-b2ae-08eb50ec4501', 83.50, 12.53, 96.03, 18),
-- Producto: Disco Duro SSD 1TB
('a0cce63d-e945-4d37-86f5-7db672be99b5', '32dd5f8a-efd6-4859-985a-06648a1f8110', 120.00, 18.00, 138.00, 10),
('a0cce63d-e945-4d37-86f5-7db672be99b5', 'e39549b0-c47c-40c8-af25-f0a45b95880f', 115.00, 17.25, 132.25, 8),
-- Producto: Tarjeta Gráfica RTX 4070
('f811419a-79b7-488f-aec4-0e366ada9ccd', 'cc549435-b867-438c-8053-a475d7d8dfe1', 650.00, 97.50, 747.50, 10),
('f811419a-79b7-488f-aec4-0e366ada9ccd', '9d8f7383-2da6-4e5c-9694-5a0e8632b751', 630.00, 94.50, 724.50, 8),
-- Producto: Fuente de Poder 750W
('19550f5d-451a-4ac9-9a52-b0ba344f9dd2', 'cf6cb764-5c4b-4852-a39f-090bab2a98bb', 100.00, 15.00, 115.00, 25),
('19550f5d-451a-4ac9-9a52-b0ba344f9dd2', '6715e6e0-4f2d-4c94-a26e-68be07960e4f', 95.00, 14.25, 109.25, 20),
-- Producto: Teclado Mecánico RGB
('654c61b0-4e2e-4617-9318-3b380eb8bd5e', '54fe1e50-78d9-4897-960a-e1f8bb4da31f', 45.00, 6.75, 51.75, 30),
('654c61b0-4e2e-4617-9318-3b380eb8bd5e', 'ba48eb4a-dbd5-4bb2-836b-8e86770e49c3', 47.50, 7.13, 54.63, 28),
-- Producto: Mouse Gaming
('75367873-38ac-4ebb-9070-8df571ef0c07', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 25.00, 3.75, 28.75, 50),
('75367873-38ac-4ebb-9070-8df571ef0c07', '8e62f2b6-6d21-4b5e-b2ae-08eb50ec4501', 24.50, 3.68, 28.18, 45),
-- Producto: Auriculares Bluetooth
('cd751b1f-8def-4514-b11a-3ae6dd050b81', '32dd5f8a-efd6-4859-985a-06648a1f8110', 60.00, 9.00, 69.00, 40),
('cd751b1f-8def-4514-b11a-3ae6dd050b81', 'e39549b0-c47c-40c8-af25-f0a45b95880f', 58.50, 8.78, 67.28, 35),
-- Producto: Cámara Web Full HD
('ab9d613a-aba3-4c88-b1ca-7d27b8b793e2', 'cc549435-b867-438c-8053-a475d7d8dfe1', 80.00, 12.00, 92.00, 25),
('ab9d613a-aba3-4c88-b1ca-7d27b8b793e2', '9d8f7383-2da6-4e5c-9694-5a0e8632b751', 77.50, 11.63, 89.13, 20),
-- Producto: Base de Enfriamiento
('86448b06-cc2e-4216-acd2-a5e4869d90b5', 'cf6cb764-5c4b-4852-a39f-090bab2a98bb', 35.00, 5.25, 40.25, 30),
('86448b06-cc2e-4216-acd2-a5e4869d90b5', '6715e6e0-4f2d-4c94-a26e-68be07960e4f', 33.50, 5.03, 38.53, 28),
-- Producto: Monitor 24 pulgadas
('e613e3af-00b8-4c43-8bf0-d1bd31a0ed25', '54fe1e50-78d9-4897-960a-e1f8bb4da31f', 150.00, 22.50, 172.50, 15),
('e613e3af-00b8-4c43-8bf0-d1bd31a0ed25', 'ba48eb4a-dbd5-4bb2-836b-8e86770e49c3', 145.00, 21.75, 166.75, 12),
-- Producto: Monitor 32 pulgadas 4K
('d38b40aa-fb81-4f02-bde4-6f33926cd568', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 400.00, 60.00, 460.00, 5),
('d38b40aa-fb81-4f02-bde4-6f33926cd568', '8e62f2b6-6d21-4b5e-b2ae-08eb50ec4501', 390.00, 58.50, 448.50, 4),
-- Producto: Monitor Curvo 27 pulgadas
('57247ea2-cbb5-445b-8832-636aeecafe31', '32dd5f8a-efd6-4859-985a-06648a1f8110', 280.00, 42.00, 322.00, 8),
('57247ea2-cbb5-445b-8832-636aeecafe31', 'e39549b0-c47c-40c8-af25-f0a45b95880f', 275.00, 41.25, 316.25, 6),
-- Producto: Monitor Portátil
('def8c2bf-1576-4ac8-a11e-a9b45c6c5757', 'cc549435-b867-438c-8053-a475d7d8dfe1', 200.00, 30.00, 230.00, 10),
('def8c2bf-1576-4ac8-a11e-a9b45c6c5757', '9d8f7383-2da6-4e5c-9694-5a0e8632b751', 190.00, 28.50, 218.50, 9),
-- Producto: Monitor para Gaming
('317bda56-0cc6-4d22-bd89-91d39719a65d', 'cf6cb764-5c4b-4852-a39f-090bab2a98bb', 500.00, 75.00, 575.00, 7),
('317bda56-0cc6-4d22-bd89-91d39719a65d', '6715e6e0-4f2d-4c94-a26e-68be07960e4f', 480.00, 72.00, 552.00, 6),
-- Producto: Laptop Ultrabook
('ffab82ef-4512-4f82-a4f8-1e584da96583', '54fe1e50-78d9-4897-960a-e1f8bb4da31f', 1200.00, 180.00, 1380.00, 5),
('ffab82ef-4512-4f82-a4f8-1e584da96583', 'ba48eb4a-dbd5-4bb2-836b-8e86770e49c3', 1150.00, 172.50, 1322.50, 4),
-- Producto: PC Gamer
('9f69ed52-bf8e-4372-81c0-e7899130e2a8', '54fe1e50-78d9-4897-960a-e1f8bb4da31f', 1500.00, 225.00, 1725.00, 5),
('9f69ed52-bf8e-4372-81c0-e7899130e2a8', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 1480.00, 222.00, 1702.00, 4),
('9f69ed52-bf8e-4372-81c0-e7899130e2a8', 'cc549435-b867-438c-8053-a475d7d8dfe1', 1520.00, 228.00, 1748.00, 3),
-- Producto: Tablet 10 pulgadas
('b8af10e2-3447-4649-8623-cf453c294e4d', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 200.00, 30.00, 230.00, 7),
('b8af10e2-3447-4649-8623-cf453c294e4d', '54fe1e50-78d9-4897-960a-e1f8bb4da31f', 190.00, 28.50, 218.50, 5),
-- Producto: Chromebook
('43eaa734-d64e-4518-9677-d48854586b1a', 'ba48eb4a-dbd5-4bb2-836b-8e86770e49c3', 350.00, 52.50, 402.50, 10),
('43eaa734-d64e-4518-9677-d48854586b1a', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 340.00, 51.00, 391.00, 8),
-- Producto: Mini PC
('1c2a5627-a830-424b-ac2a-3a3223065993', '54fe1e50-78d9-4897-960a-e1f8bb4da31f', 450.00, 67.50, 517.50, 6),
-- Producto: Impresora Láser
('79f3afe5-f886-4427-a0e0-e29e34f2f999', 'cc549435-b867-438c-8053-a475d7d8dfe1', 120.00, 18.00, 138.00, 15),
-- Producto: Impresora Multifuncional
('f7893c0b-b40d-46eb-a01e-b7d0af84450b', 'e39549b0-c47c-40c8-af25-f0a45b95880f', 250.00, 37.50, 287.50, 9),
('f7893c0b-b40d-46eb-a01e-b7d0af84450b', 'ba48eb4a-dbd5-4bb2-836b-8e86770e49c3', 240.00, 36.00, 276.00, 8),
-- Producto: Plotter de Gran Formato
('6a156401-ca31-4fea-a414-6465f2a37a39', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 2200.00, 330.00, 2530.00, 3),
('6a156401-ca31-4fea-a414-6465f2a37a39', 'cc549435-b867-438c-8053-a475d7d8dfe1', 2150.00, 322.50, 2472.50, 2),
-- Producto: Impresora 3D
('7f0f5046-2b5e-47f5-8c71-672bcf4aa6bd', '9d8f7383-2da6-4e5c-9694-5a0e8632b751', 350.00, 52.50, 402.50, 6),
-- Producto: Escáner de Documentos
('b70a811a-1812-4bbf-a17d-70c1a9cce78c', '32dd5f8a-efd6-4859-985a-06648a1f8110', 180.00, 27.00, 207.00, 8),
-- Producto: Router WiFi 6
('2f1d3310-d301-411b-8790-384984f30cb6', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 120.00, 18.00, 138.00, 10),
('2f1d3310-d301-411b-8790-384984f30cb6', 'ba48eb4a-dbd5-4bb2-836b-8e86770e49c3', 115.00, 17.25, 132.25, 7),
('2f1d3310-d301-411b-8790-384984f30cb6', 'e39549b0-c47c-40c8-af25-f0a45b95880f', 125.00, 18.75, 143.75, 5),
-- Producto: Switch Gigabit
('10d8074b-f710-4d54-9652-8629a67f93f7', '54fe1e50-78d9-4897-960a-e1f8bb4da31f', 180.00, 27.00, 207.00, 4),
('10d8074b-f710-4d54-9652-8629a67f93f7', 'e39549b0-c47c-40c8-af25-f0a45b95880f', 175.00, 26.25, 201.25, 3),
-- Producto: Extensor de Red
('e1a96def-3386-4834-8e47-a8f3758eb098', '9d8f7383-2da6-4e5c-9694-5a0e8632b751', 50.00, 7.50, 57.50, 20),
-- Producto: Adaptador USB WiFi
('2d6d4d40-0661-4ea4-b5a4-bdb2adec1d93', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 30.00, 4.50, 34.50, 15),
-- Producto: Cable Ethernet Cat6
('a2ee03cb-150e-4326-96ae-bcaca5499bbd', 'cf6cb764-5c4b-4852-a39f-090bab2a98bb', 10.00, 1.50, 11.50, 25),
-- Producto: Software de Diseño
('8cff63da-98ce-47c3-ad1e-97879d933f02', '54fe1e50-78d9-4897-960a-e1f8bb4da31f', 400.00, 60.00, 460.00, 8),
('8cff63da-98ce-47c3-ad1e-97879d933f02', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 390.00, 58.50, 448.50, 6),
-- Producto: Antivirus
('7f0880f0-e02d-4a12-9f5b-349cdef854f9', '9d8f7383-2da6-4e5c-9694-5a0e8632b751', 50.00, 7.50, 57.50, 10),
-- Producto: Suite Ofimática
('7977e94d-0686-4622-865d-4f465e29f5d7', 'e39549b0-c47c-40c8-af25-f0a45b95880f', 150.00, 22.50, 172.50, 5),
-- Producto: Software de Contabilidad
('a66d0539-06c9-4e36-b05e-c9ad585bacab', 'ba48eb4a-dbd5-4bb2-836b-8e86770e49c3', 200.00, 30.00, 230.00, 4),
-- Producto: Sistema Operativo
('6c4a9097-26e3-4c56-95b9-92ecf8698025', '8723b735-2e34-49de-ab1e-d935ff7bdf16', 150.00, 22.50, 172.50, 7);

INSERT INTO public.imagenes_productos VALUES
	('2ec6d575-8b1f-48cf-9ce0-4a59fb4797a2', NULL, 'https://jwenterprises.com.py/userfiles/images/productos/hp-a4.jpg', 'c9279638-902a-4bac-a9f0-7f284b20c9ce'),
	('80133421-94c5-4277-9737-a2f908469a80', NULL, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTPsnN3onivDqBlnLbyfjfODGOsi8lKlcrxNA&s', '6519389d-ce45-4fed-97ba-823518f20b61'),
	('52f4359b-a3f5-4b6a-b3b0-9831ad5f863e', NULL, 'https://i.blogs.es/4af7cf/corei9-2/1366_2000.jpg', '0430a2c4-6c48-480e-9d99-9afd632f2b0a'),
	('bc56cde3-d934-43ab-a777-f6442eb1e701', NULL, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQyok6tnuP4Sm1rgQc10T5mUDFZHIdT3TxTcQ&s', '10d8074b-f710-4d54-9652-8629a67f93f7'),
	('f6f0f3bd-b0c3-40b0-97d9-5f06568749d1', NULL, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQwBieOMTiau9ynjZbFkJgV2nc4AR7t2AYiog&s', '19550f5d-451a-4ac9-9a52-b0ba344f9dd2'),
	('caec3e43-1e02-4a45-91ef-91680c3824ad', NULL, 'https://m.media-amazon.com/images/I/61dH7gZc3JL.jpg', '1c2a5627-a830-424b-ac2a-3a3223065993'),
	('0840e378-9355-4057-acfb-4e036d0eefca', NULL, 'https://www.syscomstore.com/image/cache/catalog/productos/usb%20wifi/TL-WN725N-600x600.jpg', '2d6d4d40-0661-4ea4-b5a4-bdb2adec1d93'),
	('dd744bf7-d888-4ca6-aded-229b0b5015c0', NULL, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQNVQuf-EjSMsUzXhRETfZw8qHVJkcjZGKMfA&s', '2f1d3310-d301-411b-8790-384984f30cb6'),
	('4b80fc6b-d39f-447f-a927-58dd598db329', NULL, 'https://image.benq.com/is/image/benqco/2-ex2710u-front-low?$ResponsivePreset$', '317bda56-0cc6-4d22-bd89-91d39719a65d'),
	('682c6864-459c-4733-8482-4c1cc2e9e25b', NULL, 'https://m.media-amazon.com/images/I/51Y7ugfDRWS._AC_UF894,1000_QL80_.jpg', '31c02657-3da7-44a9-b520-8e2fa2ee0b93'),
	('96f6a045-52e0-4aaf-a8e1-175daf2761e5', NULL, 'https://m.media-amazon.com/images/I/61Ua7vI8kgL._AC_SL1500_.jpg', '43eaa734-d64e-4518-9677-d48854586b1a'),
	('3dfc36a8-090b-4995-8945-0aa3cc150ac5', NULL, 'https://images.samsung.com/is/image/samsung/p6pim/mx/lc27r500fhlxzx/gallery/mx-c27r5-lc27r500fhlxzx-538194916?$720_576_JPG$', '57247ea2-cbb5-445b-8832-636aeecafe31'),
	('4ff82886-148f-4faa-b163-2ebfe33968f9', NULL, 'https://m.media-amazon.com/images/I/51ONOnT6lAL._AC_UF894,1000_QL80_.jpg', '654c61b0-4e2e-4617-9318-3b380eb8bd5e'),
	('563d299d-7121-40dd-a507-89286411e689', NULL, 'https://fscompras.com/wp-content/uploads/2021/06/1600811129_IMG_1422452.jpg', '6a156401-ca31-4fea-a414-6465f2a37a39'),
	('1c46a4dd-d167-42fc-aee4-e4b24d64a7ea', NULL, 'https://store.perudataconsult.net/cdn/shop/products/windows_11_pro_940x.jpg?v=1642112444', '6c4a9097-26e3-4c56-95b9-92ecf8698025'),
	('64f36fe3-37ac-436c-a4d2-4cd78beba8e5', NULL, 'https://m.media-amazon.com/images/I/6157EeRHinL.jpg', '75367873-38ac-4ebb-9070-8df571ef0c07'),
	('d1acdc77-a9a5-4ea6-9702-438a83c7e6a7', NULL, 'https://m.media-amazon.com/images/I/41xneoMRwYL._SR290,290_.jpg', '7977e94d-0686-4622-865d-4f465e29f5d7'),
	('5665b2b1-3378-46d8-aef0-7838cfae85ea', NULL, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRxG1AF0cCb0bh55c04i_dp0R2iq66jrRq_Bg&s', '79f3afe5-f886-4427-a0e0-e29e34f2f999'),
	('6a07259b-91a0-4fd9-b1ce-0d1fa25f40bf', NULL, 'https://m.media-amazon.com/images/I/61UHy4s9lDL.jpg', '7c73cb17-1d97-4361-a0ae-c10c7ef4f72f'),
	('b0e4734b-510e-490d-ba34-21fc5141334a', NULL, 'https://m.media-amazon.com/images/I/51WMUrihzrL._AC_UF1000,1000_QL80_.jpg', '7f0880f0-e02d-4a12-9f5b-349cdef854f9'),
	('cca0aa3a-ac25-48d9-9c13-9bce81a011f2', NULL, 'https://m.media-amazon.com/images/I/71m1Tf4cIUL._AC_UF894,1000_QL80_.jpg', '7f0f5046-2b5e-47f5-8c71-672bcf4aa6bd'),
	('e0edf9aa-4daf-4be3-bbc4-f08a2fd6e80d', NULL, 'https://m.media-amazon.com/images/I/71jeBCw4QjL._AC_UF894,1000_QL80_.jpg', '801a6449-b3a4-4184-bd3b-a5f1947444a8'),
	('f8a9274f-c664-47a8-86e8-9e122a772239', NULL, 'https://m.media-amazon.com/images/I/612FZA8dF1L._AC_UF894,1000_QL80_.jpg', '86448b06-cc2e-4216-acd2-a5e4869d90b5'),
	('439d0837-d572-4908-a6d2-831909b58b0d', NULL, 'https://m.media-amazon.com/images/I/61QpWhIgjRL._AC_SX148_SY213_QL70_.jpg', '8cff63da-98ce-47c3-ad1e-97879d933f02'),
	('3c1bb3d9-0191-48f3-9e3a-093730e5ca3e', NULL, 'https://m.media-amazon.com/images/I/815zWuDT6-L._AC_UF894,1000_QL80_.jpg', '9f69ed52-bf8e-4372-81c0-e7899130e2a8'),
	('b48dddb1-062d-4b67-b602-0e09925fc017', NULL, 'https://m.media-amazon.com/images/I/41ibMIldfBL.jpg', 'a0cce63d-e945-4d37-86f5-7db672be99b5'),
	('e97fe4af-b2bb-47a3-99fc-fd693fef269f', NULL, 'https://m.media-amazon.com/images/I/71xB7MVILeL.jpg', 'a2ee03cb-150e-4326-96ae-bcaca5499bbd'),
	('ad6986c6-b74b-4827-a1cd-9f6c3709cfcd', NULL, 'https://images-na.ssl-images-amazon.com/images/I/81QORAEah8L._AC_UL330_SR330,330_.jpg', 'a66d0539-06c9-4e36-b05e-c9ad585bacab'),
	('6c8261e9-99c3-4fc5-95ee-15ca3f31fb90', NULL, 'https://m.media-amazon.com/images/I/51UmobZQQaL._AC_UF894,1000_QL80_.jpg', 'ab9d613a-aba3-4c88-b1ca-7d27b8b793e2'),
	('3d8bd506-0827-451b-ba63-55039b7a6fef', NULL, 'https://m.media-amazon.com/images/I/71y9sVY4mnL.jpg', 'b70a811a-1812-4bbf-a17d-70c1a9cce78c'),
	('a500f5b3-e335-48a9-90f3-6534346c3d94', NULL, 'https://m.media-amazon.com/images/I/81IjVvOXGdL.jpg', 'b8af10e2-3447-4649-8623-cf453c294e4d'),
	('5b05c101-4559-4984-ad65-30bd0df7c856', NULL, 'https://m.media-amazon.com/images/I/81f5z7PopnL.jpg', 'cd751b1f-8def-4514-b11a-3ae6dd050b81'),
	('23480371-9324-46ba-8196-c8f41632ffbf', NULL, 'https://m.media-amazon.com/images/I/71V9R7esdxL._AC_UF894,1000_QL80_.jpg', 'd38b40aa-fb81-4f02-bde4-6f33926cd568'),
	('fd91d917-86d4-4af4-85c7-0c4754b3badc', NULL, 'https://m.media-amazon.com/images/I/71ikvSKQpxL._AC_UF894,1000_QL80_.jpg', 'def8c2bf-1576-4ac8-a11e-a9b45c6c5757'),
	('3db0c5e2-c538-409e-ba99-8028e0891985', NULL, 'https://m.media-amazon.com/images/I/51NKLAjbWEL._AC_UF350,350_QL80_.jpg', 'e1a96def-3386-4834-8e47-a8f3758eb098'),
	('393a70cd-f606-42e8-a68b-d55ad0244a5f', NULL, 'https://m.media-amazon.com/images/I/81xrHN1BYFL._AC_UF894,1000_QL80_.jpg', 'e613e3af-00b8-4c43-8bf0-d1bd31a0ed25'),
	('3cd447bf-84b9-4a92-8007-4ee550126f9c', NULL, 'https://m.media-amazon.com/images/I/71bGaru+R7L._AC_UF894,1000_QL80_.jpg', 'f7893c0b-b40d-46eb-a01e-b7d0af84450b'),
	('21b18414-3126-45d6-9cb4-d9814ee8758c', NULL, 'https://m.media-amazon.com/images/I/71OK1+ewI0L._AC_UF894,1000_QL80_.jpg', 'f811419a-79b7-488f-aec4-0e366ada9ccd'),
	('45347f76-2a2c-474f-95ae-f2a352d41612', NULL, 'https://m.media-amazon.com/images/I/818bWToJTUL.jpg', 'ffab82ef-4512-4f82-a4f8-1e584da96583'),
	('4d2b750b-d362-4ce6-bdc0-4b1cfcf47c5d', NULL, 'https://m.media-amazon.com/images/I/71vdh4I-unL.jpg', '528bc2c7-6a72-47c8-840e-3a4d092a6b47');

