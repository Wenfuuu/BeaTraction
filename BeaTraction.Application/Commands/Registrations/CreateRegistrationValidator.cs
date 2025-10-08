using FluentValidation;

namespace BeaTraction.Application.Commands.Registrations;

public class CreateRegistrationValidator : AbstractValidator<CreateRegistrationCommand>
{
    public CreateRegistrationValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("Schedule ID is required");

        RuleFor(x => x.RegisteredAt)
            .NotEmpty().WithMessage("Registered At is required");
    }
}