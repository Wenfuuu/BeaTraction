using FluentValidation;

namespace BeaTraction.Application.Commands.Registrations;

public class DeleteRegistrationValidator : AbstractValidator<DeleteRegistrationCommand>
{
    public DeleteRegistrationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Registration ID is required");
    }
}
