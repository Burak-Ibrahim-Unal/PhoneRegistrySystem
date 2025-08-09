using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.ContactInfos.Commands.AddContactInfo;

public class AddContactInfoCommandHandler : ICommandHandler<AddContactInfoCommand, ContactInfo>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddContactInfoCommandHandler> _logger;

    public AddContactInfoCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<AddContactInfoCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ContactInfo> Handle(AddContactInfoCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.ContactInfo.Adding, command.PersonId, command.Type);

        var person = await _unitOfWork.Persons.GetByIdAsync(command.PersonId, cancellationToken);
        if (person == null)
        {
            throw new ArgumentException(Messages.ContactInfo.PersonNotFound.Replace("{PersonId}", command.PersonId.ToString()));
        }

        var contactInfo = person.AddContactInfo(command.Type, command.Content);
        
        // EF Core'a bu entity'nin yeni olduğunu söyle
        await _unitOfWork.ContactInfos.AddAsync(contactInfo, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(Messages.ContactInfo.AddedSuccessfully);

        return contactInfo;
    }
}
