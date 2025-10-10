using BeaTraction.Domain.Entities;

namespace BeaTraction.Domain.Interfaces;

public interface IScheduleAttractionRepository
{
    Task<List<ScheduleAttraction>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ScheduleAttraction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ScheduleAttraction>> GetByScheduleIdAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    Task<List<ScheduleAttraction>> GetByAttractionIdAsync(Guid attractionId, CancellationToken cancellationToken = default);
    Task<ScheduleAttraction?> GetByScheduleAndAttractionAsync(Guid scheduleId, Guid attractionId, CancellationToken cancellationToken = default);
    Task<ScheduleAttraction> CreateAsync(ScheduleAttraction scheduleAttraction, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleAttraction scheduleAttraction, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid scheduleId, Guid attractionId, CancellationToken cancellationToken = default);
    Task<bool> HasScheduleConflictAsync(Guid attractionId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
}
