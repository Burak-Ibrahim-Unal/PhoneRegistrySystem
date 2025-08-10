namespace PhoneRegistry.Application.Common.Messaging;

public class ReportRequestMessage
{
    public Guid ReportId { get; set; }
    public DateTime RequestedAt { get; set; }

    public ReportRequestMessage(Guid reportId, DateTime requestedAt)
    {
        ReportId = reportId;
        RequestedAt = requestedAt;
    }
}
