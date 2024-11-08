namespace inmobiliaria.Models;

public class Inmueble
{
    public int Id { get; set; }
    public string Direccion { get; set; } = "";
    public int Ambientes { get; set; }
    public string Tipo { get; set; } = "";
    public string Uso { get; set; } = "";
    public int Precio { get; set; }
    public bool Disponible { get; set; }
    public string Imagen { get; set; } = "";	
    public int PropietarioId { get; set; }
    public Propietario? Propietario { get; set; }
}