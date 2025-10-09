using FluentValidation;

namespace BeaTraction.Application.Commands.Registrations;

public class UpdateRegistrationValidator : AbstractValidator<UpdateRegistrationCommand>
{
    public UpdateRegistrationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Registration ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.ScheduleAttractionId)
            .NotEmpty().WithMessage("ScheduleAttraction ID is required");

        RuleFor(x => x.RegisteredAt)
            .NotEmpty().WithMessage("Registered At is required");
    }
}
