namespace OrigenDashboard.Models.Entities;

public class CajaAperturaResponsable
{
    public int CajaAperturaId { get; set; }
    public CajaApertura CajaApertura { get; set; } = null!;
    public int EmpleadoId { get; set; }
    public Empleado Empleado { get; set; } = null!;
}
