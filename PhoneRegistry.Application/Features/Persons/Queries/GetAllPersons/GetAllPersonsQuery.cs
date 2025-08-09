using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Application.Common.Interfaces;

namespace PhoneRegistry.Application.Features.Persons.Queries.GetAllPersons;

public class GetAllPersonsQuery : IQuery<List<PersonSummaryDto>>
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}
