using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Application.Common.Interfaces;

namespace PhoneRegistry.Application.Features.Persons.Queries.GetPersonById;

public class GetPersonByIdQuery : IQuery<PersonDto?>
{
    public Guid PersonId { get; set; }
}
