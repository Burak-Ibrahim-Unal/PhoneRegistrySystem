namespace PhoneRegistry.Domain.Entities;

public class City : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    // Navigation property
    public ICollection<ContactInfo> ContactInfos { get; private set; } = new List<ContactInfo>();

    protected City() { }

    public City(string name)
    {
        SetName(name);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be empty", nameof(name));

        Name = name.Trim();
        UpdateTimestamp();
    }
}


