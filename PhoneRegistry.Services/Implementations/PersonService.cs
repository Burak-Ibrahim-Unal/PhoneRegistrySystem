using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Application.Features.Persons.Commands.CreatePerson;
using PhoneRegistry.Application.Features.Persons.Commands.DeletePerson;
using PhoneRegistry.Application.Features.Persons.Queries.GetPersonById;
using PhoneRegistry.Application.Features.Persons.Queries.GetAllPersons;
using PhoneRegistry.Application.Features.ContactInfos.Commands.AddContactInfo;
using PhoneRegistry.Application.Features.ContactInfos.Commands.RemoveContactInfo;
using PhoneRegistry.Domain.ValueObjects;
using PhoneRegistry.Services.Interfaces;

namespace PhoneRegistry.Services.Implementations;

public class PersonService : IPersonService
{
    private readonly IMediator _mediator;
    private readonly ILogger<PersonService> _logger;

    public PersonService(IMediator mediator, ILogger<PersonService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Person> CreatePersonAsync(string firstName, string lastName, string? company = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.Creating, firstName, lastName);

        var command = new CreatePersonCommand
        {
            FirstName = firstName,
            LastName = lastName,
            Company = company
        };

        var result = await _mediator.Send(command, cancellationToken);
        
        _logger.LogInformation(Messages.Person.CreatedSuccessfully);
        return result;
    }

    public async Task<Person?> GetPersonByIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.GettingById, personId);

        var query = new GetPersonByIdQuery { PersonId = personId };
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            _logger.LogWarning(Messages.Person.NotFound, personId);
        }

        return result;
    }

    public async Task<List<Person>> GetAllPersonsAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.GettingAll, skip, take);

        var query = new GetAllPersonsQuery { Skip = skip, Take = take };
        var result = await _mediator.Send(query, cancellationToken);

        _logger.LogInformation(Messages.Person.RetrievedSuccessfully);
        return result;
    }

    public async Task DeletePersonAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.Deleting, personId);

        var command = new DeletePersonCommand { PersonId = personId };
        await _mediator.Send(command, cancellationToken);

        _logger.LogInformation(Messages.Person.DeletedSuccessfully, personId);
    }

    public async Task<ContactInfo> AddContactInfoAsync(Guid personId, int contactType, string content, Guid? cityId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.ContactInfo.Adding, personId, contactType);

        var command = new AddContactInfoCommand
        {
            PersonId = personId,
            Type = (ContactType)contactType,
            Content = content,
            CityId = cityId
        };

        var result = await _mediator.Send(command, cancellationToken);
        
        _logger.LogInformation(Messages.ContactInfo.AddedSuccessfully);
        return result;
    }

    public async Task RemoveContactInfoAsync(Guid personId, Guid contactInfoId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.ContactInfo.Removing, contactInfoId, personId);

        var command = new RemoveContactInfoCommand
        {
            PersonId = personId,
            ContactInfoId = contactInfoId
        };

        await _mediator.Send(command, cancellationToken);
        
        _logger.LogInformation(Messages.ContactInfo.RemovedSuccessfully, contactInfoId);
    }
}