using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Application.Features.Reports.Commands.RequestReport;
using PhoneRegistry.Application.Features.Reports.Queries.GetAllReports;
using PhoneRegistry.Application.Features.Reports.Queries.GetReportById;
using PhoneRegistry.Services.Interfaces;

namespace PhoneRegistry.Services.Implementations;

public class ReportService : IReportService
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReportService> _logger;

    public ReportService(IMediator mediator, ILogger<ReportService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Report> RequestReportAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.Requesting);

        var command = new RequestReportCommand();
        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation(Messages.Report.RequestedSuccessfully);
        return result;
    }

    public async Task<List<Report>> GetAllReportsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.GettingAll);

        var query = new GetAllReportsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} reports", result.Count);
        return result;
    }

    public async Task<Report?> GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.GettingById, reportId);

        var query = new GetReportByIdQuery { ReportId = reportId };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
        {
            _logger.LogWarning("Report not found: {ReportId}", reportId);
        }
        
        return result;
    }
}