using MediatR;
using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Domain.ValueObjects;

namespace PhoneRegistry.Application.Features.ContactInfos.Commands.AddContactInfo;

public class AddContactInfoCommand : IRequest<ContactInfoDto>
{
    public Guid PersonId { get; set; }
    public ContactType Type { get; set; }
    public string Content { get; set; } = string.Empty;
}
