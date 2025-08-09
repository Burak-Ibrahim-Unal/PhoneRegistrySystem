using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Reports.Queries.GetReportById;

public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ReportDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetReportByIdQueryHandler> _logger;

    public GetReportByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetReportByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ReportDto?> Handle(GetReportByIdQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Report.GettingById, query.ReportId);

        var report = await _unitOfWork.Reports.GetByIdWithStatisticsAsync(query.ReportId, cancellationToken);
        if (report == null)
        {
            _logger.LogWarning("Report not found: {ReportId}", query.ReportId);
            return null;
        }

        var result = _mapper.Map<ReportDto>(report);
        _logger.LogInformation("Report retrieved successfully: {ReportId}", query.ReportId);
        
        return result;
    }
}
