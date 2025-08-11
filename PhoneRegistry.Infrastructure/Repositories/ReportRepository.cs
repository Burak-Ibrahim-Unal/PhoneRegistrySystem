using Microsoft.EntityFrameworkCore;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.Infrastructure.Repositories;

public class ReportRepository : Repository<Report>, IReportRepository
{
    public ReportRepository(ReportDbContext context) : base(context)
    {
    }

    public async Task<Report?> GetByIdWithStatisticsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.LocationStatistics)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Report>> GetAllWithStatisticsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.LocationStatistics)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Report>> GetAllAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(r => r.RequestedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
