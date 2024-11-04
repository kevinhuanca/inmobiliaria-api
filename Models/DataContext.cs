using inmobiliaria.Models;
using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public DbSet<Propietario> Propietarios { get; set; }
    public DbSet<Inmueble> Inmuebles { get; set; }
}