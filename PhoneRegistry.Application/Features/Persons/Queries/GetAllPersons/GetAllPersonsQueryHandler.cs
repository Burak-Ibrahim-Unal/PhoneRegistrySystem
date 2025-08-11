using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Persons.Queries.GetAllPersons;

public class GetAllPersonsQueryHandler : IQueryHandler<GetAllPersonsQuery, List<Person>>
{
    private readonly IContactUnitOfWork _contactUnitOfWork;
    private readonly ILogger<GetAllPersonsQueryHandler> _logger;

    public GetAllPersonsQueryHandler(
        IContactUnitOfWork contactUnitOfWork,
        ILogger<GetAllPersonsQueryHandler> logger)
    {
        _contactUnitOfWork = contactUnitOfWork;
        _logger = logger;
    }

    public async Task<List<Person>> Handle(GetAllPersonsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.GettingAll, query.Skip, query.Take);

        var persons = await _contactUnitOfWork.Persons.GetAllWithContactInfosPagedAsync(query.Skip, query.Take, cancellationToken);
        
        return persons.ToList();
    }
}
