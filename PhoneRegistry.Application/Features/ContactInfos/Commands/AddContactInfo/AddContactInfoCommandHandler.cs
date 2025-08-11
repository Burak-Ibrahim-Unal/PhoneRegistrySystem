using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Application.Common.Messaging;
using PhoneRegistry.Application.Common.Interfaces;
using System.Text.Json;

namespace PhoneRegistry.Application.Features.ContactInfos.Commands.AddContactInfo;

public class AddContactInfoCommandHandler : ICommandHandler<AddContactInfoCommand, ContactInfo>
{
    private readonly IContactUnitOfWork _contactUnitOfWork;
    private readonly ILogger<AddContactInfoCommandHandler> _logger;
    private readonly IOutboxWriter _outbox;

    public AddContactInfoCommandHandler(
        IContactUnitOfWork contactUnitOfWork,
        ILogger<AddContactInfoCommandHandler> logger,
        IOutboxWriter outbox)
    {
        _contactUnitOfWork = contactUnitOfWork;
        _logger = logger;
        _outbox = outbox;
    }

    public async Task<ContactInfo> Handle(AddContactInfoCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.ContactInfo.Adding, command.PersonId, command.Type);

        var person = await _contactUnitOfWork.Persons.GetByIdAsync(command.PersonId, cancellationToken);
        if (person == null)
        {
            throw new ArgumentException(Messages.ContactInfo.PersonNotFound.Replace("{PersonId}", command.PersonId.ToString()));
        }

        var normalizedContent = command.Content;
        if (command.Type == Domain.ValueObjects.ContactType.Location && string.IsNullOrWhiteSpace(normalizedContent))
        {
            // Location için CityId gönderiliyorsa içerik zorunlu olmasın (geriye dönük alan)
            normalizedContent = "Location"; // minimal placeholder
        }

        var contactInfo = person.AddContactInfo(command.Type, normalizedContent);
        if (command.Type == Domain.ValueObjects.ContactType.Location)
        {
            if (command.CityId.HasValue && command.CityId.Value != Guid.Empty)
            {
                contactInfo.SetCity(command.CityId.Value);
            }
        }
        
        // EF Core'a bu entity'nin yeni olduğunu söyle
        await _contactUnitOfWork.ContactInfos.AddAsync(contactInfo, cancellationToken);
        // CityId Guid? → event payload beklenen alan string? ise uygun forma dönüştür
        var evt = new ContactInfoUpserted(
            person.Id,
            contactInfo.Id,
            (int)command.Type,
            command.Content,
            command.CityId?.ToString());
        await _outbox.EnqueueAsync("ContactInfoUpserted", evt, cancellationToken);
        
        await _contactUnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(Messages.ContactInfo.AddedSuccessfully);

        return contactInfo;
    }
}
