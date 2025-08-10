using PhoneRegistry.Domain.ValueObjects;

namespace PhoneRegistry.Domain.Entities;

public class ContactInfo : BaseEntity
{
    public Guid PersonId { get; private set; }
    public ContactType Type { get; private set; }
    // Telefon numarası veya e-posta için içerik
    public string Content { get; private set; } = string.Empty;
    
    // Şehir normalizasyonu
    public Guid? CityId { get; private set; }
    public City? City { get; private set; }

    // Navigation property
    public Person Person { get; private set; } = null!;

    protected ContactInfo() { } // EF Core

    public ContactInfo(Guid personId, ContactType type, string content, Guid? cityId = null)
    {
        if (personId == Guid.Empty)
            throw new ArgumentException("PersonId cannot be empty", nameof(personId));
            
        PersonId = personId;
        Type = type;
        SetContent(content);
        if (cityId.HasValue && cityId.Value != Guid.Empty)
        {
            CityId = cityId.Value;
        }
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
                // Artık konum/şehir metinsel olarak burada tutulmayacak.
                // Geriye dönük uyumluluk için çok kısa olmayan bir değer kabul edilir ama yönlendirici amaçlı.
                ValidateLocation(content);
                break;
        }

        Content = content.Trim();
        UpdateTimestamp();
    }

    private static void ValidatePhoneNumber(string phoneNumber)
    {
        // Boşluk ve özel karakterleri temizle
        var cleanPhone = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        if (cleanPhone.Length < 7 || cleanPhone.Length > 20)
            throw new ArgumentException("Telefon numarası 7-20 karakter arasında olmalıdır");
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Geçersiz email formatı");
    }

    private static void ValidateLocation(string location)
    {
        if (location.Length < 2)
            throw new ArgumentException("Location must be at least 2 characters");
    }

    public void SetCity(Guid cityId)
    {
        if (cityId == Guid.Empty)
            throw new ArgumentException("CityId cannot be empty", nameof(cityId));
        CityId = cityId;
        UpdateTimestamp();
    }

    public void SoftDelete()
    {
        MarkAsDeleted();
    }
}
