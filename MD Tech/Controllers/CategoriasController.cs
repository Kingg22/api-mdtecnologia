using MD_Tech.Context;
using MD_Tech.DTOs;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly MdtecnologiaContext context;
        private readonly LogsApi<CategoriasController> logger;

        public CategoriasController(MdtecnologiaContext context, LogsApi<CategoriasController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Obtiene todas las categorías", Description = "Devuelve una lista de categorías")]
        [SwaggerResponse(200, "Operación exitosa", typeof(List<CategoriaDto>))]
        public async Task<ActionResult<List<CategoriaDto>>> GetCategorias()
        {
            return Ok(new
            {
                categorias = await context.Categorias
                .Select(c => new CategoriaDto(c))
                .ToListAsync()
            });
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtiene una categoría por ID", Description = "Devuelve el detalle de la categoría")]
        [SwaggerResponse(200, "Operación exitosa", typeof(CategoriaDto))]
        [SwaggerResponse(404, "Categoría no encontrada")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(Guid id)
        {
            var categoria = await context.Categorias.FindAsync(id);
            return categoria == null ? NotFound() : Ok(new { categoria = new CategoriaDto(categoria) });
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Crea una nueva categoría", Description = "Agrega una nueva categoría a la base de datos")]
        [SwaggerResponse(201, "Categoría creada", typeof(CategoriaDto))]
        [SwaggerResponseHeader(201, "location", "string", "Enlace al recurso creado")]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        public async Task<ActionResult<CategoriaDto>> PostCategoria([FromBody] CategoriaDto categoriaDto)
        {
            var result = await ValidarCategoria(categoriaDto);
            if (result != null)
                return result;
            var categoria = new Categoria
            {
                Id = categoriaDto.Id ?? Guid.NewGuid(),
                Nombre = categoriaDto.Nombre,
                Descripcion = categoriaDto.Descripcion,
                CategoriaPadre = categoriaDto.CategoriaPadre,
            };
            await context.Categorias.AddAsync(categoria);
            await context.SaveChangesAsync();
            logger.Informacion("Se ha credo una nueva categoría");
            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, new CategoriaDto(categoria));
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Actualiza una categoría", Description = "Actualiza los datos de una categoría existente, incluye su relación padre")]
        [SwaggerResponse(200, "Categoría actualizada", typeof(CategoriaDto))]
        [SwaggerResponse(404, "Categoría no encontrada")]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        public async Task<ActionResult<CategoriaDto>> PutCategoria(Guid id, [FromBody] CategoriaDto categoriaDto)
        {
            if (id != categoriaDto.Id)
                return BadRequest(new { id = "Los ID no son iguales" });
            var categoria = await context.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound();

            var result = await ValidarCategoria(categoriaDto);
            if (result != null)
                return result;

            categoria.Nombre = categoriaDto.Nombre;
            categoria.Descripcion = categoriaDto.Descripcion;
            categoria.CategoriaPadre = categoriaDto.CategoriaPadre;
            await context.SaveChangesAsync();

            return Ok(new CategoriaDto(categoria));
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Elimina una categoría", Description = "Elimina una categoría de la base de datos")]
        [SwaggerResponse(204, "Categoría eliminada")]
        [SwaggerResponse(404, "Categoría no encontrada")]
        public async Task<IActionResult> DeleteCategoria(Guid id)
        {
            var categoria = await context.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound();

            context.Categorias.Remove(categoria);
            await context.SaveChangesAsync();
            logger.Advertencia($"Se ha eliminado la categoría {categoria.Nombre}");
            return NoContent();
        }

        [SwaggerIgnore]
        private async Task<bool> HasCircularReference(Guid categoriaId, Guid? categoriaPadreId)
        {
            while (categoriaPadreId != null)
            {
                if (categoriaPadreId == categoriaId)
                    return true;

                var parentCategory = await context.Categorias.FindAsync(categoriaPadreId);

                if (parentCategory == null)
                    break;

                categoriaPadreId = parentCategory.CategoriaPadre;
            }

            return false;
        }

        [SwaggerIgnore]
        private async Task<ActionResult?> ValidarCategoria(CategoriaDto categoriaDto)
        {
            if (string.IsNullOrWhiteSpace(categoriaDto.Nombre))
                return BadRequest(new { nombre = "El nombre de la categoría es obligatorio" });
            if (categoriaDto.Descripcion != null && string.IsNullOrWhiteSpace(categoriaDto.Descripcion))
                return BadRequest(new { descripcion = "La descripción es inválida. Puede ser null" });
            if (categoriaDto.CategoriaPadre != null)
            {
                if (categoriaDto.Id == categoriaDto.CategoriaPadre)
                    return BadRequest(new { categoriaPadre = "La categoría padre no puede ser igual a si misma" });
                if (!await context.Categorias.AnyAsync(c => c.Id == categoriaDto.CategoriaPadre))
                    return BadRequest(new { categoriaPadre = "No existe la categoría padre a relacionar" });
                if (await HasCircularReference(categoriaDto.Id ?? Guid.NewGuid(), categoriaDto.CategoriaPadre))
                    return BadRequest(new { categoriaPadre = "La relación con esta categoría padre crea una referencia circular" });
            }

            return null;
        }
    }
}