using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

public class ReportesController(
    IIngresoRepository ingresoRepo,
    IEgresoRepository egresoRepo,
    IClienteRepository clienteRepo) : BaseController
{
    [HttpGet("resumen")]
    public async Task<IActionResult> Resumen(DateTime? desde, DateTime? hasta)
    {
        var ahora = DateTime.UtcNow;
        var inicio = desde ?? new DateTime(ahora.Year, ahora.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fin = hasta ?? ahora;

        var ingresos = (await ingresoRepo.ObtenerPorFechaAsync(inicio, fin)).ToList();
        var egresos = (await egresoRepo.ObtenerPorFechaAsync(inicio, fin)).ToList();

        var totalIngresos = ingresos.Sum(i => i.Monto - i.Descuento);
        var totalEgresos = egresos.Sum(e => e.Monto);

        var porMetodoPago = ingresos
            .GroupBy(i => i.MetodoPago)
            .Select(g => new { metodo = g.Key, total = g.Sum(i => i.Monto - i.Descuento), cantidad = g.Count() })
            .OrderByDescending(x => x.total);

        var topServicios = ingresos
            .Where(i => i.Servicio is not null)
            .GroupBy(i => i.Servicio!.Nombre)
            .Select(g => new { servicio = g.Key, total = g.Sum(i => i.Monto - i.Descuento), cantidad = g.Count() })
            .OrderByDescending(x => x.cantidad)
            .Take(10);

        var porEmpleado = ingresos
            .Where(i => i.Empleado is not null)
            .GroupBy(i => i.Empleado!.Nombre)
            .Select(g => new
            {
                empleado = g.Key,
                totalVentas = g.Sum(i => i.Monto - i.Descuento),
                totalComision = g.Sum(i => i.Comision),
                servicios = g.Count()
            })
            .OrderByDescending(x => x.totalVentas);

        return ApiOk(new
        {
            periodo = new { desde = inicio, hasta = fin },
            resumen = new
            {
                totalIngresos,
                totalEgresos,
                utilidadNeta = totalIngresos - totalEgresos,
                totalComisiones = ingresos.Sum(i => i.Comision),
                cantidadIngresos = ingresos.Count,
                cantidadEgresos = egresos.Count
            },
            porMetodoPago,
            topServicios,
            porEmpleado
        });
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var hoy = DateTime.UtcNow.Date;
        var finHoy = hoy.AddDays(1).AddTicks(-1);
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var ingresosHoy = (await ingresoRepo.ObtenerPorFechaAsync(hoy, finHoy)).ToList();
        var egresosMes = (await egresoRepo.ObtenerPorFechaAsync(inicioMes, finHoy)).ToList();
        var ingresosMes = (await ingresoRepo.ObtenerPorFechaAsync(inicioMes, finHoy)).ToList();
        var totalClientes = (await clienteRepo.ObtenerTodosAsync()).Count();

        return ApiOk(new
        {
            kpis = new
            {
                ingresosHoy = ingresosHoy.Sum(i => i.Monto - i.Descuento),
                egresosMes = egresosMes.Sum(e => e.Monto),
                utilidadMes = ingresosMes.Sum(i => i.Monto - i.Descuento) - egresosMes.Sum(e => e.Monto),
                totalClientes,
                serviciosHoy = ingresosHoy.Count
            },
            ultimasTransacciones = ingresosHoy.Take(10).Select(i => new
            {
                i.Id,
                i.Fecha,
                cliente = i.Cliente?.Nombre ?? "—",
                concepto = i.Servicio?.Nombre ?? i.Producto?.Nombre ?? i.Paquete?.Nombre ?? i.ConceptoPersonalizado ?? "—",
                i.Monto,
                i.Descuento,
                i.MetodoPago
            }),
            porMetodoPago = ingresosHoy
                .GroupBy(i => i.MetodoPago)
                .Select(g => new { metodo = g.Key, total = g.Sum(i => i.Monto - i.Descuento) })
        });
    }
}
