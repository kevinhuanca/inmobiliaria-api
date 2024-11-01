namespace inmobiliaria.Controllers;

using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class PropietariosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public PropietariosController(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")] // Listo
    public async Task<IActionResult> Login([FromForm] LoginView loginView)
    {
        try
        {
            string hashed = Hash(loginView.Clave);

            var p = await _context.Propietarios.FirstOrDefaultAsync(x => x.Email == loginView.Email);

            if (p == null || p.Clave != hashed)
            {
                return BadRequest("Email o clave incorrectos.");
            }

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.ASCII.GetBytes(_configuration["TokenAuthentication:SecretKey"]));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim("Id", p.Id.ToString()),
                new Claim("FullName", p.Nombre + " " + p.Apellido)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["TokenAuthentication:Issuer"],
                audience: _configuration["TokenAuthentication:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credenciales
            );
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("perfil")] // Listo
    public async Task<IActionResult> Perfil()
    {
        string id = User.Claims.First(c => c.Type == "Id").Value;
        var p = await _context.Propietarios.FirstOrDefaultAsync(x => x.Id == int.Parse(id));

        if (p == null)
            return NotFound();
        
        p.Clave = "";
        return Ok(p);
    }

    [Authorize]
    [HttpPut("modificar")] // Listo
    public async Task<IActionResult> Modificar([FromForm] Propietario propietario)
    {
        try
        {
            string id = User.Claims.First(c => c.Type == "Id").Value;
            var p = await _context.Propietarios.FirstOrDefaultAsync(x => x.Id == int.Parse(id));

            if (p == null)
                return NotFound();

            var emailExiste = await _context.Propietarios
                .AnyAsync(x => x.Email == propietario.Email && x.Id != p.Id);

            if (emailExiste)
                return BadRequest("El email ya está en uso");

            p.Dni = propietario.Dni;
            p.Nombre = propietario.Nombre;
            p.Apellido = propietario.Apellido;
            p.Telefono = propietario.Telefono;
            p.Email = propietario.Email;

            _context.SaveChanges();
            return Ok("Modificado correctamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("clave")] // Listo
    public async Task<IActionResult> Clave([FromForm] CambiarClaveView cambiarClaveView)
    {
        try
        {
            string id = User.Claims.First(c => c.Type == "Id").Value;
            var p = await _context.Propietarios.FirstOrDefaultAsync(x => x.Id == int.Parse(id));

            if (p == null)
                return NotFound();

            string hashedActual = Hash(cambiarClaveView.Actual);

            if (hashedActual != p.Clave)
                return BadRequest("La clave actual es incorrecta");

            if (cambiarClaveView.Nueva != cambiarClaveView.Repetida)
                return BadRequest("Las claves no coinciden");

            string hashedNueva = Hash(cambiarClaveView.Nueva);

            p.Clave = hashedNueva;
            _context.SaveChanges();
            return Ok("Clave cambiada correctamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("avatar")] // Listo
    public async Task<IActionResult> Avatar([FromForm] IFormFile avatar)
    {
        try
        {
            string id = User.Claims.First(c => c.Type == "Id").Value;
            var p = await _context.Propietarios.FirstOrDefaultAsync(x => x.Id == int.Parse(id));

            if (p == null)
                return NotFound();

            if (avatar == null || avatar.Length == 0)
                return BadRequest("No se ha seleccionado un archivo");

            if (!string.IsNullOrEmpty(p.Avatar))
            {
                var pathOld = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", p.Avatar);
                if (System.IO.File.Exists(pathOld))
                {
                    System.IO.File.Delete(pathOld);
                }
            }

            var guid = Guid.NewGuid().ToString();
            var fileName = $"AV{guid}{Path.GetExtension(avatar.FileName)}";
            var pathNew = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);

            using (var stream = new FileStream(pathNew, FileMode.Create))
                await avatar.CopyToAsync(stream);

            p.Avatar = fileName;
            _context.SaveChanges();
            return Ok("Avatar cambiado correctamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }



    // [HttpPost("hashed")] // Para hashear claves
    // public IActionResult Hasheada()
    // {
    //     string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
    //         password: "123",
    //         salt: System.Text.Encoding.ASCII.GetBytes(_configuration["Salt"]),
    //         prf: KeyDerivationPrf.HMACSHA1,
    //         iterationCount: 1000,
    //         numBytesRequested: 256 / 8));

    //     return Ok(hashed);
    // }

    private string Hash(string password) // Para hashear claves
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: System.Text.Encoding.ASCII.GetBytes(_configuration["Salt"]),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 1000,
            numBytesRequested: 256 / 8));
    }

}