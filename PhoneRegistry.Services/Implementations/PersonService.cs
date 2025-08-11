using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Constants;
using PhoneRegistry.Application.Common.Helpers;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Application.Features.Persons.Commands.CreatePerson;
using PhoneRegistry.Application.Features.Persons.Commands.DeletePerson;
using PhoneRegistry.Application.Features.Persons.Queries.GetPersonById;
using PhoneRegistry.Application.Features.Persons.Queries.GetAllPersons;
using PhoneRegistry.Application.Features.ContactInfos.Commands.AddContactInfo;
using PhoneRegistry.Application.Features.ContactInfos.Commands.RemoveContactInfo;
using PhoneRegistry.Domain.ValueObjects;
using PhoneRegistry.Services.Interfaces;
using PhoneRegistry.Caching.Interfaces;

namespace PhoneRegistry.Services.Implementations;

public class PersonService : IPersonService
{
    private readonly IMediator _mediator;
    private readonly ILogger<PersonService> _logger;
    private readonly ICacheService _cacheService;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _listCacheExpiration = TimeSpan.FromMinutes(2);

    public PersonService(IMediator mediator, ILogger<PersonService> logger, ICacheService cacheService)
    {
        _mediator = mediator;
        _logger = logger;
        _cacheService = cacheService;
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
        
        // Invalidate all persons cache when a new person is created
        await InvalidateAllPersonsCache();
        
        _logger.LogInformation(Messages.Person.CreatedSuccessfully);
        return result;
    }

    public async Task<Person?> GetPersonByIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.GettingById, personId);

        // Try to get from cache first
        var cacheKey = CacheKeyHelper.GetPersonByIdKey(personId);
        var cachedPerson = await _cacheService.GetAsync<Person>(cacheKey);
        
        if (cachedPerson != null)
        {
            _logger.LogDebug("Person {PersonId} found in cache", personId);
            return cachedPerson;
        }

        var query = new GetPersonByIdQuery { PersonId = personId };
        var result = await _mediator.Send(query, cancellationToken);
        
        // Cache the result if found
        if (result != null)
        {
            await _cacheService.SetAsync(cacheKey, result, _cacheExpiration);
            _logger.LogDebug("Person {PersonId} cached", personId);
        }

        if (result == null)
        {
            _logger.LogWarning(Messages.Person.NotFound, personId);
        }

        return result;
    }

    public async Task<List<Person>> GetAllPersonsAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.GettingAll, skip, take);

        // Create cache key with pagination parameters
        var cacheKey = CacheKeyHelper.GetAllPersonsKey(skip, take);
        var cachedPersons = await _cacheService.GetAsync<List<Person>>(cacheKey);
        
        if (cachedPersons != null)
        {
            _logger.LogDebug("Persons list found in cache (skip: {Skip}, take: {Take})", skip, take);
            return cachedPersons;
        }

        var query = new GetAllPersonsQuery { Skip = skip, Take = take };
        var result = await _mediator.Send(query, cancellationToken);
        
        // Cache the result
        await _cacheService.SetAsync(cacheKey, result, _listCacheExpiration);

        _logger.LogInformation(Messages.Person.RetrievedSuccessfully);
        return result;
    }

    public async Task DeletePersonAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(Messages.Person.Deleting, personId);

        var command = new DeletePersonCommand { PersonId = personId };
        await _mediator.Send(command, cancellationToken);
        
        // Invalidate caches
        var cacheKey = CacheKeyHelper.GetPersonByIdKey(personId);
        await _cacheService.RemoveAsync(cacheKey);
        await InvalidateAllPersonsCache();

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
        
        // Invalidate person cache when contact info is modified
        var cacheKey = CacheKeyHelper.GetPersonByIdKey(personId);
        await _cacheService.RemoveAsync(cacheKey);
        
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
        
        // Invalidate person cache when contact info is modified
        var cacheKey = CacheKeyHelper.GetPersonByIdKey(personId);
        await _cacheService.RemoveAsync(cacheKey);
        
        _logger.LogInformation(Messages.ContactInfo.RemovedSuccessfully, contactInfoId);
    }
    
    private async Task InvalidateAllPersonsCache()
    {
        // Remove all cached person lists with different pagination
        // In production, you might want to use Redis pattern matching to delete all keys with prefix
        var commonPaginationKeys = new[]
        {
            CacheKeyHelper.GetAllPersonsKey(0, 50),
            CacheKeyHelper.GetAllPersonsKey(0, 100),
            CacheKeyHelper.GetAllPersonsKey(0, 200),
            CacheKeyHelper.GetAllPersonsKey(50, 50),
            CacheKeyHelper.GetAllPersonsKey(100, 50)
        };
        
        foreach (var key in commonPaginationKeys)
        {
            await _cacheService.RemoveAsync(key);
        }
        
        _logger.LogDebug("All persons cache invalidated");
    }
}