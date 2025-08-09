using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Domain.Repositories;

public interface IReportRepository : IRepository<Report>
{
    Task<Report?> GetByIdWithStatisticsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Report>> GetAllWithStatisticsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Report>> GetAllAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);
}
