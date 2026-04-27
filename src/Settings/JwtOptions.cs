using System.ComponentModel.DataAnnotations;

namespace OrigenDashboard.Settings;

public sealed class JwtOptions
{
    public const string Section = "Jwt";

    [Required]
    public string Secret { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    public int ExpiryMinutes { get; init; } = 30;
}
