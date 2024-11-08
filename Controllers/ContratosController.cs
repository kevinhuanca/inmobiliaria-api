using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace inmobiliaria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContratosController : ControllerBase
{
    private readonly DataContext _context;

    public ContratosController(DataContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("alquilado")] // Listo
    public async Task<IActionResult> Alquilado()
    {
        try
        {
            var id = User.Claims.First(c => c.Type == "Id").Value;
            var contratos = await _context.Contratos
                .Include(x => x.Inmueble)
                .Include(x => x.Inquilino)
                .Where(x => x.FechaFin >= DateTime.Now && x.FechaInicio <= DateTime.Now)
                .Where(x => x.Inmueble.PropietarioId == int.Parse(id))
                .ToListAsync();

            if (contratos == null || contratos.Count == 0)
                return NotFound("No hay inmuebles alquilados actualmente");

            return Ok(contratos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("pagos/{id}")] // Listo
    public async Task<IActionResult> Pagos(int id)
    {
        try
        {
            var pagos = await _context.Pagos
                .Where(x => x.ContratoId == id)
                .ToListAsync();

            if (pagos == null || pagos.Count == 0)
                return NotFound("No tiene pagos");

            return Ok(pagos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}