using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.ContactInfos.Commands.RemoveContactInfo;

public class RemoveContactInfoCommandHandler : IRequestHandler<RemoveContactInfoCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveContactInfoCommandHandler> _logger;

    public RemoveContactInfoCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<RemoveContactInfoCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(RemoveContactInfoCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.ContactInfo.Removing, command.ContactInfoId, command.PersonId);

        var person = await _unitOfWork.Persons.GetByIdWithContactInfosAsync(command.PersonId, cancellationToken);
        if (person == null)
        {
            throw new ArgumentException(Messages.ContactInfo.PersonNotFound.Replace("{PersonId}", command.PersonId.ToString()));
        }

        var contactInfo = person.ContactInfos.FirstOrDefault(ci => ci.Id == command.ContactInfoId);
        if (contactInfo == null)
        {
            throw new ArgumentException(Messages.ContactInfo.NotFound.Replace("{ContactInfoId}", command.ContactInfoId.ToString()));
        }

        // Soft delete the contact info
        contactInfo.SoftDelete();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(Messages.ContactInfo.RemovedSuccessfully, command.ContactInfoId);
    }
}
