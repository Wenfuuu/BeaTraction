using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Interfaces;
using BeaTraction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BeaTraction.Infrastructure.Repositories;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly AppDbContext _context;

    public RegistrationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Registration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.ScheduleAttraction)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<Registration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.ScheduleAttraction)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Registration>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Registrations
            .Include(r => r.ScheduleAttraction)
            .Where(r => r.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Registration>> GetByAttractionIdAsync(Guid attractionId, CancellationToken cancellationToken = default)
    {
        return await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.ScheduleAttraction)
            .Where(r => r.ScheduleAttraction.AttractionId == attractionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Registration> AddAsync(Registration registration, CancellationToken cancellationToken = default)
    {
        await _context.Registrations.AddAsync(registration, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return registration;
    }

    public async Task UpdateAsync(Registration registration, CancellationToken cancellationToken = default)
    {
        _context.Registrations.Update(registration);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Registration registration, CancellationToken cancellationToken = default)
    {
        _context.Registrations.Remove(registration);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UserAlreadyRegisteredAsync(Guid userId, Guid scheduleAttractionId, CancellationToken cancellationToken = default)
    {
        return await _context.Registrations
            .AnyAsync(r => r.UserId == userId && r.ScheduleAttractionId == scheduleAttractionId, cancellationToken);
    }
}
