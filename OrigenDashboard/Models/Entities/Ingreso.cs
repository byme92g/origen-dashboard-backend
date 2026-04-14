namespace OrigenDashboard.Models.Entities;

public class Ingreso
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int? ClienteId { get; set; }
    public int? EmpleadoId { get; set; }
    public string Tipo { get; set; } = string.Empty; // servicio | producto | paquete | personalizado
    public int? ServicioId { get; set; }
    public int? ProductoId { get; set; }
    public int? PaqueteId { get; set; }
    public string? ConceptoPersonalizado { get; set; }
    public int Cantidad { get; set; } = 1;  // unidades vendidas (relevante para tipo=producto)
    public decimal Monto { get; set; }
    public decimal Descuento { get; set; } = 0;
    public string MetodoPago { get; set; } = string.Empty; // efectivo | transferencia | pos | yape | plin | otro
    public string? Referencia { get; set; }
    public decimal Comision { get; set; } = 0;
    public string? Observaciones { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    public Cliente? Cliente { get; set; }
    public Empleado? Empleado { get; set; }
    public Servicio? Servicio { get; set; }
    public Producto? Producto { get; set; }
    public Paquete? Paquete { get; set; }
}
