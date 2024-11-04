namespace inmobiliaria.Controllers;

using System;
using System.IO;
using System.Net;
using System.Net.Mail;
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
            string hashed = HashPass(loginView.Clave);

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

            string hashedActual = HashPass(cambiarClaveView.Actual);

            if (hashedActual != p.Clave)
                return BadRequest("La clave actual es incorrecta");

            if (cambiarClaveView.Nueva != cambiarClaveView.Repetida)
                return BadRequest("Las claves no coinciden");

            string hashedNueva = HashPass(cambiarClaveView.Nueva);

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

    [AllowAnonymous]
    [HttpPost("email")] // Listo
    public async Task<IActionResult> Email([FromForm] string email)
    {
        try
        {
            var p = await _context.Propietarios.FirstOrDefaultAsync(x => x.Email == email);

            if (p == null)
                return BadRequest("El email no existe");

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
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credenciales
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var url = "http://192.168.0.14:5285/api/propietarios/token?access_token="+tokenString;

            string bodyMail = $@"
                    <style> 
                    * {{ 
                        font-family: 'Segoe UI',Helvetica,Arial 
                    }}
                    .btn {{ 
                        background-color: #2ea44f;
                        border: 1px solid rgba(27, 31, 35, .15);
                        color: #fff;
                        cursor: pointer;
                        font-weight: 600;
                        padding: 2px 10px;
                        text-decoration: none; 
                    }} 
                    </style>
                    <h2>Restablecer Contraseña</h2>
                    <p>Hola, {p.Nombre}</p>
                    <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.</p>
                    <p style='display: inline;'>Para continuar, haz clic en el siguiente enlace:</p>
                    <a href='{url}' class='btn'>Restablecer mi contraseña</a>
                    <p>Si no solicitaste este cambio, podes ignorar este correo.</p>
                    <p>Gracias,<br/>El equipo de soporte.</p>
                ";	

            SendMail("test@mailtrap.com", p.Email, "Restablecer contraseña", bodyMail);

            return Ok("Email enviado");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("token")] // Listo
    public async Task<IActionResult> Token([FromQuery] string access_token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var expiration = handler.ReadJwtToken(access_token).ValidTo;

            if (expiration < DateTime.UtcNow)
                return BadRequest("El link ya expiro!");

            string id = User.Claims.First(c => c.Type == "Id").Value;
            var p = await _context.Propietarios.FirstOrDefaultAsync(x => x.Id == int.Parse(id));

            if (p == null)
                return NotFound();

            Random rand = new Random(Environment.TickCount);
            string randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            string nuevaClave = "";
            for (int i = 0; i < 4; i++)
            {
                nuevaClave += randomChars[rand.Next(0, randomChars.Length)];
            }

            string bodyMail = $@"
                <style> 
                * {{
                    font-family: 'Segoe UI',Helvetica,Arial 
                }}
                .btn {{
                    background-color: #ffffff;
                    border: 1px solid rgba(27, 31, 35, .15);
                    color: #000000;
                    font-weight: 800;
                    padding: 2px 10px;
                    text-decoration: none; 
                }}
                </style>
                <h2>Tu nueva contraseña</h2
                <p>Hola, {p.Nombre}</p>
                <p>Se ha generado una nueva contraseña exitosamente.</p>
                <p>Tu nueva contraseña: <strong class='btn'>{nuevaClave}</strong></p>
                <p>Te recomendamos cambiar esta contraseña la próxima vez que inicies sesión para mantener la seguridad de tu cuenta.</p>
                <p>Gracias,<br/>El equipo de soporte.</p>
            ";

            SendMail("test@mailtrap.com", p.Email, "Tu nueva contraseña", bodyMail);

            p.Clave = HashPass(nuevaClave);
            _context.SaveChanges();

            return Ok("Clave restablecida! Revisa tu correo");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private string HashPass(string password)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: System.Text.Encoding.ASCII.GetBytes(_configuration["Salt"]),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 1000,
            numBytesRequested: 256 / 8));
    }

    private static void SendMail(string sender, string receiver, string subject, string body)
    {
        var client = new SmtpClient("sandbox.smtp.mailtrap.io", 587)
        {
            Credentials = new NetworkCredential("85f5bb955c6200", "988bd2d7fac119"),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(sender),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        message.To.Add(new MailAddress(receiver));
        client.SendMailAsync(message);
    }

}