using PhoneRegistry.Domain.ValueObjects;

namespace PhoneRegistry.Domain.Entities;

public class Person : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? Company { get; private set; }

    private readonly List<ContactInfo> _contactInfos = new();
    public IReadOnlyList<ContactInfo> ContactInfos => _contactInfos.AsReadOnly();

    protected Person() { } // EF Core

    public Person(string firstName, string lastName, string? company = null)
    {
        SetName(firstName, lastName);
        Company = company;
    }

    public void SetName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCompany(string? company)
    {
        Company = company?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public ContactInfo AddContactInfo(ContactType type, string content)
    {
        var contactInfo = new ContactInfo(Id, type, content);
        _contactInfos.Add(contactInfo);
        UpdatedAt = DateTime.UtcNow;
        return contactInfo;
    }

    public void RemoveContactInfo(Guid contactInfoId)
    {
        var contactInfo = _contactInfos.FirstOrDefault(ci => ci.Id == contactInfoId);
        if (contactInfo != null)
        {
            _contactInfos.Remove(contactInfo);
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
        
        // Contact info'ları da soft delete yap
        foreach (var contactInfo in _contactInfos)
        {
            contactInfo.SoftDelete();
        }
    }


    public string GetFullName() => $"{FirstName} {LastName}";
}
