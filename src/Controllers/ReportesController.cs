using Microsoft.AspNetCore.Mvc;
using System.Text;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

public class ReportesController(
    IIngresoRepository ingresoRepo,
    IEgresoRepository egresoRepo,
    IClienteRepository clienteRepo) : BaseController
{
    private sealed record ReporteData(
        DateTime Inicio, DateTime Fin, decimal TotalIngresos, decimal TotalEgresos,
        decimal TotalComisiones, int CantidadIngresos, int CantidadEgresos);

    private async Task<ReporteData> ObtenerReporteData(DateTime? desde, DateTime? hasta)
    {
        var ahora = DateTime.UtcNow;
        var inicio = desde ?? new DateTime(ahora.Year, ahora.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fin = hasta ?? ahora;
        var ingresos = (await ingresoRepo.ObtenerPorFechaAsync(inicio, fin)).ToList();
        var egresos = (await egresoRepo.ObtenerPorFechaAsync(inicio, fin)).ToList();

        return new ReporteData(
            inicio,
            fin,
            ingresos.Sum(i => i.Monto - i.Descuento),
            egresos.Sum(e => e.Monto),
            ingresos.Sum(i => i.Comision),
            ingresos.Count,
            egresos.Count);
    }

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

    [HttpGet("exportar/csv")]
    public async Task<IActionResult> ExportarCsv(DateTime? desde, DateTime? hasta)
    {
        var ahora = DateTime.UtcNow;
        var inicio = desde ?? new DateTime(ahora.Year, ahora.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fin = hasta ?? ahora;
        var ingresos = await ingresoRepo.ObtenerPorFechaAsync(inicio, fin);
        var egresos = await egresoRepo.ObtenerPorFechaAsync(inicio, fin);

        var sb = new StringBuilder();
        sb.AppendLine("Fecha,Tipo,Concepto,Monto,Descuento,Total,MetodoPago,Comision,Observaciones");
        foreach (var i in ingresos)
        {
            var concepto = i.Servicio?.Nombre ?? i.Producto?.Nombre ?? i.Paquete?.Nombre ?? i.ConceptoPersonalizado ?? "Ingreso";
            sb.AppendLine(ToCsvRow(i.Fecha, "Ingreso", concepto, i.Monto, i.Descuento, i.Monto - i.Descuento, i.MetodoPago, i.Comision, i.Observaciones));
        }
        foreach (var e in egresos)
            sb.AppendLine(ToCsvRow(e.Fecha, "Egreso", e.Descripcion, e.Monto, 0, -e.Monto, "", 0, e.Observaciones));

        return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray(),
            "text/csv; charset=utf-8",
            $"reporte-origen-{DateTime.UtcNow:yyyyMMddHHmm}.csv");
    }

    [HttpGet("exportar/pdf")]
    public async Task<IActionResult> ExportarPdf(DateTime? desde, DateTime? hasta)
    {
        var data = await ObtenerReporteData(desde, hasta);
        var lines = new[]
        {
            "Origen - Resumen financiero",
            $"Periodo: {data.Inicio:yyyy-MM-dd} a {data.Fin:yyyy-MM-dd}",
            $"Total ingresos: S/ {data.TotalIngresos:N2}",
            $"Total egresos: S/ {data.TotalEgresos:N2}",
            $"Utilidad neta: S/ {data.TotalIngresos - data.TotalEgresos:N2}",
            $"Total comisiones: S/ {data.TotalComisiones:N2}",
            $"Cantidad ingresos: {data.CantidadIngresos}",
            $"Cantidad egresos: {data.CantidadEgresos}"
        };

        return File(BuildSimplePdf(lines), "application/pdf", $"reporte-origen-{DateTime.UtcNow:yyyyMMddHHmm}.pdf");
    }

    private static string ToCsvRow(params object?[] values) =>
        string.Join(",", values.Select(v => $"\"{(v?.ToString() ?? "").Replace("\"", "\"\"")}\""));

    private static byte[] BuildSimplePdf(IEnumerable<string> lines)
    {
        static string Escape(string value) => value.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");

        var content = new StringBuilder("BT\n/F1 14 Tf\n50 780 Td\n");
        foreach (var line in lines)
            content.Append($"({Escape(line)}) Tj\n0 -24 Td\n");
        content.Append("ET");

        var objects = new List<string>
        {
            "1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n",
            "2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n",
            "3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>\nendobj\n",
            "4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n",
            $"5 0 obj\n<< /Length {Encoding.ASCII.GetByteCount(content.ToString())} >>\nstream\n{content}\nendstream\nendobj\n"
        };

        var pdf = new StringBuilder("%PDF-1.4\n");
        var offsets = new List<int> { 0 };
        foreach (var obj in objects)
        {
            offsets.Add(Encoding.ASCII.GetByteCount(pdf.ToString()));
            pdf.Append(obj);
        }

        var xref = Encoding.ASCII.GetByteCount(pdf.ToString());
        pdf.Append($"xref\n0 {objects.Count + 1}\n0000000000 65535 f \n");
        foreach (var offset in offsets.Skip(1))
            pdf.Append($"{offset:0000000000} 00000 n \n");
        pdf.Append($"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");
        return Encoding.ASCII.GetBytes(pdf.ToString());
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
