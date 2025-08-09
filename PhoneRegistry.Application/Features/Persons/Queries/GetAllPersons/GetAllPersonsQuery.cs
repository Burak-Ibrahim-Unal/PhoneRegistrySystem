using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Application.Features.Persons.Queries.GetAllPersons;

public class GetAllPersonsQuery : IQuery<List<Person>>
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}
