using BeaTraction.Domain.Entities;

namespace BeaTraction.Domain.Interfaces;

public interface IScheduleAttractionRepository
{
    Task<IEnumerable<ScheduleAttraction>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ScheduleAttraction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduleAttraction>> GetByScheduleIdAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduleAttraction>> GetByAttractionIdAsync(Guid attractionId, CancellationToken cancellationToken = default);
    Task<ScheduleAttraction?> GetByScheduleAndAttractionAsync(Guid scheduleId, Guid attractionId, CancellationToken cancellationToken = default);
    Task<ScheduleAttraction> CreateAsync(ScheduleAttraction scheduleAttraction, CancellationToken cancellationToken = default);
    Task<ScheduleAttraction> UpdateAsync(ScheduleAttraction scheduleAttraction, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid scheduleId, Guid attractionId, CancellationToken cancellationToken = default);
}
