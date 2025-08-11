using FluentValidation;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Application.Common.Messaging;
using System.Text.Json;

namespace PhoneRegistry.Application.Features.Persons.Commands.CreatePerson;

public class CreatePersonCommandHandler : ICommandHandler<CreatePersonCommand, Person>
{
    private readonly IContactUnitOfWork _contactUnitOfWork;
    private readonly IValidator<CreatePersonCommand> _validator;
    private readonly ILogger<CreatePersonCommandHandler> _logger;

    public CreatePersonCommandHandler(
        IContactUnitOfWork contactUnitOfWork,
        IValidator<CreatePersonCommand> validator,
        ILogger<CreatePersonCommandHandler> logger)
    {
        _contactUnitOfWork = contactUnitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Person> Handle(CreatePersonCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.Creating, command.FirstName, command.LastName);

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var person = new Person(command.FirstName, command.LastName, command.Company);
        
        await _contactUnitOfWork.Persons.AddAsync(person, cancellationToken);
        // Outbox write (same transaction boundary)
        var evt = new PersonUpserted(person.Id, person.FirstName, person.LastName, person.Company);
        var outbox = new OutboxMessage("PersonUpserted", JsonSerializer.Serialize(evt));
        await (_contactUnitOfWork as dynamic)._context.OutboxMessages.AddAsync(outbox, cancellationToken);
        await _contactUnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(Messages.Person.CreatedSuccessfully);

        return person;
    }
}
