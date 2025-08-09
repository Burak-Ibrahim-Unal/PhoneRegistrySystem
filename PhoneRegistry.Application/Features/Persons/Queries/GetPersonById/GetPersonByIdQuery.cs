using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Application.Features.Persons.Queries.GetPersonById;

public class GetPersonByIdQuery : IQuery<Person?>
{
    public Guid PersonId { get; set; }
}
