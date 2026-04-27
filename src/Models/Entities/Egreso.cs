namespace OrigenDashboard.Models.Entities;

public class Egreso
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string? Proveedor { get; set; }
    public string? Comprobante { get; set; }
    public string? Observaciones { get; set; }
    public int? CajaAperturaId { get; set; }
    public CajaApertura? CajaApertura { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
}
