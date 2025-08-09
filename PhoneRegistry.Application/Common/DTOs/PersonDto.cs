using PhoneRegistry.Domain.ValueObjects;

namespace PhoneRegistry.Application.Common.DTOs;

// Request DTOs - sadece API'den gelen istekler için
public class CreatePersonRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Company { get; set; }
}

public class AddContactInfoRequest
{
    public ContactType Type { get; set; }
    public string Content { get; set; } = string.Empty;
}
