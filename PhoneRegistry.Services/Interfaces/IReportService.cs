using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Services.Interfaces;

public interface IReportService
{
    Task<Report> RequestReportAsync(CancellationToken cancellationToken = default);
    Task<List<Report>> GetAllReportsAsync(CancellationToken cancellationToken = default);
    Task<Report?> GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default);
}
