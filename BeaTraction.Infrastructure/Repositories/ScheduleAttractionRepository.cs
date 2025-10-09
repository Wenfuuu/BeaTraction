using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Interfaces;
using BeaTraction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BeaTraction.Infrastructure.Repositories;

public class ScheduleAttractionRepository : IScheduleAttractionRepository
{
    private readonly AppDbContext _context;

    public ScheduleAttractionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ScheduleAttraction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleAttractions
            .Include(sa => sa.Schedule)
            .Include(sa => sa.Attraction)
            .ToListAsync(cancellationToken);
    }

    public async Task<ScheduleAttraction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleAttractions
            .Include(sa => sa.Schedule)
            .Include(sa => sa.Attraction)
            .FirstOrDefaultAsync(sa => sa.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ScheduleAttraction>> GetByScheduleIdAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleAttractions
            .Include(sa => sa.Schedule)
            .Include(sa => sa.Attraction)
            .Where(sa => sa.ScheduleId == scheduleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ScheduleAttraction>> GetByAttractionIdAsync(Guid attractionId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleAttractions
            .Include(sa => sa.Schedule)
            .Include(sa => sa.Attraction)
            .Where(sa => sa.AttractionId == attractionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ScheduleAttraction?> GetByScheduleAndAttractionAsync(Guid scheduleId, Guid attractionId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleAttractions
            .Include(sa => sa.Schedule)
            .Include(sa => sa.Attraction)
            .FirstOrDefaultAsync(sa => sa.ScheduleId == scheduleId && sa.AttractionId == attractionId, cancellationToken);
    }

    public async Task<ScheduleAttraction> CreateAsync(ScheduleAttraction scheduleAttraction, CancellationToken cancellationToken = default)
    {
        _context.ScheduleAttractions.Add(scheduleAttraction);
        await _context.SaveChangesAsync(cancellationToken);
        return scheduleAttraction;
    }

    public async Task<ScheduleAttraction> UpdateAsync(ScheduleAttraction scheduleAttraction, CancellationToken cancellationToken = default)
    {
        _context.ScheduleAttractions.Update(scheduleAttraction);
        await _context.SaveChangesAsync(cancellationToken);
        return scheduleAttraction;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var scheduleAttraction = await _context.ScheduleAttractions.FindAsync(new object[] { id }, cancellationToken);
        if (scheduleAttraction == null)
        {
            return false;
        }

        _context.ScheduleAttractions.Remove(scheduleAttraction);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid scheduleId, Guid attractionId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleAttractions
            .AnyAsync(sa => sa.ScheduleId == scheduleId && sa.AttractionId == attractionId, cancellationToken);
    }
}
