using MediatR;
using PhoneRegistry.Application.Common.DTOs;

namespace PhoneRegistry.Application.Features.Persons.Queries.GetAllPersons;

public class GetAllPersonsQuery : IRequest<List<PersonSummaryDto>>
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}
