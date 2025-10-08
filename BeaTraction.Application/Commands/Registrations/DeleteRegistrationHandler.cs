using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Registrations;

public class DeleteRegistrationHandler : IRequestHandler<DeleteRegistrationCommand, bool>
{
    private readonly IRegistrationRepository _registrationRepository;

    public DeleteRegistrationHandler(IRegistrationRepository registrationRepository)
    {
        _registrationRepository = registrationRepository;
    }

    public async Task<bool> Handle(DeleteRegistrationCommand request, CancellationToken cancellationToken)
    {
        var registration = await _registrationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (registration == null)
        {
            throw new InvalidOperationException("Registration not found");
        }

        await _registrationRepository.DeleteAsync(registration, cancellationToken);
        return true;
    }
}
