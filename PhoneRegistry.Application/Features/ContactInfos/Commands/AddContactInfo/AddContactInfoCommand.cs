using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.ValueObjects;

namespace PhoneRegistry.Application.Features.ContactInfos.Commands.AddContactInfo;

public class AddContactInfoCommand : ICommand<ContactInfo>
{
    public Guid PersonId { get; set; }
    public ContactType Type { get; set; }
    public string Content { get; set; } = string.Empty;
}
