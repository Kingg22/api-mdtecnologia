using MD_Tech.Context;
using MD_Tech.DTOs;
using MD_Tech.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<List<Venta>>> GetVentas()
        {
            return Ok(new { ventas = await mdtecnologiaContext.Ventas.Include(v => v.DetallesVenta).ToListAsync() });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Venta>> GetVenta(Guid id)
        {
            var ven = await mdtecnologiaContext.Ventas
              .Include(venta => venta.DetallesVenta)
              .FirstOrDefaultAsync(p => p.Id == id);
            return ven == null ? NotFound() : Ok(new { venta = ven });
        }

        [HttpGet("Detalles/{id}")]
        public async Task<ActionResult<List<DetalleVenta>>> GetDetalles(Guid id)
        {
            var detalles = await mdtecnologiaContext.DetallesVentas
                .Where(d => d.Venta == id)
                .ToListAsync();
            if (detalles == null || detalles.Count == 0)
                return NotFound(new { mensaje = "No se encontraron detalles para esta venta." });
            return Ok(detalles);
        }

        [HttpPost]
        public async Task<ActionResult> AddVenta(VentaDto ventaDto)
        {
            using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                if (ventaDto.Descuento < 0)
                {
                    logsApi.Errores("El descuento no puede ser negativo");
                    return BadRequest(new { Descuento = "Descuento inválido" });
                }
                if (ventaDto.CantidadTotalProductos < 0)
                {
                    logsApi.Errores("La cantidad de productos no puede ser negativa");
                    return BadRequest(new { CantidadProducto = "Cantidad inválida" });
                }
                if (ventaDto.Subtotal < 0)
                {
                    logsApi.Errores("El subtotal no puede ser negativo");
                    return BadRequest(new { Subtotal = "Subtotal inválido" });
                }
                var cliente = await mdtecnologiaContext.Clientes
                    .Include(c => c.Direcciones)
                    .FirstOrDefaultAsync(c => c.Id == ventaDto.Cliente);
                if (cliente == null)
                {
                    logsApi.Errores("El Cliente No existe");
                    return BadRequest(new { Cliente = "Cliente inválido" });
                }
                if (ventaDto.DireccionEntrega == null)
                {
                    if (cliente.Direcciones.Count == 0)
                    {
                        // No tiene direcciones registradas
                        return NotFound();
                    }
                    ventaDto.DireccionEntrega = cliente.Direcciones.FirstOrDefault()?.Id;
                }
                else if (!cliente.Direcciones.Any(d => d.Id == ventaDto.DireccionEntrega))
                    return BadRequest(new { Direccion_Entrega = "La Direccion ingresada No esta Registrada Por el Cliente" });

                var venta = new Venta()
                {
                    Cliente = ventaDto.Cliente,
                    Descuento = decimal.Zero,
                    Estado = ventaDto.Estado.ToString(),
                    CantidadTotalProductos = 0,
                    Subtotal = 0,
                    Total = 0,
                    DireccionEntrega = (Guid)ventaDto.DireccionEntrega,
                };

                await mdtecnologiaContext.Ventas.AddAsync(venta);
                await mdtecnologiaContext.SaveChangesAsync();

                if (ventaDto.Detalles.Count > 0)
                {
                    foreach (var detalle in ventaDto.Detalles)
                    {
                        if (detalle.PrecioUnitario < 0)
                        {
                            logsApi.Errores("El precio no puede ser negativo");
                            await transaction.RollbackAsync();
                            return BadRequest(new { precio = "Precio Invalido" });
                        }
                        
                        var producto = await mdtecnologiaContext.Productos.Include(p => p.ProductosProveedores ).FirstOrDefaultAsync(p=> p.Id==detalle.Producto);
                        if (producto == null)
                        {
                            logsApi.Errores("El Producto no esta Registrado");
                            await transaction.RollbackAsync();
                            return NotFound(new { DetalleProducto_Producto = $"Producto Invalido ID: {detalle.Producto}" });
                        }
                        if (producto.ProductosProveedores.Count == 0)
                        {
                            await transaction.RollbackAsync();
                            return NotFound(new { Producto = "El producto no tiene registrado proveedores" });
                        }
                        var precio = detalle.PrecioUnitario ?? producto.ProductosProveedores.MinBy(pp => pp.Total).Precio;
                        var sub = precio * detalle.Cantidad;
                        var detalleVenta = new DetalleVenta()
                        {
                            Venta = venta.Id,
                            Cantidad = detalle.Cantidad,
                            PrecioUnitario = precio,
                            Subtotal = sub,
                            Descuento = detalle.Descuento ?? decimal.Zero,
                            Impuesto = detalle.Impuesto ?? decimal.Zero,
                            Total = decimal.Zero,
                            Producto = detalle.Producto,
                        };
                        detalleVenta.Total = (detalleVenta.Subtotal - detalleVenta.Impuesto) - detalleVenta.Descuento;
                        
                        await mdtecnologiaContext.DetallesVentas.AddAsync(detalleVenta);
                    }
                    await mdtecnologiaContext.SaveChangesAsync();
                }

                await mdtecnologiaContext.Entry(venta).ReloadAsync();
                await mdtecnologiaContext.Entry(venta).Collection(v => v.DetallesVenta).LoadAsync();
                // Se hace la suma de todos los detalles para el recibo final de venta
                venta.CantidadTotalProductos = venta.DetallesVenta.Sum(d => d.Cantidad);
                venta.Subtotal = venta.DetallesVenta.Sum(d => d.Subtotal);
                venta.Impuesto = venta.DetallesVenta.Sum(d => d.Impuesto);
                venta.Total = venta.DetallesVenta.Sum(d => d.Total);

                await mdtecnologiaContext.SaveChangesAsync();
                await transaction.CommitAsync();
                // Se recarga por última vez con todos los cambios realizados
                await mdtecnologiaContext.Entry(venta).ReloadAsync();
                await mdtecnologiaContext.Entry(venta).Collection(v => v.DetallesVenta).LoadAsync();
                return Ok(new { venta });
            }
            catch (Exception ex)
            {
                logsApi.Excepciones(ex, "Error al crear la venta");
                await transaction.RollbackAsync();
                return Problem("Ocurrió un error al crear la venta");
            }
        }

        [HttpPatch("estado/{id}")]
        public async Task<ActionResult> UpdateEstadoVenta(Guid id, [FromBody] EstadoVentaEnum newEstado)
        {
            try
            {
                var venta = await mdtecnologiaContext.Ventas.FindAsync(id);
                if (venta == null)
                {
                    logsApi.Errores("venta no encontrada.");
                    return NotFound();
                }
                venta.Estado = newEstado.ToString();

                await mdtecnologiaContext.SaveChangesAsync();
                return Ok(new { Message = "El estado de la venta se actualizó con éxito.", EstadoActualizado = venta.Estado });
            }
            catch (Exception ex)
            {
                logsApi.Excepciones(ex, "Error al actualizar el estado de la venta");
                return Problem("Error al actualizar el estado de la venta");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletleVenta(Guid id)
        {
            try
            {
                var venta = await mdtecnologiaContext.Ventas.FindAsync(id);
                if (venta == null)
                {
                    logsApi.Informacion("La Venta no existe");
                    return NotFound(new { veta = "Venta no Registrada" });
                }

                mdtecnologiaContext.Ventas.Remove(venta);
                await mdtecnologiaContext.SaveChangesAsync();
                return Ok("Se elimino la Venta");
            }
            catch (Exception ex)
            {
                logsApi.Excepciones(ex, "error al eliminar una veta");
                return BadRequest(new { Deletle = ex });
            }
        }

    }
}
