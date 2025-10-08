using BeaTraction.Domain.Entities;

namespace BeaTraction.Domain.Interfaces;

public interface IRegistrationRepository
{
    Task<Registration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Registration>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Registration>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Registration>> GetByAttractionIdAsync(Guid attractionId, CancellationToken cancellationToken = default);
    Task<Registration> AddAsync(Registration registration, CancellationToken cancellationToken = default);
    Task UpdateAsync(Registration registration, CancellationToken cancellationToken = default);
    Task DeleteAsync(Registration registration, CancellationToken cancellationToken = default);
    Task<bool> UserAlreadyRegisteredAsync(Guid userId, Guid attractionId, CancellationToken cancellationToken = default);
}
