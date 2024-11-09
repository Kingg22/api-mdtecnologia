using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MD_Tech.Contexts;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using MD_Tech.DTOs;
using Swashbuckle.AspNetCore.Annotations;

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
            logs = new LogsApi(typeof(ProductosController));
        }

        [HttpGet]
        public async Task<ActionResult> GetProductos()
        {
            return Ok(new
            {
                productos = await MdTecnologiaContext.Productos.Select(p => new ProductosDto()
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Marca = p.Marca,
                    Descripcion = p.Descripcion,
                    Categoria = p.Categoria,
                    Imagen1 = p.ImagenFile != null ? $"{Request.Scheme}://{Request.Host}{Url.Action(nameof(GetImage), "Productos", new { id = p.Id })}" : null,
                    Imagen2 = p.ImagenUrl,
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

        // TODO cambiar todo sobre imagenes a una tabla aparte con links de AWS S3
        [HttpGet("image/{id}")]
        public async Task<ActionResult> GetImage(Guid id)
        {
            var producto = await MdTecnologiaContext.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            else if (producto.ImagenFile != null)
            {
                // TODO cambiar por columna en la BD de tipo de imagen
                return File(producto.ImagenFile, "image/png");
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPost("image/{id}")]
        public async Task<ActionResult> UploadImage(Guid id, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest(new { message = "No se ha proporcionado un archivo de imagen válido." });
            }

            if (!imageFile.ContentType.StartsWith("image/"))
            {
                return BadRequest(new { message = "El archivo debe ser una imagen válida." });
            }

            var producto = await MdTecnologiaContext.Productos.FindAsync(id);

            if (producto != null)
            {
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();

                producto.ImagenFile = imageData;
                await MdTecnologiaContext.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductos(Guid id)
        {
            var p = await MdTecnologiaContext.Productos.FindAsync(id);

            if (p == null)
            {
                logs.Informacion($"Producto con id {id} a buscar no fue encontrado");
                return NotFound();
            }

            return Ok(new
            {
                producto = new ProductosDto()
                {
                    Id = id,
                    Nombre = p.Nombre,
                    Marca = p.Marca,
                    Descripcion = p.Descripcion,
                    Imagen1 = p.ImagenFile != null ? Url.Action(nameof(GetImage), "Productos", new { id = p.Id }, Request.Scheme) : null,
                    Imagen2 = p.ImagenUrl,
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
        public async Task<ActionResult> PutProductos(Guid id, [FromBody] ProductosDto productosDto)
        {
            if (id != productosDto.Id)
            {
                logs.Informacion($"Producto de id {id} no coincide con el body id {productosDto.Id}");
                return BadRequest();
            }
            var producto = await MdTecnologiaContext.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrWhiteSpace(productosDto.Imagen1))
            {
                return BadRequest(new { message = "para cambiar imagenes como archivo debe hacerlo en " + Url.Action(nameof(UploadImage), "Productos", new { id = productosDto.Id }, Request.Scheme) });
            }
            if (!string.IsNullOrWhiteSpace(productosDto.Imagen2) && IsValidUrl(productosDto.Imagen2))
            {
                using var transaction = await MdTecnologiaContext.Database.BeginTransactionAsync();
                try
                {
                    producto.Nombre = productosDto.Nombre;
                    producto.Marca = productosDto.Marca;
                    producto.Categoria = productosDto.Categoria;
                    producto.Descripcion = productosDto.Descripcion;
                    await transaction.CreateSavepointAsync("producto");

                    var listaProveedores = new List<ProductosProveedor>();
                    if (productosDto.Proveedores != null && productosDto.Proveedores.Count != 0)
                    {
                        try
                        {
                            foreach (var proveedorDto in productosDto.Proveedores)
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
                                    relacion.Total = proveedorDto.Precio * proveedorDto.Impuesto;
                                    relacion.FechaActualizado = proveedorDto.FechaActualizado != null ? (DateOnly)proveedorDto.FechaActualizado : DateOnly.FromDateTime(DateTime.UtcNow);
                                    relacion.Stock = proveedorDto.Stock;

                                    listaProveedores.Add(relacion);
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
                                        FechaActualizado = proveedorDto.FechaActualizado != null ? (DateOnly)proveedorDto.FechaActualizado : DateOnly.FromDateTime(DateTime.UtcNow),
                                        Stock = proveedorDto.Stock
                                    };
                                    await MdTecnologiaContext.ProductosProveedores.AddAsync(productoProveedor);

                                    listaProveedores.Add(productoProveedor);
                                }
                            }
                            await MdTecnologiaContext.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            logs.Excepciones(ex, "Error al actualizar relacionaciones producto - proveedor");
                            await transaction.RollbackToSavepointAsync("producto");
                            listaProveedores.Clear();
                            // Evaluar si eliminar toda lista de proveedores o volver a cargar la lista que existía en la BD
                            // antes de actualizar 
                        }
                    }
                    if (listaProveedores.Count > 0)
                    {
                        producto.ProductosProveedores = listaProveedores;
                        await MdTecnologiaContext.SaveChangesAsync();
                    }
                    return Ok(new
                    {
                        producto = new ProductosDto()
                        {
                            Nombre = producto.Nombre,
                            Marca = producto.Marca,
                            Descripcion = producto.Descripcion,
                            Imagen1 = producto.ImagenFile != null ? Url.Action(nameof(GetImage), "Productos", new { id = producto.Id }, Request.Scheme) : null,
                            Imagen2 = producto.ImagenUrl,
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
            else
            {
                return BadRequest(new { imagen2 = "ingrese una url para la imagen válida" });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostProductos([FromBody] ProductosDto productoDto)
        {
            logs.Informacion($"Iniciando la creación del producto con ID {productoDto.Id} y nombre {productoDto.Nombre}");
            if (productoDto.Id != null && await MdTecnologiaContext.Productos.FindAsync(productoDto.Id) != null)
            {
                return BadRequest(new { Id = "id proporcionado en uso" });
            }
            if (!string.IsNullOrWhiteSpace(productoDto.Imagen1) && !string.IsNullOrWhiteSpace(productoDto.Imagen2))
            {
                return BadRequest(new
                {
                    error = "Solo se soporta una imagen: Imagen1 como archivo o Imagen2 como URL. Deje uno de los campos como null. Para subir como archivo posterior a crear utilizar " +
                    Url.Action(nameof(UploadImage), "Productos", new { id = productoDto.Id }, Request.Scheme)
                });
            }
            if (!string.IsNullOrWhiteSpace(productoDto.Imagen1))
            {
                return BadRequest(new { message = "para imagenes como archivo debe hacerlo posterior a la creación en " + Url.Action(nameof(UploadImage), "Productos", new { id = productoDto.Id }, Request.Scheme) });
            }
            if (!string.IsNullOrWhiteSpace(productoDto.Nombre) || !string.IsNullOrWhiteSpace(productoDto.Marca) || !string.IsNullOrWhiteSpace(productoDto.Descripcion))
            {
                return BadRequest(new { fieldsString = "campos como Nombre o Marca son requeridos, mientras Descripcion no deben ser blanks ' '" });
            }
            if (!string.IsNullOrWhiteSpace(productoDto.Imagen2) && IsValidUrl(productoDto.Imagen2))
            {
                using var transaction = await MdTecnologiaContext.Database.BeginTransactionAsync();
                try
                {
                    var productos = new Productos()
                    {
                        Id = productoDto.Id ?? Guid.NewGuid(),
                        Nombre = productoDto.Nombre,
                        Marca = productoDto.Marca,
                        Descripcion = productoDto.Descripcion,
                        ImagenUrl = productoDto.Imagen2,
                    };

                    await MdTecnologiaContext.Productos.AddAsync(productos);
                    await MdTecnologiaContext.SaveChangesAsync();
                    transaction.CreateSavepoint("producto");

                    if (productoDto.Proveedores != null && productoDto.Proveedores.Count != 0)
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
                                    Total = proveedorDto.Precio * proveedorDto.Impuesto,
                                    FechaActualizado = proveedorDto.FechaActualizado != null ? (DateOnly)proveedorDto.FechaActualizado : DateOnly.FromDateTime(DateTime.UtcNow),
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
                    await transaction.CommitAsync();
                    return Created(Url.Action(nameof(GetProductos), "Productos", new { id = productos.Id }, Request.Scheme), new
                    {
                        producto = new ProductosDto()
                        {
                            Nombre = productos.Nombre,
                            Marca = productos.Marca,
                            Descripcion = productos.Descripcion,
                            Imagen1 = productos.ImagenFile != null ? Url.Action(nameof(GetImage), "Productos", new { id = productos.Id }, Request.Scheme) : null,
                            Imagen2 = productos.ImagenUrl,
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
            else
            {
                return BadRequest(new { imagen2 = "ingrese una url válida para la imagen" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
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

            return Ok();
        }

        [SwaggerIgnore]
        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

    }
}
