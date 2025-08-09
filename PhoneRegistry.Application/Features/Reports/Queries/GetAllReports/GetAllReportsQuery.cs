using MediatR;
using PhoneRegistry.Application.Common.DTOs;

namespace PhoneRegistry.Application.Features.Reports.Queries.GetAllReports;

public class GetAllReportsQuery : IRequest<List<ReportDto>>
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}
