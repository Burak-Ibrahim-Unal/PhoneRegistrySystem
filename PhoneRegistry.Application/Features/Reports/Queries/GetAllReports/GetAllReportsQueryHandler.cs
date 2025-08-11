using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Reports.Queries.GetAllReports;

public class GetAllReportsQueryHandler : IQueryHandler<GetAllReportsQuery, List<Report>>
{
    private readonly IReportUnitOfWork _reportUnitOfWork;
    private readonly ILogger<GetAllReportsQueryHandler> _logger;

    public GetAllReportsQueryHandler(
        IReportUnitOfWork reportUnitOfWork,
        ILogger<GetAllReportsQueryHandler> logger)
    {
        _reportUnitOfWork = reportUnitOfWork;
        _logger = logger;
    }

    public async Task<List<Report>> Handle(GetAllReportsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.GettingAll);

        // Konum/kişi/telefon özetlerinin doğru hesaplanabilmesi için istatistikleri include ederek çek
        var reportsWithStats = await _reportUnitOfWork.Reports.GetAllWithStatisticsAsync(cancellationToken);

        // Tercihen RequestedAt'e göre sıralayıp sayfalandır
        var result = reportsWithStats
            .OrderByDescending(r => r.RequestedAt)
            .Skip(query.Skip)
            .Take(query.Take)
            .ToList();

        _logger.LogInformation("Retrieved {Count} reports", result.Count);
        return result;
    }
}
