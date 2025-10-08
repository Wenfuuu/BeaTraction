using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Interfaces;
using BeaTraction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BeaTraction.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly AppDbContext _context;

    public ScheduleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Schedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Schedules
            .Include(s => s.Attraction)
            .Include(s => s.Registrations)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Schedule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Schedules
            .Include(s => s.Attraction)
            .Include(s => s.Registrations)
            .ToListAsync(cancellationToken);
    }

    public async Task<Schedule> AddAsync(Schedule schedule, CancellationToken cancellationToken = default)
    {
        await _context.Schedules.AddAsync(schedule, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return schedule;
    }

    public async Task UpdateAsync(Schedule schedule, CancellationToken cancellationToken = default)
    {
        _context.Schedules.Update(schedule);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Schedule schedule, CancellationToken cancellationToken = default)
    {
        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
