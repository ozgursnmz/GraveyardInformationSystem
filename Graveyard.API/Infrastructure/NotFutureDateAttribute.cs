using System.ComponentModel.DataAnnotations;

namespace Graveyard.API.Infrastructure;

// Tarihin gelecekte olmamasini dogrular (DateOnly icin). Ornek: dogum/olum/odeme tarihi.
public class NotFutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is DateOnly d && d > DateOnly.FromDateTime(DateTime.Today))
            return new ValidationResult(
                ErrorMessage ?? "Tarih gelecekte olamaz.",
                ctx.MemberName != null ? new[] { ctx.MemberName } : null);
        return ValidationResult.Success;
    }
}
