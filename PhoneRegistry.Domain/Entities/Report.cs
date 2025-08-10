using PhoneRegistry.Domain.ValueObjects;

namespace PhoneRegistry.Domain.Entities;

public class Report : BaseEntity
{
    public DateTime RequestedAt { get; private set; }
    public ReportStatus Status { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? ErrorMessage { get; private set; }

    private readonly List<LocationStatistic> _locationStatistics = new();
    public IReadOnlyList<LocationStatistic> LocationStatistics => _locationStatistics.AsReadOnly();

    public Report()
    {
        RequestedAt = DateTime.UtcNow;
        Status = ReportStatus.Preparing;
    }

    public void CompleteReport(IEnumerable<LocationStatistic> statistics)
    {
        if (Status != ReportStatus.Preparing)
            throw new InvalidOperationException("Report is not in preparing status");

        // Her statistic için ReportId'yi set et
        foreach (var stat in statistics)
        {
            stat.SetReportId(Id);
            _locationStatistics.Add(stat);
        }
        
        Status = ReportStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void FailReport(string errorMessage)
    {
        if (Status != ReportStatus.Preparing)
            throw new InvalidOperationException("Report is not in preparing status");

        Status = ReportStatus.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
