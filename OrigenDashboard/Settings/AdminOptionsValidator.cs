using FluentValidation;

namespace OrigenDashboard.Settings;

public sealed class AdminOptionsValidator : AbstractValidator<AdminOptions>
{
    public AdminOptionsValidator()
    {
        RuleFor(x => x.AdminUsername)
            .NotEmpty()
                .WithMessage("AdminUsername es requerido.")
            .MinimumLength(3)
                .WithMessage("AdminUsername debe tener al menos 3 caracteres.")
            .MaximumLength(50)
                .WithMessage("AdminUsername no puede superar 50 caracteres.");

        RuleFor(x => x.AdminPassword)
            .NotEmpty()
                .WithMessage("AdminPassword es requerido.")
            .MinimumLength(8)
                .WithMessage("AdminPassword debe tener al menos 8 caracteres.")
            .MaximumLength(100)
                .WithMessage("AdminPassword no puede superar 100 caracteres.");
    }
}
