using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Application.Common.Messaging;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Reports.Commands.RequestReport;

public class RequestReportCommandHandler : ICommandHandler<RequestReportCommand, Report>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RequestReportCommandHandler> _logger;
    private readonly IMessagePublisher _messagePublisher;

    public RequestReportCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<RequestReportCommandHandler> logger,
        IMessagePublisher messagePublisher)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _messagePublisher = messagePublisher;
    }

    public async Task<Report> Handle(RequestReportCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.Requesting);

        var report = new Report();
        
        await _unitOfWork.Reports.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(Messages.Report.Created, report.Id);

        // Publish message to RabbitMQ for async processing
        var message = new ReportRequestMessage(report.Id, report.RequestedAt);
        await _messagePublisher.PublishAsync(message, "report-processing-queue", cancellationToken);
        
        _logger.LogInformation("Report request message published to queue for Report {ReportId}", report.Id);

        return report;
    }
}
