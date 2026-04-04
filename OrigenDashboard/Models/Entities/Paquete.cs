namespace OrigenDashboard.Models.Entities;

public class Paquete
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public decimal Descuento { get; set; } = 0;
    public bool Activo { get; set; } = true;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    public ICollection<PaqueteServicio> Servicios { get; set; } = [];
}

public class PaqueteServicio
{
    public int Id { get; set; }
    public int PaqueteId { get; set; }
    public int ServicioId { get; set; }

    public Paquete Paquete { get; set; } = null!;
    public Servicio Servicio { get; set; } = null!;
}
