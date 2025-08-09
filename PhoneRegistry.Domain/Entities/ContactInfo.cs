using PhoneRegistry.Domain.ValueObjects;

namespace PhoneRegistry.Domain.Entities;

public class ContactInfo : BaseEntity
{
    public Guid PersonId { get; private set; }
    public ContactType Type { get; private set; }
    public string Content { get; private set; } = string.Empty;

    // Navigation property
    public Person Person { get; private set; } = null!;

    protected ContactInfo() { } // EF Core

    public ContactInfo(Guid personId, ContactType type, string content)
    {
        PersonId = personId;
        Type = type;
        SetContent(content);
    }

    public void SetContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Contact content cannot be empty", nameof(content));

        // Validation based on type
        switch (Type)
        {
            case ContactType.PhoneNumber:
                ValidatePhoneNumber(content);
                break;
            case ContactType.EmailAddress:
                ValidateEmail(content);
                break;
            case ContactType.Location:
                ValidateLocation(content);
                break;
        }

        Content = content.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidatePhoneNumber(string phoneNumber)
    {
        if (phoneNumber.Length < 10 || phoneNumber.Length > 15)
            throw new ArgumentException("Phone number must be between 10 and 15 characters");
    }

    private static void ValidateEmail(string email)
    {
        if (!email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("Invalid email format");
    }

    private static void ValidateLocation(string location)
    {
        if (location.Length < 2)
            throw new ArgumentException("Location must be at least 2 characters");
    }
}
