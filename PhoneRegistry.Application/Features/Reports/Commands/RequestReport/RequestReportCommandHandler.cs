using AutoMapper;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Reports.Commands.RequestReport;

public class RequestReportCommandHandler : ICommandHandler<RequestReportCommand, ReportDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RequestReportCommandHandler> _logger;

    public RequestReportCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RequestReportCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ReportDto> HandleAsync(RequestReportCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Requesting new report");

        var report = new Report();
        
        await _unitOfWork.Reports.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Report requested successfully with ID: {ReportId}", report.Id);

        // TODO: Publish message to queue for async processing

        return _mapper.Map<ReportDto>(report);
    }
}
