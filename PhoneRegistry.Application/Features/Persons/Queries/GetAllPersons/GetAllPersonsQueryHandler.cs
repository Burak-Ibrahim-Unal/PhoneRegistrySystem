using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Domain.Repositories;

namespace PhoneRegistry.Application.Features.Persons.Queries.GetAllPersons;

public class GetAllPersonsQueryHandler : IRequestHandler<GetAllPersonsQuery, List<PersonSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllPersonsQueryHandler> _logger;

    public GetAllPersonsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllPersonsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<PersonSummaryDto>> Handle(GetAllPersonsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all persons with skip: {Skip}, take: {Take}", query.Skip, query.Take);

        var persons = await _unitOfWork.Persons.GetAllWithContactInfosAsync(query.Skip, query.Take, cancellationToken);
        
        return _mapper.Map<List<PersonSummaryDto>>(persons);
    }
}
