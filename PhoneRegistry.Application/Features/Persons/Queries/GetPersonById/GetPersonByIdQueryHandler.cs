using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Persons.Queries.GetPersonById;

public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, PersonDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPersonByIdQueryHandler> _logger;

    public GetPersonByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetPersonByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PersonDto?> Handle(GetPersonByIdQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting person by ID: {PersonId}", query.PersonId);

        var person = await _unitOfWork.Persons.GetByIdWithContactInfosAsync(query.PersonId, cancellationToken);
        
        if (person == null)
        {
            _logger.LogWarning("Person not found: {PersonId}", query.PersonId);
            return null;
        }

        return _mapper.Map<PersonDto>(person);
    }
}
