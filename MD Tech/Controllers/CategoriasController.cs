﻿using MD_Tech.Context;
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
        private readonly LogsApi logger;

        public CategoriasController(MdtecnologiaContext context)
        {
            this.context = context;
            logger = new LogsApi(GetType());
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
            if (string.IsNullOrWhiteSpace(categoriaDto.Nombre))
                return BadRequest(new { nombre = "El nombre de la categoría es obligatorio" });
            if (categoriaDto.Descripcion != null && string.IsNullOrWhiteSpace(categoriaDto.Descripcion))
                return BadRequest(new { descripcion = "La descripción es inválida. Puede ser null" });
            if (categoriaDto.CategoriaPadre != null)
            {
                if (!await context.Categorias.AnyAsync(c => c.Id == categoriaDto.CategoriaPadre))
                {
                    return BadRequest(new { categoriaPadre = "No existe la categoría padre a relacionar" });
                }
            }
            var categoria = new Categoria
            {
                Id = categoriaDto.Id ?? Guid.NewGuid(),
                Nombre = categoriaDto.Nombre,
                Descripcion = categoriaDto.Descripcion,
            };
            await context.Categorias.AddAsync(categoria);
            await context.SaveChangesAsync();
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
            var categoria = await context.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound();
            if (string.IsNullOrWhiteSpace(categoriaDto.Nombre))
                return BadRequest(new { mensaje = "El nombre de la categoría es obligatorio" });
            if (categoriaDto.Descripcion != null && string.IsNullOrWhiteSpace(categoriaDto.Descripcion))
                return BadRequest(new { descripcion = "La descripción es inválida. Puede ser null" });
            if (categoriaDto.CategoriaPadre != null)
            {
                if (!await context.Categorias.AnyAsync(c => c.Id == categoriaDto.CategoriaPadre))
                {
                    return BadRequest(new { categoriaPadre = "No existe la categoría padre a relacionar" });
                }
            }
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
                return NotFound(new { mensaje = "Categoría no encontrada" });

            context.Categorias.Remove(categoria);
            await context.SaveChangesAsync();
            logger.Advertencia($"Se ha eliminado la categoría {categoria.Nombre}");
            return NoContent();
        }
    }
}