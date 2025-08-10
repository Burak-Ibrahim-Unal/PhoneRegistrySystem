using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Reports.Queries.GetAllReports;

public class GetAllReportsQueryHandler : IQueryHandler<GetAllReportsQuery, List<Report>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAllReportsQueryHandler> _logger;

    public GetAllReportsQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetAllReportsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<Report>> Handle(GetAllReportsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.GettingAll);

        // Konum/kişi/telefon özetlerinin doğru hesaplanabilmesi için istatistikleri include ederek çek
        var reportsWithStats = await _unitOfWork.Reports.GetAllWithStatisticsAsync(cancellationToken);

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
