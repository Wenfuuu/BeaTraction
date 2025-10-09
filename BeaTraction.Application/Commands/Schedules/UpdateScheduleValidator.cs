using FluentValidation;

namespace BeaTraction.Application.Commands.Schedules;

public class UpdateScheduleValidator : AbstractValidator<UpdateScheduleCommand>
{
    public UpdateScheduleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Schedule ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time");
    }
}
