using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Application.Features.Persons.Commands.CreatePerson;
using PhoneRegistry.Application.Features.Persons.Commands.DeletePerson;
using PhoneRegistry.Application.Features.Persons.Queries.GetPersonById;
using PhoneRegistry.Application.Features.Persons.Queries.GetAllPersons;
using PhoneRegistry.Application.Features.ContactInfos.Commands.AddContactInfo;
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

    public async Task<PersonDto> CreatePersonAsync(string firstName, string lastName, string? company = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating person: {FirstName} {LastName}", firstName, lastName);

        var command = new CreatePersonCommand
        {
            FirstName = firstName,
            LastName = lastName,
            Company = company
        };

        var result = await _mediator.Send(command, cancellationToken);
        
        _logger.LogInformation("Person created successfully");
        return result;
    }

    public async Task<PersonDto?> GetPersonByIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting person by ID: {PersonId}", personId);

        var query = new GetPersonByIdQuery { PersonId = personId };
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            _logger.LogWarning("Person not found: {PersonId}", personId);
        }

        return result;
    }

    public async Task<List<PersonSummaryDto>> GetAllPersonsAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all persons with skip: {Skip}, take: {Take}", skip, take);

        var query = new GetAllPersonsQuery { Skip = skip, Take = take };
        var result = await _mediator.Send(query, cancellationToken);

        _logger.LogInformation("Retrieved persons successfully");
        return result;
    }

    public async Task DeletePersonAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting person: {PersonId}", personId);

        var command = new DeletePersonCommand { PersonId = personId };
        await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Person deleted successfully: {PersonId}", personId);
    }

    public async Task<ContactInfoDto> AddContactInfoAsync(Guid personId, int contactType, string content, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding contact info for person: {PersonId}, type: {ContactType}", personId, contactType);

        var command = new AddContactInfoCommand
        {
            PersonId = personId,
            Type = (ContactType)contactType,
            Content = content
        };

        var result = await _mediator.Send(command, cancellationToken);
        
        _logger.LogInformation("Contact info added successfully");
        return result;
    }
}