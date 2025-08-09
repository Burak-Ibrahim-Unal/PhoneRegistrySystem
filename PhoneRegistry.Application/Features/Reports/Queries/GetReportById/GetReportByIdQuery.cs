using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Application.Features.Reports.Queries.GetReportById;

public class GetReportByIdQuery : IQuery<Report?>
{
    public Guid ReportId { get; set; }
}
