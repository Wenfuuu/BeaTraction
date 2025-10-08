using BeaTraction.Domain.Entities;

namespace BeaTraction.Domain.Interfaces;

public interface IScheduleRepository
{
    Task<Schedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Schedule>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Schedule> AddAsync(Schedule schedule, CancellationToken cancellationToken = default);
    Task UpdateAsync(Schedule schedule, CancellationToken cancellationToken = default);
    Task DeleteAsync(Schedule schedule, CancellationToken cancellationToken = default);
}
