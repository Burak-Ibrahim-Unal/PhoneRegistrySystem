using MediatR;
using PhoneRegistry.Application.Common.DTOs;

namespace PhoneRegistry.Application.Features.Reports.Commands.RequestReport;

public class RequestReportCommand : IRequest<ReportDto>
{
}
