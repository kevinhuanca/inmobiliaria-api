namespace inmobiliaria.Models;

public class Pago
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public DateTime Fecha { get; set; }
    public int Importe { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }
}