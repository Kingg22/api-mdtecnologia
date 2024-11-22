using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MD_Tech.Context;
using MD_Tech.Models;
using MD_Tech.DTOs;
using MD_Tech.Storage;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text.Json;
using NodaTime;
using System.Text.RegularExpressions;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class ProductosController : ControllerBase
    {
        private readonly MdtecnologiaContext context;
        private readonly LogsApi<ProductosController> logger;
        private readonly IStorageApi storageApi;

        public ProductosController(MdtecnologiaContext context, IStorageApi storageApi, LogsApi<ProductosController> logger)
        {
            this.context = context;
            this.storageApi = storageApi;
            this.logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Obtiene todos los productos", Description = "Devuelve una lista de productos")]
        [SwaggerResponse(200, "Operación exitosa", typeof(List<ProductosDto>))]
        public async Task<ActionResult<List<ProductosDto>>> GetProductos([FromQuery] ProductosRequestDto paginacionDto)
        {
            if (paginacionDto.Page < 0 || paginacionDto.Limit < 0)
                return BadRequest(new { paginacion = "las números de paginación o límites no pueden ser negativos" });
            var query = context.Productos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(paginacionDto.Nombre))
                query = query.Where(p => EF.Functions.ILike(p.Nombre, $"%{paginacionDto.Nombre}%"));
            if (!string.IsNullOrWhiteSpace(paginacionDto.Marca))
                query = query.Where(p => EF.Functions.ILike(p.Marca, $"%{paginacionDto.Marca}%"));
            if (paginacionDto.Categoria != null)
                query = query.Where(p => p.Categoria == paginacionDto.Categoria);
            if (!string.IsNullOrWhiteSpace(paginacionDto.OrderBy))
            {
                var regex = OrderByRegex();
                var match = regex.Match(paginacionDto.OrderBy);

                if (match.Success)
                {
                    var orderBy = paginacionDto.OrderBy.Split("-");
                    var orderByProperty = orderBy[0];
                    var orderByDirection = orderBy.Length > 1 && orderBy[1].Equals("DESC", StringComparison.OrdinalIgnoreCase);
                    var property = typeof(Producto).GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property != null)
                    {
                        // Verifica si la propiedad es de tipo ICollection o virtual (no es posible ordenar si no es columna simple)
                        if (typeof(ICollection<>).IsAssignableFrom(property.PropertyType) || property.GetGetMethod().IsVirtual)
                        {
                            return BadRequest(new { OrderBy = "No se puede ordenar por este propiedad." });
                        }
                        query = orderByDirection
                            ? query.OrderByDescending(e => EF.Property<object>(e, property.Name))
                            : query.OrderBy(e => EF.Property<object>(e, property.Name));
                    }
                    else
                        return BadRequest(new { OrderBy = "No se ha podido determinar la columna para ordenar sus resultados, formato esperado es 'columna' o 'columna-desc'" });
                }
                else
                    return BadRequest(new { OrderBy = "Formato inválido. Use 'columna' o 'columna-desc'." });
            }
            else
                query = query.OrderBy(p => p.Id);

            var totalProducts = await context.Productos.CountAsync();
            var hasNextPage = (paginacionDto.Page + 1) * paginacionDto.Limit < totalProducts;
            // Calcula la página anterior que contiene resultados
            var previousPage = paginacionDto.Page - 1;
            while (previousPage > 0 && previousPage * paginacionDto.Limit >= totalProducts)
                previousPage--;

            var nextUrl = hasNextPage
                ? Url.Action(
                    nameof(GetProductos),
                    "Productos",
                    new ProductosRequestDto
                    {
                        Nombre = paginacionDto.Nombre,
                        Marca = paginacionDto.Marca,
                        Categoria = paginacionDto.Categoria,
                        OrderBy = paginacionDto.OrderBy,
                        Limit = paginacionDto.Limit,
                        Page = paginacionDto.Page + 1
                    },
                    Request.Scheme)
                : null;

            var previousUrl = previousPage >= 0 && previousPage * paginacionDto.Limit < totalProducts
                ? Url.Action(
                    nameof(GetProductos),
                    "Productos",
                    new ProductosRequestDto
                    {
                        Nombre = paginacionDto.Nombre,
                        Marca = paginacionDto.Marca,
                        Categoria = paginacionDto.Categoria,
                        OrderBy = paginacionDto.OrderBy,
                        Limit = paginacionDto.Limit,
                        Page = previousPage
                    },
                    Request.Scheme
                ) : null;

            return Ok(new
            {
                count = totalProducts,
                next = nextUrl,
                previous = previousUrl,
                productos = await query
                .Skip(paginacionDto.Page * paginacionDto.Limit)
                .Take(paginacionDto.Limit)
                .Include(p => p.ImagenesProductos)
                .Include(p => p.ProductosProveedores)
                .Select(p => new ProductosDto(p))
                .AsNoTracking()
                .ToListAsync()
            });
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtiene un producto por ID", Description = "Devuelve el detalle del producto")]
        [SwaggerResponse(200, "Operación exitosa", typeof(ProductosDto))]
        [SwaggerResponse(404, "Producto no encontrado")]
        public async Task<ActionResult<ProductosDto>> GetProductos(Guid id)
        {
            var p = await context.Productos
                .Include(producto => producto.ProductosProveedores)
                .Include(p => p.ImagenesProductos)
                .OrderBy(p => p.Id)
                .FirstOrDefaultAsync(p => p.Id == id);
            return p == null ? NotFound() : Ok(new { producto = new ProductosDto(p) });
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Actualiza un producto", Description = "Se actualizan todos los campos de un producto")]
        [SwaggerResponse(200, "Producto actualizado", typeof(ProductosDto))]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(404, "Producto no encontrado")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult<ProductosDto>> PutProductos(Guid id, [FromBody] ProductosDto productoDto)
        {
            if (id != productoDto.Id)
            {
                logger.Informacion($"Producto de id {id} no coincide con el body id {productoDto.Id}");
                return BadRequest(new { id = "El id del body no coincide con la ruta" });
            }
            var producto = await context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound(new { id = "producto no encontrado" });
            var result = await ValidarCrearProducto(productoDto);
            if (result != null)
                return result;

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                producto.Nombre = productoDto.Nombre;
                producto.Marca = productoDto.Marca;
                producto.Categoria = productoDto.Categoria;
                producto.Descripcion = productoDto.Descripcion;

                if (productoDto.Proveedores?.Count > 0)
                {
                    logger.Informacion("Relaciones de proveedores encontradas, procesando...");
                    logger.Depuracion("Producto proveedores dto: " + JsonSerializer.Serialize(productoDto.Proveedores));
                    var proveedoresUnicos = productoDto.Proveedores.Select(pp => pp.Proveedor).Distinct();
                    logger.Depuracion("Proveedores dto Id unique: " + JsonSerializer.Serialize(proveedoresUnicos));
                    var proveedoresExistentes = await context.Proveedores.Where(p => proveedoresUnicos.Contains(p.Id)).Select(p => p.Id).ToListAsync();
                    logger.Depuracion("Proveedores id válidos: " + JsonSerializer.Serialize(proveedoresExistentes));
                    var productosProveedor = productoDto.Proveedores.Where(proveedorDto => proveedoresExistentes.Contains(proveedorDto.Proveedor));
                    foreach (var proveedorDto in productosProveedor)
                    {
                        var relacion = await context.ProductosProveedores.FirstOrDefaultAsync(pp => pp.Producto == id && pp.Proveedor == proveedorDto.Proveedor);

                        if (relacion != null)
                        {
                            // Actualizar la relación existente
                            relacion.Precio = proveedorDto.Precio;
                            relacion.Impuesto = proveedorDto.Impuesto;
                            relacion.Total = proveedorDto.Precio + proveedorDto.Impuesto;
                            relacion.FechaActualizado = proveedorDto.FechaActualizado ?? LocalDate.FromDateTime(DateTime.UtcNow);
                            relacion.Stock = proveedorDto.Stock;
                        }
                        else
                        {
                            var productoProveedor = new ProductosProveedor()
                            {
                                Producto = id,
                                Proveedor = proveedorDto.Proveedor,
                                Precio = proveedorDto.Precio,
                                Impuesto = proveedorDto.Impuesto,
                                Total = proveedorDto.Precio * proveedorDto.Impuesto,
                                FechaActualizado = proveedorDto.FechaActualizado ?? LocalDate.FromDateTime(DateTime.UtcNow),
                                Stock = proveedorDto.Stock
                            };
                            await context.ProductosProveedores.AddAsync(productoProveedor);
                        }
                    }
                    // Considerar si eliminar o no lo existente
                    // var relacionesAEliminar = context.ProductosProveedores.Where(rel => rel.Producto == producto.Id && !proveedoresExistentes.Contains(rel.Proveedor));
                    // context.ProductosProveedores.RemoveRange(relacionesAEliminar);
                }

                if (productoDto.Imagenes?.Count > 0)
                {
                    logger.Informacion("Relaciones de imágenes producto encontradas. Procesando...");
                    logger.Depuracion("Imágenes producto Dto: " + JsonSerializer.Serialize(productoDto.Imagenes));
                    // Añade solo los links nuevos 
                    var imagenesExistentes = context.ImagenesProductos.Where(imagen => imagen.Producto == producto.Id).Select(imagen => imagen.Url);
                    var imagenesValidas = productoDto.Imagenes
                        .Where(imagenDto => !string.IsNullOrWhiteSpace(imagenDto.Url) && IsValidUrl(imagenDto.Url) && !imagenesExistentes.Contains(imagenDto.Url))
                        .Select(imagenDto => new ImagenesProducto()
                        {
                            Producto = producto.Id,
                            Url = imagenDto.Url,
                            Descripcion = string.IsNullOrWhiteSpace(imagenDto.Descripcion) ? null : imagenDto.Descripcion,
                        }).ToList();
                    if (imagenesValidas.Count > 0)
                        await context.ImagenesProductos.AddRangeAsync(imagenesValidas);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                logger.Informacion($"Se ha actualizado el producto {producto.Id}");
                await context.Entry(producto).ReloadAsync();
                await context.Entry(producto).Collection(p => p.ProductosProveedores).LoadAsync();
                await context.Entry(producto).Collection(p => p.ImagenesProductos).LoadAsync();
                return Ok(new { producto = new ProductosDto(producto) });
            }
            catch (Exception ex)
            {
                logger.Excepciones(ex, "Error general al actualiza producto");
                await transaction.RollbackAsync();
                return Problem();
            }
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Crea un producto", Description = "Agrega un nuevo producto a la base de datos")]
        [SwaggerResponse(201, "Producto creado", typeof(ProductosDto))]
        [SwaggerResponseHeader(201, "location", "string", "Enlace al recurso creado")]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult<ProductosDto>> PostProductos([FromBody] ProductosDto productoDto)
        {
            logger.Informacion($"Iniciando la creación del producto con ID {productoDto.Id} y nombre {productoDto.Nombre}");
            var result = await ValidarCrearProducto(productoDto);
            if (result != null)
                return result;
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var productos = new Producto()
                {
                    Id = productoDto.Id ?? Guid.NewGuid(),
                    Nombre = productoDto.Nombre,
                    Marca = productoDto.Marca,
                    Categoria = productoDto.Categoria,
                    Descripcion = productoDto.Descripcion,
                };
                await context.Productos.AddAsync(productos);

                if (productoDto.Proveedores?.Count > 0)
                {
                    logger.Informacion("Relaciones de proveedores encontradas, procesando...");
                    logger.Depuracion("Producto proveedores dto: " + JsonSerializer.Serialize(productoDto.Proveedores));
                    // Se genera una lista temporal para buscar los id de proveedores en la BD
                    var proveedoresUnicos = productoDto.Proveedores.Select(pp => pp.Proveedor).Distinct();
                    logger.Depuracion("Proveedores dto Id unique: " + JsonSerializer.Serialize(proveedoresUnicos));
                    // lista con todos los ID existentes de la lista anterior
                    var proveedoresExistentes = await context.Proveedores.Where(p => proveedoresUnicos.Contains(p.Id)).Select(p => p.Id).ToListAsync();
                    logger.Depuracion("Proveedores id válidos: " + JsonSerializer.Serialize(proveedoresExistentes));
                    // Se filtra del input solo los productos - proveedor con Id de proveedor válido y se convierte a model
                    var productosProveedor = productoDto.Proveedores
                        .Where(proveedorDto => proveedoresExistentes.Contains(proveedorDto.Proveedor))
                        .Select(proveedorDto =>
                        new ProductosProveedor()
                        {
                            Producto = productos.Id,
                            Proveedor = proveedorDto.Proveedor,
                            Precio = proveedorDto.Precio,
                            Impuesto = proveedorDto.Impuesto,
                            Total = proveedorDto.Precio + proveedorDto.Impuesto,
                            FechaActualizado = proveedorDto.FechaActualizado ?? LocalDate.FromDateTime(DateTime.UtcNow),
                            Stock = proveedorDto.Stock
                        }).ToList();
                    if (productosProveedor.Count > 0)
                        await context.ProductosProveedores.AddRangeAsync(productosProveedor);
                }

                if (productoDto.Imagenes?.Count > 0)
                {
                    logger.Informacion("Relaciones de imágenes producto encontradas. Procesando...");
                    logger.Depuracion("Imágenes producto Dto: " + JsonSerializer.Serialize(productoDto.Imagenes));
                    var imagenesValidas = productoDto.Imagenes
                        .Where(imagenDto => !string.IsNullOrWhiteSpace(imagenDto.Url) && IsValidUrl(imagenDto.Url))
                        .Select(imagenDto => new ImagenesProducto()
                        {
                            Producto = productos.Id,
                            Url = imagenDto.Url,
                            Descripcion = string.IsNullOrWhiteSpace(imagenDto.Descripcion) ? null : imagenDto.Descripcion,
                        }).ToList();
                    if (imagenesValidas.Count > 0)
                        await context.ImagenesProductos.AddRangeAsync(imagenesValidas);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                await context.Entry(productos).ReloadAsync();
                await context.Entry(productos).Collection(p => p.ProductosProveedores).LoadAsync();
                await context.Entry(productos).Collection(p => p.ImagenesProductos).LoadAsync();
                logger.Informacion($"Se ha creado un nuevo producto Id: {productos.Id} Nombre: {productos.Nombre}");
                return Created(Url.Action(nameof(GetProductos), "Productos", new { id = productos.Id }, Request.Scheme), new { producto = new ProductosDto(productos) });
            }
            catch (Exception ex)
            {
                logger.Excepciones(ex, "Excepción general al crear producto");
                await transaction.RollbackAsync();
                return Problem("Ha ocurrido un error al guardar en la base de datos, revisar si el producto solo fue creado");
            }
        }

        [HttpPost("image/{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Añade una imagen a un producto", Description = "Agrega una nueva imagen al servicio utilizado y guarda en la base de datos su referencia")]
        [SwaggerResponse(202, "Imagen guardada")]
        [SwaggerResponseHeader(202, "location", "string", "Enlace a la imagen guardada")]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(404, "Producto no encontrado")]
        [SwaggerResponse(500, "No se pudo guardar la imagen")]
        public async Task<ActionResult> UploadImage(Guid id, IFormFile image)
        {
            if (image.Length == 0)
                return BadRequest(new { image = "No se ha proporcionado un archivo de imagen válido." });
            if (!image.ContentType.StartsWith("image/"))
                return BadRequest(new { image = "El archivo debe ser una imagen válida." });
            // 5 MB en bytes limitados por tema de rendimiento del fronted y costo de almacenamiento
            const long maxSizeInBytes = 5 * 1024 * 1024;
            if (image.Length > maxSizeInBytes)
                return BadRequest(new { image = "El archivo de imagen excede el tamaño máximo permitido de 5 MB." });

            var producto = await context.Productos.FindAsync(id);
            if (producto != null)
            {
                await using var stream = image.OpenReadStream();
                var result = await storageApi.PutObjectAsync(new StorageApiDto()
                {
                    Name = image.FileName,
                    Type = image.ContentType,
                    Stream = stream,
                });
                if (result != null && result.Status && result.Url != null)
                {
                    producto.ImagenesProductos.Add(new ImagenesProducto()
                    {
                        Producto = producto.Id,
                        Descripcion = result.Name,
                        Url = result.Url.ToString(),
                    });
                    await context.SaveChangesAsync();
                    return Accepted(result.Url);
                }
                else
                    return Problem();
            }
            else
                return NotFound();
        }

        [HttpPatch("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Actualiza los proveedores de un producto", Description = "Agrega o actualiza una lista de proveedores de un producto a la base de datos")]
        [SwaggerResponse(200, "Producto actualizado", typeof(ProductosDto))]
        [SwaggerResponse(404, "Producto no encontrado")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult<ProductosDto>> UpdatePrecios(Guid id, List<ProductoProveedorDto> listProductoProveedorDto)
        {
            var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var producto = await context.Productos.FindAsync(id);
                if (producto == null)
                    return NotFound();

                var proveedoresUnicos = listProductoProveedorDto.Where(pp => pp.Producto == id).Select(pp => pp.Proveedor).Distinct().ToList();
                var proveedoresExistentes = await context.Proveedores.Where(p => proveedoresUnicos.Contains(p.Id)).Select(p => p.Id).ToListAsync();
                foreach (var proveedorDto in listProductoProveedorDto)
                {
                    if (!proveedoresExistentes.Contains(proveedorDto.Proveedor))
                    {
                        logger.Advertencia($"Se ha omitido la relación producto - proveedor con Id {proveedorDto.Proveedor} no encontrado, precio = {proveedorDto.Precio} + impuesto = {proveedorDto.Impuesto}");
                        continue;
                    }

                    var productoProveedorExistente = await context.ProductosProveedores.FirstOrDefaultAsync(pp => pp.Producto == id && pp.Proveedor == proveedorDto.Proveedor);
                    if (productoProveedorExistente != null)
                    {
                        // Actualizar si ya existe
                        productoProveedorExistente.Precio = proveedorDto.Precio;
                        productoProveedorExistente.Impuesto = proveedorDto.Impuesto;
                        productoProveedorExistente.Total = proveedorDto.Precio + proveedorDto.Impuesto;
                        productoProveedorExistente.Stock = proveedorDto.Stock;
                        productoProveedorExistente.FechaActualizado = proveedorDto.FechaActualizado ?? LocalDate.FromDateTime(DateTime.UtcNow);
                    }
                    else
                    {
                        var productoProveedor = new ProductosProveedor()
                        {
                            Producto = id,
                            Proveedor = proveedorDto.Proveedor,
                            Precio = proveedorDto.Precio,
                            Impuesto = proveedorDto.Impuesto,
                            Total = proveedorDto.Precio + proveedorDto.Impuesto,
                            Stock = proveedorDto.Stock,
                            FechaActualizado = proveedorDto.FechaActualizado ?? LocalDate.FromDateTime(DateTime.UtcNow),
                        };
                        await context.ProductosProveedores.AddAsync(productoProveedor);
                    }
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                await context.Entry(producto).ReloadAsync();
                await context.Entry(producto).Collection(p => p.ProductosProveedores).LoadAsync();
                return Ok(new { producto = new ProductosDto(producto) });
            }
            catch (Exception ex)
            {
                logger.Excepciones(ex, "Error general al actualizar precios");
                await transaction.RollbackAsync();
                return Problem();
            }
        }

        [HttpPatch("image/{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Actualiza las imágenes de un producto", Description = "Agrega o actualiza una lista de imágenes de un producto a la base de datos")]
        [SwaggerResponse(200, "Producto actualizado", typeof(ProductosDto))]
        [SwaggerResponse(404, "Producto no encontrado")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult<ProductosDto>> UpdateImagenes(Guid id, List<ImagenesProductoDto> listImagenesProducto)
        {
            var productoId = listImagenesProducto.Select(lp => lp.Producto).Distinct();
            if (productoId.Any(pid => pid != id))
            {
                logger.Informacion($"Producto de id {id} no coincide con todos los id del body");
                return BadRequest(new { id = "No todos los id del body no coincide con la ruta" });
            }
            var producto = await context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound(new { id = "producto no encontrado" });
            
            if (listImagenesProducto.Count > 0)
            {
                logger.Informacion("Relaciones de imágenes producto encontradas. Procesando...");
                logger.Depuracion("Imágenes producto Dto: " + JsonSerializer.Serialize(listImagenesProducto));
                // Añade solo los links nuevos 
                var imagenesExistentes = context.ImagenesProductos.Where(imagen => imagen.Producto == producto.Id).Select(imagen => imagen.Url);
                var imagenesValidas = listImagenesProducto
                    .Where(imagenDto => !string.IsNullOrWhiteSpace(imagenDto.Url) && IsValidUrl(imagenDto.Url) && !imagenesExistentes.Contains(imagenDto.Url))
                    .Select(imagenDto => new ImagenesProducto()
                    {
                        Producto = producto.Id,
                        Url = imagenDto.Url,
                        Descripcion = string.IsNullOrWhiteSpace(imagenDto.Descripcion) ? null : imagenDto.Descripcion,
                    }).ToList();
                if (imagenesValidas.Count > 0)
                    await context.ImagenesProductos.AddRangeAsync(imagenesValidas);
            }
            else
                return BadRequest(new { imagenes = "la lista está vacía, ingrese datos válidos" });

            await context.SaveChangesAsync();
            await context.Entry(producto).ReloadAsync();
            await context.Entry(producto).Collection(p => p.ImagenesProductos).LoadAsync();
            return Ok(new { producto = new ProductosDto(producto) });
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Elimina un producto", Description = "Se elimina un producto de la base de datos")]
        [SwaggerResponse(204, "Producto eliminado")]
        [SwaggerResponse(404, "Producto no encontrado")]
        public async Task<ActionResult> DeleteProductos(Guid id)
        {
            var productos = await context.Productos.FindAsync(id);
            if (productos == null)
                return NotFound();
            logger.Advertencia($"Se va a eliminar el producto con Id: {id} y nombre: {productos.Nombre}");
            context.Productos.Remove(productos);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}/{idImagen}")]
        [Authorize]
        [SwaggerOperation(Summary = "Elimina la imagen de referencia de un producto", Description = "Se elimina la imagen del servicio utilizado y de la base de datos")]
        [SwaggerResponse(200, "Imagen eliminada del producto", typeof(ProductosDto))]
        [SwaggerResponse(404, "No encontrado, especificado en la propiedad en el body", typeof(Dictionary<string, string>))]
        [SwaggerResponse(500, "No se pudo eliminar la imagen en el servicio")]
        public async Task<ActionResult> DeleteImagenes(Guid id, Guid idImagen)
        {
            var producto = await context.Productos.Include(p => p.ImagenesProductos).FirstOrDefaultAsync(p => p.Id == id);
            if (producto == null)
                return NotFound(new { producto = "No fue encontrado el producto" });
            var imagen = producto.ImagenesProductos.FirstOrDefault(imagenes => imagenes.Id == idImagen);
            if (imagen == null)
                return NotFound(new { imagen = "no fue encontrada la imagen con ese Id para ese producto" });
            if (string.IsNullOrWhiteSpace(imagen.Descripcion))
                return Problem("No se pudo obtener el nombre para eliminar su referencia en el servidor.");

            var response = await storageApi.DeleteObjectAsync(imagen.Descripcion);
            if (response)
            {
                context.Remove(imagen);
                await context.SaveChangesAsync();
                await context.Entry(producto).ReloadAsync();
                await context.Entry(producto).Collection(p => p.ProductosProveedores).LoadAsync();
                await context.Entry(producto).Collection(p => p.ImagenesProductos).LoadAsync();
                logger.Advertencia($"Se ha eliminado una imagen del producto {producto.Id}");
                return Ok(new ProductosDto(producto));
            }
            return Problem("No se pudo eliminar la imagen en el servicio utilizado");
        }

        [SwaggerIgnore]
        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        [SwaggerIgnore]
        private async Task<ActionResult?> ValidarCrearProducto(ProductosDto productoDto)
        {
            // Campos obligatorios tipo string
            if (string.IsNullOrWhiteSpace(productoDto.Nombre) || string.IsNullOrWhiteSpace(productoDto.Marca))
                return BadRequest(new { fieldsString = "campos como Nombre o Marca son requeridos" });
            // Descripción no es null se valida tipo string
            if (productoDto.Descripcion != null && string.IsNullOrWhiteSpace(productoDto.Descripcion))
                return BadRequest(new { descripcion = "la descripción no puede ser espacios en blanco" });
            if (productoDto.Categoria != null && await context.Categorias.FindAsync(productoDto.Categoria) == null)
                return NotFound(new { categoria = "categoría de producto no encontrada" });
            return null;
        }

        [GeneratedRegex(@"^(?<property>\w+)(-(?<direction>asc|desc))?$", RegexOptions.IgnoreCase, "es-PA")]
        private static partial Regex OrderByRegex();
    }
}
