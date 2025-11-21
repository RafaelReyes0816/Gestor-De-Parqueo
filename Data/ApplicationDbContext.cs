using Microsoft.EntityFrameworkCore;
using Control_de_Parqueo.Models;

namespace Control_de_Parqueo.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehiculo> Vehiculos { get; set; }
    public DbSet<RegistroParqueo> RegistrosParqueo { get; set; }
    public DbSet<Recompensa> Recompensas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de índices
        modelBuilder.Entity<Vehiculo>()
            .HasIndex(v => v.Placa)
            .IsUnique();

        modelBuilder.Entity<RegistroParqueo>()
            .HasIndex(r => r.VehiculoId);

        modelBuilder.Entity<RegistroParqueo>()
            .HasIndex(r => r.Estado);

        modelBuilder.Entity<Recompensa>()
            .HasIndex(r => r.VehiculoId);
    }
}

