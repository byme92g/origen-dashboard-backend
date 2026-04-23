using Microsoft.Extensions.Options;

namespace OrigenDashboard.Settings;

public static class ConfigurationValidationExtensions
{
    public static WebApplication ValidateConfiguration(this WebApplication app)
    {
        var adminOptions = app.Services.GetRequiredService<IOptions<AdminOptions>>().Value;
        var result = new AdminOptionsValidator().Validate(adminOptions);

        if (!result.IsValid)
            throw new InvalidOperationException(
                $"Configuración inválida:{Environment.NewLine}" +
                string.Join(Environment.NewLine, result.Errors.Select(e => e.ErrorMessage)));

        return app;
    }
}
