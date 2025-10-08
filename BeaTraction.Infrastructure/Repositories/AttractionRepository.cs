using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Interfaces;
using BeaTraction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BeaTraction.Infrastructure.Repositories;

public class AttractionRepository : IAttractionRepository
{
    private readonly AppDbContext _context;

    public AttractionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Attraction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Attractions
            .Include(a => a.Schedule)
            .Include(a => a.Registrations)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Attraction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Attractions
            .Include(a => a.Schedule)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Attraction>> GetByScheduleIdAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        return await _context.Attractions
            .Where(a => a.ScheduleId == scheduleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Attraction> AddAsync(Attraction attraction, CancellationToken cancellationToken = default)
    {
        await _context.Attractions.AddAsync(attraction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return attraction;
    }

    public async Task UpdateAsync(Attraction attraction, CancellationToken cancellationToken = default)
    {
        _context.Attractions.Update(attraction);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Attraction attraction, CancellationToken cancellationToken = default)
    {
        _context.Attractions.Remove(attraction);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
