using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Messaging.Interfaces;
using PhoneRegistry.Messaging.Models;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Domain.Common.Constants;

namespace PhoneRegistry.Application.Features.Reports.Commands.RequestReport;

public class RequestReportCommandHandler : ICommandHandler<RequestReportCommand, Report>
{
    private readonly IReportUnitOfWork _reportUnitOfWork;
    private readonly ILogger<RequestReportCommandHandler> _logger;
    private readonly IMessagePublisher _messagePublisher;

    public RequestReportCommandHandler(
        IReportUnitOfWork reportUnitOfWork,
        ILogger<RequestReportCommandHandler> logger,
        IMessagePublisher messagePublisher)
    {
        _reportUnitOfWork = reportUnitOfWork;
        _logger = logger;
        _messagePublisher = messagePublisher;
    }

    public async Task<Report> Handle(RequestReportCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.Requesting);

        var report = new Report();
        
        await _reportUnitOfWork.Reports.AddAsync(report, cancellationToken);
        await _reportUnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(Messages.Report.Created, report.Id);

        // Publish message to RabbitMQ for async processing
        var message = new ReportRequestMessage(report.Id, report.RequestedAt);
        await _messagePublisher.PublishAsync(message, MessagingConstants.Queues.ReportProcessing, cancellationToken);
        
        _logger.LogInformation("Report request message published to queue for Report {ReportId}", report.Id);

        return report;
    }
}
