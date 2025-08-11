using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Application.Common.Messaging;

namespace PhoneRegistry.Application.Features.ContactInfos.Commands.RemoveContactInfo;

public class RemoveContactInfoCommandHandler : IRequestHandler<RemoveContactInfoCommand>
{
    private readonly IContactUnitOfWork _contactUnitOfWork;
    private readonly ILogger<RemoveContactInfoCommandHandler> _logger;
    private readonly IOutboxWriter _outbox;

    public RemoveContactInfoCommandHandler(
        IContactUnitOfWork contactUnitOfWork,
        ILogger<RemoveContactInfoCommandHandler> logger,
        IOutboxWriter outbox)
    {
        _contactUnitOfWork = contactUnitOfWork;
        _logger = logger;
        _outbox = outbox;
    }

    public async Task Handle(RemoveContactInfoCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.ContactInfo.Removing, command.ContactInfoId, command.PersonId);

        var person = await _contactUnitOfWork.Persons.GetByIdWithContactInfosAsync(command.PersonId, cancellationToken);
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
        var evt = new ContactInfoDeleted(person.Id, contactInfo.Id);
        await _outbox.EnqueueAsync("ContactInfoDeleted", evt, cancellationToken);
        await _contactUnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(Messages.ContactInfo.RemovedSuccessfully, command.ContactInfoId);
    }
}
