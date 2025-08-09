using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Application.Features.Reports.Commands.RequestReport;
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

    public async Task<ReportDto> RequestReportAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.Requesting);

        var command = new RequestReportCommand();
        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation(Messages.Report.RequestedSuccessfully);
        return result;
    }

    public async Task<List<ReportDto>> GetAllReportsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.GettingAll);

        // TODO: Implement GetAllReportsQuery when needed
        await Task.CompletedTask;
        return new List<ReportDto>();
    }

    public async Task<ReportDto?> GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.GettingById, reportId);

        // TODO: Implement GetReportByIdQuery when needed
        await Task.CompletedTask;
        return null;
    }
}