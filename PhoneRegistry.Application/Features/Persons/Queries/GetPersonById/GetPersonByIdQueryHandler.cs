using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Persons.Queries.GetPersonById;

public class GetPersonByIdQueryHandler : IQueryHandler<GetPersonByIdQuery, Person?>
{
    private readonly IContactUnitOfWork _contactUnitOfWork;
    private readonly ILogger<GetPersonByIdQueryHandler> _logger;

    public GetPersonByIdQueryHandler(
        IContactUnitOfWork contactUnitOfWork,
        ILogger<GetPersonByIdQueryHandler> logger)
    {
        _contactUnitOfWork = contactUnitOfWork;
        _logger = logger;
    }

    public async Task<Person?> Handle(GetPersonByIdQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.GettingById, query.PersonId);

        var person = await _contactUnitOfWork.Persons.GetByIdWithContactInfosAsync(query.PersonId, cancellationToken);
        
        if (person == null)
        {
            _logger.LogWarning(Messages.Person.NotFound, query.PersonId);
            return null;
        }

        return person;
    }
}
