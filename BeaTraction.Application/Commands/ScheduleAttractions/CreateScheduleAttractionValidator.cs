using FluentValidation;

namespace BeaTraction.Application.Commands.ScheduleAttractions;

public class CreateScheduleAttractionValidator : AbstractValidator<CreateScheduleAttractionCommand>
{
    public CreateScheduleAttractionValidator()
    {
        RuleFor(x => x.Data.ScheduleId)
            .NotEmpty().WithMessage("Schedule ID is required");

        RuleFor(x => x.Data.AttractionId)
            .NotEmpty().WithMessage("Attraction ID is required");
    }
}
