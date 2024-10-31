namespace inmobiliaria.Controllers;

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
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: loginView.Clave,
                salt: System.Text.Encoding.ASCII.GetBytes(_configuration["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));

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
                new Claim("FullName", p.Nombre + " " + p.Apellido),
                new Claim("Id", p.Id.ToString()),
                new Claim("Email", p.Email)
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
        return Ok(p);
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

}