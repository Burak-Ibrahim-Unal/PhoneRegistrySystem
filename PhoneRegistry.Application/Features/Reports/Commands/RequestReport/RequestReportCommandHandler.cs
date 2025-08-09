using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Reports.Commands.RequestReport;

public class RequestReportCommandHandler : ICommandHandler<RequestReportCommand, Report>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RequestReportCommandHandler> _logger;

    public RequestReportCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<RequestReportCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Report> Handle(RequestReportCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.Requesting);

        var report = new Report();
        
        await _unitOfWork.Reports.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(Messages.Report.Created, report.Id);

        // TODO: Publish message to queue for async processing

        return report;
    }
}
