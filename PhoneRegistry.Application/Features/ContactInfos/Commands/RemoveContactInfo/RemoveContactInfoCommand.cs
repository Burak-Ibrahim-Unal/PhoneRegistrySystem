using MediatR;

namespace PhoneRegistry.Application.Features.ContactInfos.Commands.RemoveContactInfo;

public class RemoveContactInfoCommand : IRequest
{
    public Guid PersonId { get; set; }
    public Guid ContactInfoId { get; set; }
}
