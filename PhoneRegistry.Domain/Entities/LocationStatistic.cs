namespace PhoneRegistry.Domain.Entities;

public class LocationStatistic : BaseEntity
{
    public Guid ReportId { get; private set; }
    public string Location { get; private set; } = string.Empty;
    public int PersonCount { get; private set; }
    public int PhoneNumberCount { get; private set; }

    // Navigation property
    public Report Report { get; private set; } = null!;

    protected LocationStatistic() { } // EF Core

    public LocationStatistic(Guid reportId, string location, int personCount, int phoneNumberCount)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty", nameof(location));

        if (personCount < 0)
            throw new ArgumentException("Person count cannot be negative", nameof(personCount));

        if (phoneNumberCount < 0)
            throw new ArgumentException("Phone number count cannot be negative", nameof(phoneNumberCount));

        ReportId = reportId;
        Location = location.Trim();
        PersonCount = personCount;
        PhoneNumberCount = phoneNumberCount;
    }
}
