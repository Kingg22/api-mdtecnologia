using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MD_Tech.Context;
using MD_Tech.Models;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialProductosController : ControllerBase
    {
        private readonly MdtecnologiaContext context;

        public HistorialProductosController(MdtecnologiaContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene el historial de precios de todos los productos", Description = "Devuelve una lista de historial productos")]
        [SwaggerResponse(200, "Operación exitosa", typeof(List<HistorialProducto>))]
        public async Task<ActionResult<List<HistorialProducto>>> GetHistorialProducto()
        {
            return Ok(new { historial = await context.HistorialProductos.ToListAsync() });
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene un historial detallado por ID", Description = "Devuelve el detalle del historial")]
        [SwaggerResponse(200, "Operación exitosa", typeof(HistorialProducto))]
        [SwaggerResponse(404, "Historial no encontrado")]
        public async Task<ActionResult<HistorialProducto>> GetHistorialProducto(Guid id)
        {
            var historialProducto = await context.HistorialProductos.FindAsync(id);
            return historialProducto == null ? NotFound() : historialProducto;
        }

        [HttpGet("producto/{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene el historial de un producto por ID", Description = "Devuelve una lista de historial producto")]
        [SwaggerResponse(200, "Operación exitosa", typeof(List<HistorialProducto>))]
        [SwaggerResponse(404, "Producto no encontrado")]
        public async Task<ActionResult<List<HistorialProducto>>> GetHistorialProductoByProducto(Guid id)
        {
            return await context.HistorialProductos.Where(h => h.Producto == id).ToListAsync();
        }
    }
}
