using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Persons.Queries.GetPersonById;

public class GetPersonByIdQueryHandler : IQueryHandler<GetPersonByIdQuery, Person?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPersonByIdQueryHandler> _logger;

    public GetPersonByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPersonByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Person?> Handle(GetPersonByIdQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.GettingById, query.PersonId);

        var person = await _unitOfWork.Persons.GetByIdWithContactInfosAsync(query.PersonId, cancellationToken);
        
        if (person == null)
        {
            _logger.LogWarning(Messages.Person.NotFound, query.PersonId);
            return null;
        }

        return person;
    }
}
