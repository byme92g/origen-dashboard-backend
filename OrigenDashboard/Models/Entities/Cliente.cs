namespace OrigenDashboard.Models.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Observaciones { get; set; } // tipo de cabello, alergias, preferencias
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
