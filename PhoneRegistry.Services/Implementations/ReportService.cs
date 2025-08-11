using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Application.Features.Reports.Commands.RequestReport;
using PhoneRegistry.Application.Features.Reports.Queries.GetAllReports;
using PhoneRegistry.Application.Features.Reports.Queries.GetReportById;
using PhoneRegistry.Services.Interfaces;
using PhoneRegistry.Caching.Interfaces;

namespace PhoneRegistry.Services.Implementations;

public class ReportService : IReportService
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReportService> _logger;
    private readonly ICacheService _cacheService;
    private const string REPORT_CACHE_KEY = "report:{0}";
    private const string ALL_REPORTS_CACHE_KEY = "all_reports";
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public ReportService(IMediator mediator, ILogger<ReportService> logger, ICacheService cacheService)
    {
        _mediator = mediator;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<Report> RequestReportAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.Requesting);

        var command = new RequestReportCommand();
        var result = await _mediator.Send(command, cancellationToken);
        
        // Invalidate all reports cache when a new report is requested
        await _cacheService.RemoveAsync(ALL_REPORTS_CACHE_KEY);

        _logger.LogInformation(Messages.Report.RequestedSuccessfully);
        return result;
    }

    public async Task<List<Report>> GetAllReportsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.GettingAll);
        
        // Try to get from cache first
        var cachedReports = await _cacheService.GetAsync<List<Report>>(ALL_REPORTS_CACHE_KEY);
        
        if (cachedReports != null)
        {
            _logger.LogDebug("Reports list found in cache");
            return cachedReports;
        }

        var query = new GetAllReportsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        
        // Cache the result
        await _cacheService.SetAsync(ALL_REPORTS_CACHE_KEY, result, TimeSpan.FromMinutes(2));
        
        _logger.LogInformation("Retrieved {Count} reports", result.Count);
        return result;
    }

    public async Task<Report?> GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.GettingById, reportId);
        
        // Try to get from cache first
        var cacheKey = string.Format(REPORT_CACHE_KEY, reportId);
        var cachedReport = await _cacheService.GetAsync<Report>(cacheKey);
        
        if (cachedReport != null)
        {
            _logger.LogDebug("Report {ReportId} found in cache", reportId);
            return cachedReport;
        }

        var query = new GetReportByIdQuery { ReportId = reportId };
        var result = await _mediator.Send(query, cancellationToken);
        
        // Cache the result if found
        if (result != null)
        {
            await _cacheService.SetAsync(cacheKey, result, _cacheExpiration);
            _logger.LogDebug("Report {ReportId} cached", reportId);
        }
        
        if (result == null)
        {
            _logger.LogWarning("Report not found: {ReportId}", reportId);
        }
        
        return result;
    }
}