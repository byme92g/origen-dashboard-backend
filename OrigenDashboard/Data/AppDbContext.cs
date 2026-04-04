using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Paquete> Paquetes => Set<Paquete>();
    public DbSet<PaqueteServicio> PaqueteServicios => Set<PaqueteServicio>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Ingreso> Ingresos => Set<Ingreso>();
    public DbSet<Egreso> Egresos => Set<Egreso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasIndex(u => u.NombreUsuario).IsUnique();
            e.Property(u => u.Rol).HasMaxLength(20);
        });

        modelBuilder.Entity<Empleado>(e =>
        {
            e.Property(emp => emp.ComisionPct).HasPrecision(5, 2);
        });

        modelBuilder.Entity<Servicio>(e =>
        {
            e.Property(s => s.Precio).HasPrecision(10, 2);
            e.Property(s => s.ComisionPct).HasPrecision(5, 2);
        });

        modelBuilder.Entity<Producto>(e =>
        {
            e.Property(p => p.PrecioVenta).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Paquete>(e =>
        {
            e.Property(p => p.Precio).HasPrecision(10, 2);
            e.Property(p => p.Descuento).HasPrecision(10, 2);
        });

        modelBuilder.Entity<PaqueteServicio>(e =>
        {
            e.HasOne(ps => ps.Paquete).WithMany(p => p.Servicios).HasForeignKey(ps => ps.PaqueteId);
            e.HasOne(ps => ps.Servicio).WithMany().HasForeignKey(ps => ps.ServicioId);
        });

        modelBuilder.Entity<Ingreso>(e =>
        {
            e.Property(i => i.Monto).HasPrecision(10, 2);
            e.Property(i => i.Descuento).HasPrecision(10, 2);
            e.Property(i => i.Comision).HasPrecision(10, 2);
            e.HasOne(i => i.Cliente).WithMany().HasForeignKey(i => i.ClienteId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(i => i.Empleado).WithMany().HasForeignKey(i => i.EmpleadoId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(i => i.Servicio).WithMany().HasForeignKey(i => i.ServicioId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(i => i.Producto).WithMany().HasForeignKey(i => i.ProductoId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(i => i.Paquete).WithMany().HasForeignKey(i => i.PaqueteId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Egreso>(e =>
        {
            e.Property(eg => eg.Monto).HasPrecision(10, 2);
        });
    }
}
