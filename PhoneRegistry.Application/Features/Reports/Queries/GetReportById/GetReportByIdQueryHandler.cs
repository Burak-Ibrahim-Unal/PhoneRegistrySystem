using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Reports.Queries.GetReportById;

public class GetReportByIdQueryHandler : IQueryHandler<GetReportByIdQuery, Report?>
{
    private readonly IReportUnitOfWork _reportUnitOfWork;
    private readonly ILogger<GetReportByIdQueryHandler> _logger;

    public GetReportByIdQueryHandler(
        IReportUnitOfWork reportUnitOfWork,
        ILogger<GetReportByIdQueryHandler> logger)
    {
        _reportUnitOfWork = reportUnitOfWork;
        _logger = logger;
    }

    public async Task<Report?> Handle(GetReportByIdQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.GettingById, query.ReportId);

        var report = await _reportUnitOfWork.Reports.GetByIdWithStatisticsAsync(query.ReportId, cancellationToken);
        if (report == null)
        {
            _logger.LogWarning("Report not found: {ReportId}", query.ReportId);
            return null;
        }

        _logger.LogInformation("Report retrieved successfully: {ReportId}", query.ReportId);
        
        return report;
    }
}
