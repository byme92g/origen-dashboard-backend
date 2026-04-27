namespace OrigenDashboard.Models.Entities;

public class CajaApertura
{
    public int Id { get; set; }
    public DateTime AbiertaEn { get; set; } = DateTime.UtcNow;
    public decimal MontoInicial { get; set; }
    public string? Responsables { get; set; }
    public DateTime? CerradaEn { get; set; }
    public decimal? TotalIngresos { get; set; }
    public decimal? TotalEgresos { get; set; }
    public decimal? SaldoFinal { get; set; }
    public string? Observaciones { get; set; }
}
