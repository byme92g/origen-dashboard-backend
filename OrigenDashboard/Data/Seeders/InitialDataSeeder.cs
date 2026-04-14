using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Models.Entities;
using BC = BCrypt.Net.BCrypt;

namespace OrigenDashboard.Data.Seeders;

public interface IInitialDataSeeder
{
    Task SeedAsync(AppDbContext db);
}

public class InitialDataSeeder : IInitialDataSeeder
{
    public async Task SeedAsync(AppDbContext db)
    {
        if (db.Empleados.Any() || db.Clientes.Any() || db.Servicios.Any())
            return; // Already seeded

        await SeedEmpleados(db);
        await SeedServicios(db);
        await SeedProductos(db);
        await SeedClientes(db);
        await SeedPaquetes(db);
        await SeedIngresos(db);
        await SeedEgresos(db);
    }

    private async Task SeedEmpleados(AppDbContext db)
    {
        var empleados = new[]
        {
            new Empleado { Nombre = "María García", Cargo = "Estilista Senior", ComisionPct = 22, UsuarioLogin = "maria", Activo = true, CreadoEn = DateTime.UtcNow },
            new Empleado { Nombre = "Juan Pérez", Cargo = "Barbero", ComisionPct = 25, UsuarioLogin = "juan", Activo = true, CreadoEn = DateTime.UtcNow },
            new Empleado { Nombre = "Laura Martínez", Cargo = "Masajista", ComisionPct = 30, UsuarioLogin = "laura", Activo = true, CreadoEn = DateTime.UtcNow },
            new Empleado { Nombre = "Carlos López", Cargo = "Recepcionista", ComisionPct = 0, UsuarioLogin = "carlos", Activo = true, CreadoEn = DateTime.UtcNow },
            new Empleado { Nombre = "Ana Rodríguez", Cargo = "Colorista", ComisionPct = 28, UsuarioLogin = "ana", Activo = true, CreadoEn = DateTime.UtcNow },
            new Empleado { Nombre = "Diego Sánchez", Cargo = "Estilista", ComisionPct = 20, UsuarioLogin = "diego", Activo = true, CreadoEn = DateTime.UtcNow },
            new Empleado { Nombre = "Rosa Fernández", Cargo = "Manicurista", ComisionPct = 18, UsuarioLogin = "rosa", Activo = true, CreadoEn = DateTime.UtcNow },
        };

        foreach (var e in empleados)
            db.Empleados.Add(e);

        await db.SaveChangesAsync();
    }

    private async Task SeedServicios(AppDbContext db)
    {
        var servicios = new[]
        {
            new Servicio { Nombre = "Corte de cabello", Categoria = "Barbería", Precio = 25.00m, DuracionMin = 30, ComisionPct = 20, Activo = true, CreadoEn = DateTime.UtcNow },
            new Servicio { Nombre = "Corte + Afeitado", Categoria = "Barbería", Precio = 35.00m, DuracionMin = 45, ComisionPct = 25, Activo = true, CreadoEn = DateTime.UtcNow },
            new Servicio { Nombre = "Tinte completo", Categoria = "Coloración", Precio = 60.00m, DuracionMin = 90, ComisionPct = 22, Activo = true, CreadoEn = DateTime.UtcNow },
            new Servicio { Nombre = "Tinte + corte", Categoria = "Coloración", Precio = 80.00m, DuracionMin = 120, ComisionPct = 22, Activo = true, CreadoEn = DateTime.UtcNow },
            new Servicio { Nombre = "Alisado permanente", Categoria = "Tratamientos", Precio = 90.00m, DuracionMin = 120, ComisionPct = 20, Activo = true, CreadoEn = DateTime.UtcNow },
            new Servicio { Nombre = "Tratamiento capilar", Categoria = "Tratamientos", Precio = 45.00m, DuracionMin = 45, ComisionPct = 20, Activo = true, CreadoEn = DateTime.UtcNow },
            new Servicio { Nombre = "Masaje relajante", Categoria = "Bienestar", Precio = 50.00m, DuracionMin = 60, ComisionPct = 30, Activo = true, CreadoEn = DateTime.UtcNow },
            new Servicio { Nombre = "Masaje descontracturante", Categoria = "Bienestar", Precio = 55.00m, DuracionMin = 60, ComisionPct = 30, Activo = true, CreadoEn = DateTime.UtcNow },
            new Servicio { Nombre = "Manicura", Categoria = "Manos", Precio = 20.00m, DuracionMin = 30, ComisionPct = 18, Activo = true, CreadoEn = DateTime.UtcNow },
            new Servicio { Nombre = "Manicura + gel", Categoria = "Manos", Precio = 30.00m, DuracionMin = 45, ComisionPct = 18, Activo = true, CreadoEn = DateTime.UtcNow },
            new Servicio { Nombre = "Pedicura", Categoria = "Pies", Precio = 25.00m, DuracionMin = 40, ComisionPct = 18, Activo = true, CreadoEn = DateTime.UtcNow },
            new Servicio { Nombre = "Pedicura + gel", Categoria = "Pies", Precio = 35.00m, DuracionMin = 50, ComisionPct = 18, Activo = true, CreadoEn = DateTime.UtcNow },
        };

        foreach (var s in servicios)
            db.Servicios.Add(s);

        await db.SaveChangesAsync();
    }

    private async Task SeedProductos(AppDbContext db)
    {
        var productos = new[]
        {
            new Producto { Nombre = "Shampoo profesional", Categoria = "Cuidado capilar", PrecioVenta = 18.00m, Stock = 50, Activo = true, CreadoEn = DateTime.UtcNow },
            new Producto { Nombre = "Acondicionador premium", Categoria = "Cuidado capilar", PrecioVenta = 20.00m, Stock = 45, Activo = true, CreadoEn = DateTime.UtcNow },
            new Producto { Nombre = "Mascarilla capilar", Categoria = "Tratamientos", PrecioVenta = 25.00m, Stock = 35, Activo = true, CreadoEn = DateTime.UtcNow },
            new Producto { Nombre = "Sérum capilar", Categoria = "Tratamientos", PrecioVenta = 32.00m, Stock = 25, Activo = true, CreadoEn = DateTime.UtcNow },
            new Producto { Nombre = "Tinte (coloración)", Categoria = "Coloración", PrecioVenta = 35.00m, Stock = 30, Activo = true, CreadoEn = DateTime.UtcNow },
            new Producto { Nombre = "Gel para cabello", Categoria = "Estilización", PrecioVenta = 12.00m, Stock = 60, Activo = true, CreadoEn = DateTime.UtcNow },
            new Producto { Nombre = "Pomada hold fuerte", Categoria = "Estilización", PrecioVenta = 15.00m, Stock = 40, Activo = true, CreadoEn = DateTime.UtcNow },
            new Producto { Nombre = "Spray fijador", Categoria = "Estilización", PrecioVenta = 10.00m, Stock = 80, Activo = true, CreadoEn = DateTime.UtcNow },
            new Producto { Nombre = "Aceite aromaterapia", Categoria = "Bienestar", PrecioVenta = 38.00m, Stock = 20, Activo = true, CreadoEn = DateTime.UtcNow },
            new Producto { Nombre = "Crema manos", Categoria = "Cuidado personal", PrecioVenta = 16.00m, Stock = 55, Activo = true, CreadoEn = DateTime.UtcNow },
        };

        foreach (var p in productos)
            db.Productos.Add(p);

        await db.SaveChangesAsync();
    }

    private async Task SeedClientes(AppDbContext db)
    {
        var clientes = new[]
        {
            new Cliente { Nombre = "Ana Rodríguez", Telefono = "987654321", Email = "ana@example.com", Observaciones = "Cabello rizado, sensible. Preferencia: Silvia", FechaRegistro = DateTime.UtcNow },
            new Cliente { Nombre = "Roberto Díaz", Telefono = "987654322", Email = "roberto@example.com", Observaciones = "Corte clásico cada 4 semanas. VIP", FechaRegistro = DateTime.UtcNow },
            new Cliente { Nombre = "Sofía Morales", Telefono = "987654323", Email = "sofia@example.com", Observaciones = "Viene para tratamientos. Alérgica a químicos fuertes", FechaRegistro = DateTime.UtcNow },
            new Cliente { Nombre = "Miguel Fernández", Telefono = "987654324", Email = "miguel@example.com", Observaciones = "Cliente frecuente. Descuento aplicado", FechaRegistro = DateTime.UtcNow },
            new Cliente { Nombre = "Carmen Soto", Telefono = "987654325", Email = "carmen@example.com", Observaciones = "Manicura cada semana. Viernes preferencia", FechaRegistro = DateTime.UtcNow },
            new Cliente { Nombre = "Pablo Gutiérrez", Telefono = "987654326", Email = "pablo@example.com", Observaciones = "Barbero, corte + afeitado", FechaRegistro = DateTime.UtcNow },
            new Cliente { Nombre = "Isabel García", Telefono = "987654327", Email = "isabel@example.com", Observaciones = "Colorista, tinte cada 6 semanas", FechaRegistro = DateTime.UtcNow },
            new Cliente { Nombre = "Fernando López", Telefono = "987654328", Email = "fernando@example.com", Observaciones = "Masajes regulares. Lunes y miércoles", FechaRegistro = DateTime.UtcNow },
        };

        foreach (var c in clientes)
            db.Clientes.Add(c);

        await db.SaveChangesAsync();
    }

    private async Task SeedPaquetes(AppDbContext db)
    {
        var paquetes_list = await db.Servicios.ToListAsync();
        var servicioIds = paquetes_list.Select(s => s.Id).ToList();

        var paquete1 = new Paquete { Nombre = "Pack Belleza Completa", Descripcion = "Corte + Tinte + Mascarilla", Precio = 120.00m, Descuento = 10.00m, Activo = true, CreadoEn = DateTime.UtcNow };
        var paquete2 = new Paquete { Nombre = "Pack Bienestar", Descripcion = "Masaje + Manicura + Pedicura", Precio = 90.00m, Descuento = 5.00m, Activo = true, CreadoEn = DateTime.UtcNow };

        db.Paquetes.Add(paquete1);
        db.Paquetes.Add(paquete2);
        await db.SaveChangesAsync();

        // Link services to packages
        if (servicioIds.Count >= 6)
        {
            db.PaqueteServicios.AddRange(
                new PaqueteServicio { PaqueteId = paquete1.Id, ServicioId = servicioIds[0] },
                new PaqueteServicio { PaqueteId = paquete1.Id, ServicioId = servicioIds[1] },
                new PaqueteServicio { PaqueteId = paquete1.Id, ServicioId = servicioIds[2] },
                new PaqueteServicio { PaqueteId = paquete2.Id, ServicioId = servicioIds[3] },
                new PaqueteServicio { PaqueteId = paquete2.Id, ServicioId = servicioIds[4] },
                new PaqueteServicio { PaqueteId = paquete2.Id, ServicioId = servicioIds[5] }
            );
            await db.SaveChangesAsync();
        }
    }

    private async Task SeedIngresos(AppDbContext db)
    {
        var empleados = await db.Empleados.ToListAsync();
        var clientes = await db.Clientes.ToListAsync();
        var servicios = await db.Servicios.ToListAsync();
        var productos = await db.Productos.ToListAsync();

        var random = new Random(42);
        var ingresos = new List<Ingreso>();

        // 50+ transacciones variadas entre los últimos 60 días
        for (int i = 0; i < 50; i++)
        {
            var fecha = DateTime.UtcNow.AddDays(-random.Next(0, 60));
            var empleado = empleados[random.Next(empleados.Count)];
            var cliente = clientes[random.Next(clientes.Count)];
            var tipoRandom = random.Next(0, 100);
            
            Ingreso ingreso;

            if (tipoRandom < 50) // 50% servicios
            {
                var servicio = servicios[random.Next(servicios.Count)];
                var descuentoEspecial = random.Next(0, 20);
                ingreso = new Ingreso
                {
                    Fecha = fecha,
                    Tipo = "servicio",
                    Monto = servicio.Precio,
                    Descuento = descuentoEspecial > 0 ? Math.Round(servicio.Precio * descuentoEspecial / 100m, 2) : 0m,
                    MetodoPago = new[] { "efectivo", "yape", "plin", "transferencia" }[random.Next(4)],
                    Comision = Math.Round(servicio.Precio * (servicio.ComisionPct / 100m), 2),
                    Observaciones = $"Servicio completado",
                    ClienteId = cliente.Id,
                    EmpleadoId = empleado.Id,
                    ServicioId = servicio.Id,
                    CreadoEn = DateTime.UtcNow
                };
            }
            else if (tipoRandom < 75) // 25% productos
            {
                var producto = productos[random.Next(productos.Count)];
                var cantidad = random.Next(1, 4);
                ingreso = new Ingreso
                {
                    Fecha = fecha,
                    Tipo = "producto",
                    Monto = producto.PrecioVenta * cantidad,
                    Descuento = 0m,
                    MetodoPago = new[] { "efectivo", "pos" }[random.Next(2)],
                    Comision = 0m,
                    Observaciones = $"{cantidad} unidades vendidas",
                    ClienteId = random.Next(0, 2) == 0 ? cliente.Id : null,
                    EmpleadoId = empleado.Id,
                    ProductoId = producto.Id,
                    CreadoEn = DateTime.UtcNow
                };
            }
            else // 25% personalizados
            {
                ingreso = new Ingreso
                {
                    Fecha = fecha,
                    Tipo = "personalizado",
                    Monto = Math.Round((decimal)(20 + random.Next(1, 60)), 2),
                    Descuento = 0m,
                    MetodoPago = new[] { "efectivo", "transferencia", "yape" }[random.Next(3)],
                    Comision = 0m,
                    Observaciones = "Ingreso sin categoría específica",
                    ClienteId = null,
                    EmpleadoId = empleado.Id,
                    CreadoEn = DateTime.UtcNow
                };
            }

            ingresos.Add(ingreso);
        }

        foreach (var ingreso in ingresos)
            db.Ingresos.Add(ingreso);

        await db.SaveChangesAsync();
    }

    private async Task SeedEgresos(AppDbContext db)
    {
        var random = new Random(43);
        var egresos = new List<Egreso>();

        var categoriasList = new[] { "suministros", "servicios", "salarios", "renta", "marketing", "mantenimiento", "otros" };
        var proveedorList = new[] { "Distribuidora Beauty Plus", "Insumos Profesionales Inc", "Empresa de Agua Local", "Electricidad Nacional", "Google Ads", "Facebook Ads", "Reparaciones Locales", "Proveedor de Equipos" };
        var descripcionesList = new[] { 
            "Compra de insumos y productos",
            "Pago de servicios",
            "Pago de salarios",
            "Pago de renta",
            "Gastos de marketing y publicidad",
            "Mantenimiento de equipos",
            "Gastos generales"
        };

        // 35+ gastos variados en los últimos 60 días
        for (int i = 0; i < 35; i++)
        {
            var fecha = DateTime.UtcNow.AddDays(-random.Next(0, 60));
            var categoria = categoriasList[random.Next(categoriasList.Length)];
            var proveedor = proveedorList[random.Next(proveedorList.Length)];
            var descripcion = descripcionesList[random.Next(descripcionesList.Length)];
            
            decimal monto = categoria switch
            {
                "suministros" => Math.Round((decimal)(15 + random.Next(5, 150)), 2),
                "servicios" => Math.Round((decimal)(25 + random.Next(10, 100)), 2),
                "salarios" => Math.Round((decimal)(1000 + random.Next(0, 800)), 2),
                "renta" => Math.Round((decimal)(2000 + random.Next(-50, 50)), 2),
                "marketing" => Math.Round((decimal)(30 + random.Next(20, 200)), 2),
                "mantenimiento" => Math.Round((decimal)(40 + random.Next(20, 150)), 2),
                _ => Math.Round((decimal)(10 + random.Next(5, 100)), 2)
            };

            var egreso = new Egreso
            {
                Fecha = fecha,
                Monto = monto,
                Categoria = categoria,
                Descripcion = descripcion,
                Proveedor = proveedor,
                Comprobante = $"REC-{DateTime.UtcNow.Year}{random.Next(1000, 9999)}",
                Observaciones = $"Gasto registrado automaticamente",
                CreadoEn = DateTime.UtcNow
            };

            egresos.Add(egreso);
        }

        foreach (var egreso in egresos)
            db.Egresos.Add(egreso);

        await db.SaveChangesAsync();
    }
}
