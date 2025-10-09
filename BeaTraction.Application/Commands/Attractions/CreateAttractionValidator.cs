using FluentValidation;

namespace BeaTraction.Application.Commands.Attractions;

public class CreateAttractionValidator : AbstractValidator<CreateAttractionCommand>
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024;

    public CreateAttractionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0");

        RuleFor(x => x.Image)
            .Must(file => file == null || file.Length <= MaxFileSize)
            .WithMessage($"Image size must not exceed {MaxFileSize / (1024 * 1024)}MB")
            .Must(file => file == null || AllowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            .WithMessage($"Image must be one of the following types: {string.Join(", ", AllowedExtensions)}");
    }
}
