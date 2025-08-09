using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Reports.Queries.GetReportById;

public class GetReportByIdQueryHandler : IQueryHandler<GetReportByIdQuery, Report?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetReportByIdQueryHandler> _logger;

    public GetReportByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetReportByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Report?> Handle(GetReportByIdQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.GettingById, query.ReportId);

        var report = await _unitOfWork.Reports.GetByIdWithStatisticsAsync(query.ReportId, cancellationToken);
        if (report == null)
        {
            _logger.LogWarning("Report not found: {ReportId}", query.ReportId);
            return null;
        }

        _logger.LogInformation("Report retrieved successfully: {ReportId}", query.ReportId);
        
        return report;
    }
}
