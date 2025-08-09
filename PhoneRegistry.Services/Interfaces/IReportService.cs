using PhoneRegistry.Application.Common.DTOs;

namespace PhoneRegistry.Services.Interfaces;

public interface IReportService
{
    Task<ReportDto> RequestReportAsync(CancellationToken cancellationToken = default);
    Task<List<ReportDto>> GetAllReportsAsync(CancellationToken cancellationToken = default);
    Task<ReportDto?> GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default);
}
