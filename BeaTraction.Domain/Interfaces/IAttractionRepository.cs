using BeaTraction.Domain.Entities;

namespace BeaTraction.Domain.Interfaces;

public interface IAttractionRepository
{
    Task<Attraction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Attraction>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Attraction>> GetByScheduleIdAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    Task<Attraction> AddAsync(Attraction attraction, CancellationToken cancellationToken = default);
    Task UpdateAsync(Attraction attraction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Attraction attraction, CancellationToken cancellationToken = default);
}
