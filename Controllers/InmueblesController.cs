namespace inmobiliaria.Controllers;

using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class InmueblesController : ControllerBase
{
    private readonly DataContext _context;

    public InmueblesController(DataContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("todos")] // Listo
    public async Task<IActionResult> Inmuebles()
    {
        try
        {
            var id = User.Claims.First(c => c.Type == "Id").Value;
            var inmuebles = await _context.Inmuebles
                .Where(x => x.PropietarioId == int.Parse(id))
                .Include(x => x.Propietario)
                .ToListAsync();

            inmuebles.ForEach(x => x.Propietario.Clave = "");
            return Ok(inmuebles);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("agregar")] // Listo
    public async Task<IActionResult> Agregar([FromForm] Inmueble inmueble)
    {
        try
        {
            var id = User.Claims.First(c => c.Type == "Id").Value;
            inmueble.PropietarioId = int.Parse(id);

            _context.Inmuebles.Add(inmueble);
            await _context.SaveChangesAsync();

            return Ok(inmueble.Id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("imagen")] // Listo
    public async Task<IActionResult> Imagen([FromForm] IFormFile imagen, [FromForm] int id)
    {
        try
        {
            var i = await _context.Inmuebles.FirstOrDefaultAsync(x => x.Id == id);

            if (i == null)
                return NotFound();

            if (imagen == null || imagen.Length == 0)
                return BadRequest("No se ha seleccionado un archivo");

            if (!string.IsNullOrEmpty(i.Imagen))
            {
                var pathOld = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "in", i.Imagen);
                if (System.IO.File.Exists(pathOld))
                {
                    System.IO.File.Delete(pathOld);
                }
            }

            var guid = Guid.NewGuid().ToString();
            var fileName = $"in{guid}{Path.GetExtension(imagen.FileName)}";
            var pathNew = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "in", fileName);

            using (var stream = new FileStream(pathNew, FileMode.Create))
                await imagen.CopyToAsync(stream);

            i.Imagen = fileName;
            _context.SaveChanges();
            return Ok("Imagen subida correctamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [Authorize]
    [HttpPut("disponible")] // Listo
    public async Task<IActionResult> Disponible([FromForm] int id)
    {
        try
        {
            var i = await _context.Inmuebles.FirstOrDefaultAsync(x => x.Id == id);

            if (i == null)
                return NotFound();

            i.Disponible = !i.Disponible;
            _context.SaveChanges();
            return Ok("Modificado correctamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}