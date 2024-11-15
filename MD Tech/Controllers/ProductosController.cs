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
using NodaTime;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly MdtecnologiaContext MdTecnologiaContext;
        private readonly LogsApi<ProductosController> logger;
        private readonly IStorageApi storageApi;

        public ProductosController(MdtecnologiaContext MdTecnologiaContext, IStorageApi storageApi, LogsApi<ProductosController> logger)
        {
            this.MdTecnologiaContext = MdTecnologiaContext;
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
            var query = MdTecnologiaContext.Productos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(paginacionDto.Nombre))
                query = query.Where(p => EF.Functions.ILike(p.Nombre, $"%{paginacionDto.Nombre}%"));
            if (!string.IsNullOrWhiteSpace(paginacionDto.Marca))
                query = query.Where(p => EF.Functions.ILike(p.Marca, $"%{paginacionDto.Marca}%"));
            if (paginacionDto.Categoria != null)
                query = query.Where(p => p.Categoria == paginacionDto.Categoria);
            if (!string.IsNullOrWhiteSpace(paginacionDto.OrderBy))
            {
                var orderBy = paginacionDto.OrderBy.Split("-");
                var orderByProperty = orderBy[0];
                var orderByDirection = orderBy[1].Equals("DESC", StringComparison.OrdinalIgnoreCase);
                var property = typeof(Producto).GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    query = orderByDirection
                        ? query.OrderByDescending(e => EF.Property<object>(e, property.Name))
                        : query.OrderBy(e => EF.Property<object>(e, property.Name));
                }
                else
                    return BadRequest(new { OrderBy = "no se ha podido determinar la columna para ordernar sus resultados, formato esperado es 'columna-desc'" });
            }
            else
                query = query.OrderBy(p => p.Id);

            var totalProducts = await MdTecnologiaContext.Productos.CountAsync();
            var hasNextPage = (paginacionDto.Page + 1) * paginacionDto.Limit < totalProducts;
            // Calcula la página anterior que contiene resultados
            int previousPage = paginacionDto.Page - 1;
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
            var p = await MdTecnologiaContext.Productos
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
            var producto = await MdTecnologiaContext.Productos.FindAsync(id);
            if (producto == null)
                return NotFound(new { id = "producto no encontrado" });
            // Campos obligatorios tipo string
            if (string.IsNullOrWhiteSpace(productoDto.Nombre) || string.IsNullOrWhiteSpace(productoDto.Marca))
                return BadRequest(new { fieldsString = "campos como Nombre o Marca son requeridos" });
            // Descripción no es null se valida tipo string
            if (productoDto.Descripcion != null && string.IsNullOrWhiteSpace(productoDto.Descripcion))
                return BadRequest(new { descripcion = "la descripción no puede ser espacios en blanco" });
            if (productoDto.Categoria != null && await MdTecnologiaContext.Categorias.FindAsync(productoDto.Categoria) == null)
                return NotFound(new { categoria = "categoria de producto no encontrada" });

            using var transaction = await MdTecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                producto.Nombre = productoDto.Nombre;
                producto.Marca = productoDto.Marca;
                producto.Categoria = productoDto.Categoria;
                producto.Descripcion = productoDto.Descripcion;
                await transaction.CreateSavepointAsync("producto");

                if (productoDto.Proveedores != null && productoDto.Proveedores.Count != 0)
                {
                    try
                    {
                        foreach (var proveedorDto in productoDto.Proveedores)
                        {
                            var proveedor = await MdTecnologiaContext.Proveedores.FindAsync(proveedorDto.Proveedor);
                            if (proveedor == null)
                            {
                                logger.Advertencia($"Se ha omitido la relación producto - proveedor con Id {proveedorDto.Proveedor} no encontrado, precio = {proveedorDto.Precio} + impuesto = {proveedorDto.Impuesto}");
                                continue;
                            }
                            var relacion = await MdTecnologiaContext.ProductosProveedores.FirstOrDefaultAsync(pp => pp.Producto == id && pp.Proveedor == proveedorDto.Proveedor);

                            if (relacion != null)
                            {
                                // Actualizar la relación existente
                                relacion.Precio = proveedorDto.Precio;
                                relacion.Impuesto = proveedorDto.Impuesto;
                                relacion.Total = proveedorDto.Precio + proveedorDto.Impuesto;
                                relacion.FechaActualizado = proveedorDto.FechaActualizado != null ? (LocalDate)proveedorDto.FechaActualizado : LocalDate.FromDateTime(DateTime.UtcNow);
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
                                    FechaActualizado = proveedorDto.FechaActualizado != null ? (LocalDate)proveedorDto.FechaActualizado : LocalDate.FromDateTime(DateTime.UtcNow),
                                    Stock = proveedorDto.Stock
                                };
                                await MdTecnologiaContext.ProductosProveedores.AddAsync(productoProveedor);
                            }
                        }
                        await MdTecnologiaContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.Excepciones(ex, "Error al actualizar relacionaciones producto - proveedor");
                        await transaction.RollbackToSavepointAsync("producto");
                    }
                }
                await transaction.CreateSavepointAsync("proveedores");

                if (productoDto.Imagenes != null && productoDto.Imagenes.Count > 0)
                {
                    try
                    {
                        logger.Informacion("Relaciones de imagenes producto encontradas. Procesando...");
                        foreach (var imagenDto in productoDto.Imagenes)
                        {
                            if (!string.IsNullOrWhiteSpace(imagenDto.Url) && IsValidUrl(imagenDto.Url))
                            {
                                if (imagenDto.Descripcion != null && string.IsNullOrEmpty(imagenDto.Descripcion))
                                {
                                    logger.Advertencia($"Colocando null a descripción inválida, url: {imagenDto.Url}");
                                    imagenDto.Descripcion = null;
                                }
                                var imagenProducto = new ImagenesProducto()
                                {
                                    Producto = producto.Id,
                                    Url = imagenDto.Url,
                                    Descripcion = imagenDto.Descripcion,
                                };
                                await MdTecnologiaContext.ImagenesProductos.AddAsync(imagenProducto);
                            }
                            else
                            {
                                logger.Advertencia($"Omitiendo una url no válida la imagen: {imagenDto.Url}");
                                continue;
                            }
                        }
                        await MdTecnologiaContext.SaveChangesAsync();
                    }
                    catch (Exception excep)
                    {
                        logger.Excepciones(excep, "ha ocurrido un error al guardar las imagenes del producto");
                        await transaction.RollbackToSavepointAsync("proveedores");
                    }
                }

                await transaction.CommitAsync();
                logger.Informacion($"Se ha actualizado el producto {producto.Id}");
                await MdTecnologiaContext.Entry(producto).ReloadAsync();
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
            if (productoDto.Id != null && await MdTecnologiaContext.Productos.FindAsync(productoDto.Id) != null)
                return BadRequest(new { Id = "id proporcionado en uso" });
            // Campos obligatorios tipo string
            if (string.IsNullOrWhiteSpace(productoDto.Nombre) || string.IsNullOrWhiteSpace(productoDto.Marca))
                return BadRequest(new { fieldsString = "campos como Nombre o Marca son requeridos" });
            // Descripción no es null se valida tipo string
            if (productoDto.Descripcion != null && string.IsNullOrWhiteSpace(productoDto.Descripcion))
                return BadRequest(new { descripcion = "la descripción no puede ser espacios en blanco" });
            if (productoDto.Categoria != null && await MdTecnologiaContext.Categorias.FindAsync(productoDto.Categoria) == null)
                return NotFound(new { categoria = "categoria de producto no encontrada" });

            using var transaction = await MdTecnologiaContext.Database.BeginTransactionAsync();
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

                await MdTecnologiaContext.Productos.AddAsync(productos);
                await MdTecnologiaContext.SaveChangesAsync();
                await transaction.CreateSavepointAsync("producto");

                if (productoDto.Proveedores != null && productoDto.Proveedores.Count > 0)
                {
                    try
                    {
                        logger.Informacion("Relaciones de proveedores encontradas, procesando...");
                        foreach (var proveedorDto in productoDto.Proveedores)
                        {
                            if (await MdTecnologiaContext.Proveedores.FindAsync(proveedorDto.Proveedor) == null)
                            {
                                logger.Advertencia($"Se ha omitido la relación producto - proveedor con Id {proveedorDto.Proveedor} no encontrado, precio = {proveedorDto.Precio} + impuesto = {proveedorDto.Impuesto}");
                                continue;
                            }
                            var productoProveedor = new ProductosProveedor()
                            {
                                Producto = productos.Id,
                                Proveedor = proveedorDto.Proveedor,
                                Precio = proveedorDto.Precio,
                                Impuesto = proveedorDto.Impuesto,
                                Total = proveedorDto.Precio + proveedorDto.Impuesto,
                                FechaActualizado = proveedorDto.FechaActualizado != null ? (LocalDate)proveedorDto.FechaActualizado : LocalDate.FromDateTime(DateTime.UtcNow),
                                Stock = proveedorDto.Stock
                            };
                            await MdTecnologiaContext.ProductosProveedores.AddAsync(productoProveedor);
                        }
                        await MdTecnologiaContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.Excepciones(ex, "Excepción al crear producto relacionado a proveedor, revirtiendo a savepoint");
                        await transaction.RollbackToSavepointAsync("producto");
                    }
                }

                await transaction.CreateSavepointAsync("proveedores");

                if (productoDto.Imagenes != null && productoDto.Imagenes.Count > 0)
                {
                    try
                    {
                        logger.Informacion("Relaciones de imagenes producto encontradas. Procesando...");
                        foreach (var imagenDto in productoDto.Imagenes)
                        {
                            if (!string.IsNullOrWhiteSpace(imagenDto.Url) && IsValidUrl(imagenDto.Url))
                            {
                                if (imagenDto.Descripcion != null && string.IsNullOrEmpty(imagenDto.Descripcion))
                                {
                                    logger.Advertencia($"Colocando null a descripción inválida, url: {imagenDto.Url}");
                                    imagenDto.Descripcion = null;
                                }
                                var imagenProducto = new ImagenesProducto()
                                {
                                    Producto = productos.Id,
                                    Url = imagenDto.Url,
                                    Descripcion = imagenDto.Descripcion,
                                };
                                await MdTecnologiaContext.ImagenesProductos.AddAsync(imagenProducto);
                            }
                            else
                            {
                                logger.Advertencia($"Omitiendo una url no válida la imagen: {imagenDto.Url}");
                                continue;
                            }
                        }
                        await MdTecnologiaContext.SaveChangesAsync();
                    }
                    catch (Exception excep)
                    {
                        logger.Excepciones(excep, "ha ocurrido un error al guardar las imagenes del producto");
                        await transaction.RollbackToSavepointAsync("proveedores");
                    }
                }

                await transaction.CommitAsync();
                await MdTecnologiaContext.Entry(productos).ReloadAsync();
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
            if (image == null || image.Length == 0)
                return BadRequest(new { image = "No se ha proporcionado un archivo de imagen válido." });
            if (!image.ContentType.StartsWith("image/"))
                return BadRequest(new { image = "El archivo debe ser una imagen válida." });
            // 5 MB en bytes limitado por tema de rendimiento del fronted y costo de almacenamiento
            const long maxSizeInBytes = 5 * 1024 * 1024;
            if (image.Length > maxSizeInBytes)
                return BadRequest(new { image = "El archivo de imagen excede el tamaño máximo permitido de 5 MB." });

            var producto = await MdTecnologiaContext.Productos.FindAsync(id);
            if (producto != null)
            {
                var result = await storageApi.PutObjectAsync(new StorageApiDto()
                {
                    Name = image.FileName,
                    Type = image.ContentType,
                    Stream = image.OpenReadStream(),
                });
                if (result != null && result.Status && result.Url != null)
                {
                    producto.ImagenesProductos.Add(new ImagenesProducto()
                    {
                        Producto = producto.Id,
                        Descripcion = result.Name,
                        Url = result.Url.ToString(),
                    });
                    await MdTecnologiaContext.SaveChangesAsync();
                    return Accepted(result.Url);
                }
                else
                    return Problem();
            }
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Elimina un producto", Description = "Se elimina un producto de la base de datos")]
        [SwaggerResponse(204, "Producto eliminado")]
        [SwaggerResponse(404, "Producto no encontrado")]
        public async Task<ActionResult> DeleteProductos(Guid id)
        {
            var productos = await MdTecnologiaContext.Productos.FindAsync(id);
            if (productos == null)
                return NotFound();
            logger.Advertencia($"Se va a eliminar el producto con Id: {id} y nombre: {productos.Nombre}");
            MdTecnologiaContext.Productos.Remove(productos);
            await MdTecnologiaContext.SaveChangesAsync();

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
            var producto = await MdTecnologiaContext.Productos.Include(p => p.ImagenesProductos).FirstOrDefaultAsync(p => p.Id == id);
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
                MdTecnologiaContext.Remove(imagen);
                await MdTecnologiaContext.SaveChangesAsync();
                await MdTecnologiaContext.Entry(producto).ReloadAsync();
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

    }
}
