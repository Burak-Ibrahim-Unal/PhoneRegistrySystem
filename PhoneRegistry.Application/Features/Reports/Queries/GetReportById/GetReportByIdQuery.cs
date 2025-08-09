using MediatR;
using PhoneRegistry.Application.Common.DTOs;

namespace PhoneRegistry.Application.Features.Reports.Queries.GetReportById;

public class GetReportByIdQuery : IRequest<ReportDto?>
{
    public Guid ReportId { get; set; }
}
