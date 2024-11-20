using MD_Tech.Context;
using MD_Tech.DTOs;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly MdtecnologiaContext mdtecnologiaContext;
        private readonly LogsApi<VentasController> logsApi;

        public VentasController(MdtecnologiaContext mdtecnologiaContext, LogsApi<VentasController> logsApi)
        {
            this.mdtecnologiaContext = mdtecnologiaContext;
            this.logsApi = logsApi;
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene todos las ventas", Description = "Devuelve una lista de ventas con sus detalles")]
        [SwaggerResponse(200, "Operación exitosa", typeof(List<Venta>))]
        public async Task<ActionResult<List<Venta>>> GetVenta()
        {
            return Ok(new { ventas = await mdtecnologiaContext.Ventas.Include(v => v.DetallesVenta).ToListAsync() });
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene una venta por ID", Description = "Devuelve la venta con su detalle")]
        [SwaggerResponse(200, "Operación exitosa", typeof(Venta))]
        [SwaggerResponse(404, "Venta no encontrado")]
        public async Task<ActionResult<Venta>> GetVenta(Guid id)
        {
            var ven = await mdtecnologiaContext.Ventas
              .Include(venta => venta.DetallesVenta)
              .FirstOrDefaultAsync(p => p.Id == id);
            return ven == null ? NotFound() : Ok(new { venta = ven });
        }

        [HttpGet("Detalles/{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene los detalles por ID venta", Description = "Devuelve una lista de detalles de la venta")]
        [SwaggerResponse(200, "Operación exitosa", typeof(List<DetalleVenta>))]
        [SwaggerResponse(404, "Venta no encontrado o sin detalles")]
        public async Task<ActionResult<List<DetalleVenta>>> GetDetalles(Guid id)
        {
            var detalles = await mdtecnologiaContext.DetallesVentas
                .Where(d => d.Venta == id)
                .ToListAsync();
            if (detalles == null || detalles.Count == 0)
                return NotFound();
            return Ok(detalles);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Crea un venta con sus detalles", 
                          Description = "Agrega una nueva venta a la base de datos. Si no se dan valores, se determina el mejor dato almacenado en la base de datos")]
        [SwaggerResponse(201, "Venta creada", typeof(Venta))]
        [SwaggerResponseHeader(201, "location", "string", "Enlace al recurso creado")]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(404, "No se han encontrado datos necesarios")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult<Venta>> AddVenta(VentaDto ventaDto)
        {
            using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                var cliente = await mdtecnologiaContext.Clientes
                    .Include(c => c.Direcciones)
                    .FirstOrDefaultAsync(c => c.Id == ventaDto.Cliente);
                if (cliente == null)
                    return NotFound(new { Cliente = "Cliente No Existe" });
                if (cliente.Direcciones.Count == 0)
                {
                    logsApi.Errores($"No tiene direcciones registradas el cliente {cliente.Id} para asignarle a su venta");
                    return NotFound(new { DireccionEntrega = "el cliente no tiene direcciones registradas" });
                }
                if (ventaDto.DireccionEntrega == null)
                    ventaDto.DireccionEntrega = cliente.Direcciones.First().Id;
                else if (!cliente.Direcciones.Any(d => d.Id == ventaDto.DireccionEntrega))
                    return BadRequest(new { Direccion_Entrega = "La Direccion ingresada No esta Registrada Por el Cliente" });

                var venta = new Venta()
                {
                    Cliente = ventaDto.Cliente,
                    Estado = ventaDto.Estado.ToString(),
                    Fecha = ventaDto.Fecha ?? LocalDateTime.FromDateTime(DateTime.UtcNow),
                    DireccionEntrega = (Guid)ventaDto.DireccionEntrega,
                };
                await mdtecnologiaContext.Ventas.AddAsync(venta);
                await mdtecnologiaContext.SaveChangesAsync();

                if (ventaDto.Detalles.Count > 0)
                {
                    foreach (var detalle in ventaDto.Detalles)
                    {
                        if ((detalle.Cantidad < 1) || (detalle.PrecioUnitario.HasValue && detalle.PrecioUnitario < 0) || (detalle.Descuento.HasValue && detalle.Descuento < 0) || (ventaDto.Subtotal.HasValue && ventaDto.Subtotal < 0))
                        {
                            logsApi.Errores($"Valores negativos en el detalle del producto {detalle.Producto}");
                            await transaction.RollbackAsync();
                            return BadRequest(new { Detalle_Numeros = $"Valor inválido para el producto {detalle.Producto}. Los números no pueden ser negativos" });
                        }

                        var producto = await mdtecnologiaContext.Productos
                            .Include(p => p.ProductosProveedores)
                            .FirstOrDefaultAsync(p=> p.Id==detalle.Producto);
                        if (producto == null)
                        {
                            logsApi.Errores("El Producto no esta Registrado para la venta");
                            await transaction.RollbackAsync();
                            return NotFound(new { Detalle_Producto = $"Producto Invalido ID: {detalle.Producto}" });
                        }
                        if (producto.ProductosProveedores.Count == 0)
                        {
                            logsApi.Errores("El producto no tiene proveedores para realizar la venta");
                            await transaction.RollbackAsync();
                            return NotFound(new { Detalle_Producto_Proveedores = "El producto no tiene registrado proveedores" });
                        }
                        var producto_menor_precio = producto.ProductosProveedores.MinBy(pp => pp.Total);
                        if (producto_menor_precio == null)
                        {
                            logsApi.Errores($"No se pudo determinar el proveedor con menor precio para el producto {producto.Id}");
                            await transaction.RollbackAsync();
                            return NotFound(new { Detalle_Producto_Proveedor = "No se pudo encontrar el mejor proveedor para esta venta" });
                        }
                        var precio = detalle.PrecioUnitario ?? producto_menor_precio.Precio;
                        var sub = precio * detalle.Cantidad;
                        var impuesto = detalle.Impuesto ?? producto_menor_precio.Impuesto;
                        var descuento = detalle.Descuento ?? decimal.Zero;
                        var total = Math.Round(sub + impuesto - descuento, 2);
                        
                        var detalleVenta = new DetalleVenta()
                        {
                            Venta = venta.Id,
                            Cantidad = detalle.Cantidad,
                            PrecioUnitario = precio,
                            Subtotal = sub,
                            Descuento = descuento,
                            Impuesto = impuesto,
                            Total = total,
                            Producto = detalle.Producto,
                        };
                        await mdtecnologiaContext.DetallesVentas.AddAsync(detalleVenta);
                    }
                    await mdtecnologiaContext.SaveChangesAsync();
                }
                await mdtecnologiaContext.Entry(venta).ReloadAsync();
                await mdtecnologiaContext.Entry(venta).Collection(v => v.DetallesVenta).LoadAsync();
                // Se hace la suma de todos los detalles para el recibo final de venta
                venta.CantidadTotalProductos = venta.DetallesVenta.Sum(d => d.Cantidad);
                venta.Subtotal = venta.DetallesVenta.Sum(d => d.Subtotal);
                venta.Descuento = venta.DetallesVenta.Sum(d => d.Descuento);
                venta.Impuesto = venta.DetallesVenta.Sum(d => d.Impuesto);
                venta.Total = venta.DetallesVenta.Sum(d => d.Total);
                if (venta.Total != Math.Round(venta.Subtotal + venta.Impuesto - venta.Descuento, 2))
                {
                    logsApi.Errores("El total calculado no coincide con la suma de todos los detalles");
                    await transaction.RollbackAsync();
                    return Problem("Errores de cálculos para la factura");
                }

                await mdtecnologiaContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Created(Url.Action(nameof(GetVenta), "Ventas", new { id = venta.Id }, Request.Scheme), new { venta });
            }
            catch (Exception ex)
            {
                logsApi.Excepciones(ex, "Error al crear la venta");
                await transaction.RollbackAsync();
                return Problem();
            }
        }

        [HttpPatch("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Cambiar el estado de la venta", Description = "Se actualizan el estado de la venta en la base de datos")]
        [SwaggerResponse(200, "Venta actualizada", typeof(Venta))]
        [SwaggerResponse(404, "Venta no encontrada")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult<Venta>> UpdateEstadoVenta(Guid id, [FromQuery] EstadoVentaEnum estado)
        {
            try
            {
                var venta = await mdtecnologiaContext.Ventas.FindAsync(id);
                if (venta == null)
                {
                    logsApi.Errores("venta no encontrada.");
                    return NotFound();
                }
                venta.Estado = estado.ToString();

                await mdtecnologiaContext.SaveChangesAsync();
                return Ok(new { venta });
            }
            catch (Exception ex)
            {
                logsApi.Excepciones(ex, "Error al actualizar el estado de la venta");
                return Problem();
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Elimina una venta", Description = "Se elimina una venta y sus detalles de la base de datos")]
        [SwaggerResponse(204, "Venta eliminada")]
        [SwaggerResponse(404, "Venta no encontrada")]
        public async Task<ActionResult> DeleteVenta(Guid id)
        {
            try
            {
                var venta = await mdtecnologiaContext.Ventas.FindAsync(id);
                if (venta == null)
                {
                    logsApi.Informacion("La Venta no existe");
                    return NotFound();
                }

                mdtecnologiaContext.Ventas.Remove(venta);
                await mdtecnologiaContext.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                logsApi.Excepciones(ex, "error al eliminar una venta");
                return Problem();
            }
        }

    }
}
