using FluentValidation;

namespace BeaTraction.Application.Commands.Attractions;

public class CreateAttractionValidator : AbstractValidator<CreateAttractionCommand>
{
    public CreateAttractionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0");
    }
}
