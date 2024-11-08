namespace inmobiliaria.Models;

public class Contrato
{
    public int Id { get; set; }
    public int Monto { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int InquilinoId { get; set; }
    public int InmuebleId { get; set; }
    public Inquilino? Inquilino { get; set; }
    public Inmueble? Inmueble { get; set; }
}