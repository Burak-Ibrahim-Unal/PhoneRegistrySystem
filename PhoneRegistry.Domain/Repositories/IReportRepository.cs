using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Domain.Repositories;

public interface IReportRepository : IRepository<Report>
{
    Task<Report?> GetByIdWithStatisticsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Report>> GetAllWithStatisticsAsync(CancellationToken cancellationToken = default);
}
