using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MD_Tech.Contexts;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using MD_Tech.DTOs;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;
using NodaTime;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly MdtecnologiaContext MdTecnologiaContext;
        private readonly LogsApi logs;

        public ProductosController(MdtecnologiaContext MdTecnologiaContext)
        {
            this.MdTecnologiaContext = MdTecnologiaContext;
            logs = new LogsApi(GetType());
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Obtiene todos los productos", Description = "Devuelve una lista de productos")]
        [SwaggerResponse(200, "Operación exitosa", typeof(List<ProductosDto>))]
        public async Task<ActionResult<List<ProductosDto>>> GetProductos([FromQuery] ProductosRequestDto paginacionDto)
        {
            if (paginacionDto.Page < 0 || paginacionDto.Limit < 0)
            {
                return BadRequest(new { paginacion = "las números de paginación o límites no pueden ser negativos" });
            }
            var query = MdTecnologiaContext.Productos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(paginacionDto.Nombre))
            {
                query = query.Where(p => EF.Functions.ILike(p.Nombre, $"%{paginacionDto.Nombre}%"));
            }
            if (!string.IsNullOrWhiteSpace(paginacionDto.Marca))
            {
                query = query.Where(p => EF.Functions.ILike(p.Marca, $"%{paginacionDto.Marca}%"));
            }
            if (paginacionDto.Categoria != null)
            {
                query = query.Where(p => p.Categoria == paginacionDto.Categoria);
            }
            if (!string.IsNullOrWhiteSpace(paginacionDto.OrderBy))
            {
                var orderBy = paginacionDto.OrderBy.Split("-");
                var orderByProperty = orderBy[0];
                var orderByDirection = orderBy[1].Equals("DESC", StringComparison.OrdinalIgnoreCase);

                var property = typeof(Productos).GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    query = orderByDirection
                        ? query.OrderByDescending(e => EF.Property<object>(e, property.Name))
                        : query.OrderBy(e => EF.Property<object>(e, property.Name));
                }
                else
                {
                    return BadRequest(new { OrderBy = "no se ha podido determinar la columna para ordernar sus resultados, formato esperado es 'columna-desc'" });
                }
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }
            var totalProducts = await MdTecnologiaContext.Productos.CountAsync();
            var hasNextPage = (paginacionDto.Page + 1) * paginacionDto.Limit < totalProducts;
            // Calcula la página anterior que contiene resultados
            int previousPage = paginacionDto.Page - 1;
            while (previousPage > 0 && previousPage * paginacionDto.Limit >= totalProducts)
            {
                previousPage--;
            }

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
            logs.Depuracion(query.ToQueryString());
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
                .Select(p => new ProductosDto()
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Marca = p.Marca,
                    Descripcion = p.Descripcion,
                    Categoria = p.Categoria,
                    Imagenes = p.ImagenesProductos.Select(img => new ImagenesProductoDto()
                    {
                        Id = img.Id,
                        Url = img.Url,
                        Descripcion = img.Descripcion,
                        Producto = img.Producto,
                    }).ToList(),
                    Proveedores = p.ProductosProveedores.Select(pp => new ProductoProveedorDto()
                    {
                        Producto = pp.Producto,
                        Proveedor = pp.Proveedor,
                        Precio = pp.Precio,
                        Impuesto = pp.Impuesto,
                        Total = pp.Total,
                        FechaActualizado = pp.FechaActualizado,
                        Stock = pp.Stock,
                    }).ToList()
                }).AsNoTracking()
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
                .FirstOrDefaultAsync();
            return p == null ? NotFound() : Ok(new
            {
                producto = new ProductosDto()
                {
                    Id = id,
                    Nombre = p.Nombre,
                    Marca = p.Marca,
                    Descripcion = p.Descripcion,
                    Imagenes = p.ImagenesProductos.Select(img => new ImagenesProductoDto()
                    {
                        Id = img.Id,
                        Url = img.Url,
                        Descripcion = img.Descripcion,
                        Producto = img.Producto,
                    }).ToList(),
                    Proveedores = p.ProductosProveedores.Select(pp => new ProductoProveedorDto()
                    {
                        Producto = pp.Producto,
                        Proveedor = pp.Proveedor,
                        Precio = pp.Precio,
                        Impuesto = pp.Impuesto,
                        Total = pp.Total,
                        FechaActualizado = pp.FechaActualizado,
                        Stock = pp.Stock,
                    }).ToList()
                }
            });
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
                logs.Informacion($"Producto de id {id} no coincide con el body id {productoDto.Id}");
                return BadRequest();
            }
            var producto = await MdTecnologiaContext.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound(new { id = "producto no encontrado" });
            }
            // Campos obligatorios tipo string
            if (string.IsNullOrWhiteSpace(productoDto.Nombre) || string.IsNullOrWhiteSpace(productoDto.Marca))
            {
                return BadRequest(new { fieldsString = "campos como Nombre o Marca son requeridos" });
            }
            // Descripción no es null se valida tipo string
            if (productoDto.Descripcion != null && string.IsNullOrWhiteSpace(productoDto.Descripcion))
            {
                return BadRequest(new { descripcion = "la descripción no puede ser espacios en blanco" });
            }
            if (productoDto.Categoria != null && await MdTecnologiaContext.Categorias.FindAsync(productoDto.Categoria) == null)
            {
                return NotFound(new { categoria = "categoria de producto no encontrada" });
            }
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
                                logs.Advertencia($"Se ha omitido la relación producto - proveedor con Id {proveedorDto.Proveedor} no encontrado, precio = {proveedorDto.Precio} + impuesto = {proveedorDto.Impuesto}");
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
                        logs.Excepciones(ex, "Error al actualizar relacionaciones producto - proveedor");
                        await transaction.RollbackToSavepointAsync("producto");
                    }
                }
                await transaction.CreateSavepointAsync("proveedores");

                if (productoDto.Imagenes != null && productoDto.Imagenes.Count > 0)
                {
                    try
                    {
                        logs.Informacion("Relaciones de imagenes producto encontradas. Procesando...");
                        foreach (var imagenDto in productoDto.Imagenes)
                        {
                            if (!string.IsNullOrWhiteSpace(imagenDto.Url) && IsValidUrl(imagenDto.Url))
                            {
                                if (imagenDto.Descripcion != null && string.IsNullOrEmpty(imagenDto.Descripcion))
                                {
                                    logs.Advertencia($"Colocando null a descripción inválida, url: {imagenDto.Url}");
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
                                logs.Advertencia($"Omitiendo una url no válida la imagen: {imagenDto.Url}");
                                continue;
                            }
                        }
                        await MdTecnologiaContext.SaveChangesAsync();
                    }
                    catch (Exception excep)
                    {
                        logs.Excepciones(excep, "ha ocurrido un error al guardar las imagenes del producto");
                        await transaction.RollbackToSavepointAsync("proveedores");
                    }
                }

                await transaction.CommitAsync();

                await MdTecnologiaContext.Entry(producto).ReloadAsync();
                return Ok(new
                {
                    producto = new ProductosDto()
                    {
                        Id = producto.Id,
                        Nombre = producto.Nombre,
                        Marca = producto.Marca,
                        Descripcion = producto.Descripcion,
                        Imagenes = producto.ImagenesProductos.Select(img => new ImagenesProductoDto()
                        {
                            Id = img.Id,
                            Url = img.Url,
                            Descripcion = img.Descripcion,
                            Producto = img.Producto,
                        }).ToList(),
                        Proveedores = producto.ProductosProveedores.Select(pp => new ProductoProveedorDto()
                        {
                            Producto = pp.Producto,
                            Proveedor = pp.Proveedor,
                            Precio = pp.Precio,
                            Impuesto = pp.Impuesto,
                            Total = pp.Total,
                            FechaActualizado = pp.FechaActualizado,
                            Stock = pp.Stock,
                        }).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                logs.Excepciones(ex, "Error general al actualiza producto");
                await transaction.RollbackAsync();
                return Problem();
            }
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Crea un producto", Description = "Agrega un nuevo producto a la base de datos")]
        [SwaggerResponse(201, "Producto creado", typeof(ProductosDto))]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult<ProductosDto>> PostProductos([FromBody] ProductosDto productoDto)
        {
            logs.Informacion($"Iniciando la creación del producto con ID {productoDto.Id} y nombre {productoDto.Nombre}");
            if (productoDto.Id != null && await MdTecnologiaContext.Productos.FindAsync(productoDto.Id) != null)
            {
                return BadRequest(new { Id = "id proporcionado en uso" });
            }
            // Campos obligatorios tipo string
            if (string.IsNullOrWhiteSpace(productoDto.Nombre) || string.IsNullOrWhiteSpace(productoDto.Marca))
            {
                return BadRequest(new { fieldsString = "campos como Nombre o Marca son requeridos" });
            }
            // Descripción no es null se valida tipo string
            if (productoDto.Descripcion != null && string.IsNullOrWhiteSpace(productoDto.Descripcion))
            {
                return BadRequest(new { descripcion = "la descripción no puede ser espacios en blanco" });
            }
            if (productoDto.Categoria != null && await MdTecnologiaContext.Categorias.FindAsync(productoDto.Categoria) == null)
            {
                return NotFound(new { categoria = "categoria de producto no encontrada" });
            }
            using var transaction = await MdTecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                var productos = new Productos()
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
                        logs.Informacion("Relaciones de proveedores encontradas, procesando...");
                        foreach (var proveedorDto in productoDto.Proveedores)
                        {
                            if (await MdTecnologiaContext.Proveedores.FindAsync(proveedorDto.Proveedor) == null)
                            {
                                logs.Advertencia($"Se ha omitido la relación producto - proveedor con Id {proveedorDto.Proveedor} no encontrado, precio = {proveedorDto.Precio} + impuesto = {proveedorDto.Impuesto}");
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
                        logs.Excepciones(ex, "Excepción al crear producto relacionado a proveedor, revirtiendo a savepoint");
                        await transaction.RollbackToSavepointAsync("producto");
                    }
                }

                await transaction.CreateSavepointAsync("proveedores");

                if (productoDto.Imagenes != null && productoDto.Imagenes.Count > 0)
                {
                    try
                    {
                        logs.Informacion("Relaciones de imagenes producto encontradas. Procesando...");
                        foreach (var imagenDto in productoDto.Imagenes)
                        {
                            if (!string.IsNullOrWhiteSpace(imagenDto.Url) && IsValidUrl(imagenDto.Url))
                            {
                                if (imagenDto.Descripcion != null && string.IsNullOrEmpty(imagenDto.Descripcion))
                                {
                                    logs.Advertencia($"Colocando null a descripción inválida, url: {imagenDto.Url}");
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
                                logs.Advertencia($"Omitiendo una url no válida la imagen: {imagenDto.Url}");
                                continue;
                            }
                        }
                        await MdTecnologiaContext.SaveChangesAsync();
                    }
                    catch (Exception excep)
                    {
                        logs.Excepciones(excep, "ha ocurrido un error al guardar las imagenes del producto");
                        await transaction.RollbackToSavepointAsync("proveedores");
                    }
                }

                await transaction.CommitAsync();
                await MdTecnologiaContext.Entry(productos).ReloadAsync();
                return Created(Url.Action(nameof(GetProductos), "Productos", new { id = productos.Id }, Request.Scheme), new
                {
                    producto = new ProductosDto()
                    {
                        Id = productos.Id,
                        Nombre = productos.Nombre,
                        Marca = productos.Marca,
                        Descripcion = productos.Descripcion,
                        Imagenes = productos.ImagenesProductos.Select(img => new ImagenesProductoDto()
                        {
                            Id = img.Id,
                            Url = img.Url,
                            Descripcion = img.Descripcion,
                            Producto = img.Producto,
                        }).ToList(),
                        Proveedores = productos.ProductosProveedores.Select(pp => new ProductoProveedorDto()
                        {
                            Producto = pp.Producto,
                            Proveedor = pp.Proveedor,
                            Precio = pp.Precio,
                            Impuesto = pp.Impuesto,
                            Total = pp.Total,
                            FechaActualizado = pp.FechaActualizado,
                            Stock = pp.Stock,
                        }).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                logs.Excepciones(ex, "Excepción general al crear producto");
                await transaction.RollbackAsync();
                return Problem("Ha ocurrido un error al guardar en la base de datos, revisar si el producto solo fue creado");
            }
        }

        [HttpPost("image/{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Añade una imagen a un producto", Description = "Agrega una nueva imagen al servicio utilizado y guarda en la base de datos su referencia")]
        [SwaggerResponse(200, "Imagen guardada")]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(404, "Producto no encontrado")]
        public async Task<ActionResult> UploadImage(Guid id, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(new { message = "No se ha proporcionado un archivo de imagen válido." });
            }

            if (!image.ContentType.StartsWith("image/"))
            {
                return BadRequest(new { message = "El archivo debe ser una imagen válida." });
            }

            var producto = await MdTecnologiaContext.Productos.FindAsync(id);

            if (producto != null)
            {
                // TODO guardar usando el servicio IStorageApi y guardar esta referencia en la base de datos
                return NoContent();
            }
            else
            {
                return NotFound();
            }
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
            {
                return NotFound();
            }
            logs.Advertencia($"Se va a eliminar el producto con Id: {id} y nombre: {productos.Nombre}");
            MdTecnologiaContext.Productos.Remove(productos);
            await MdTecnologiaContext.SaveChangesAsync();

            return NoContent();
        }

        [SwaggerIgnore]
        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

    }
}
