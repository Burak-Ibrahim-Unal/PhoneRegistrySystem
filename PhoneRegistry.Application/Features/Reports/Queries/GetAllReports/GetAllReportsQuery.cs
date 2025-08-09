using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Application.Features.Reports.Queries.GetAllReports;

public class GetAllReportsQuery : IQuery<List<Report>>
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}
