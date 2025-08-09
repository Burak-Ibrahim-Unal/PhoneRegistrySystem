using MediatR;
using PhoneRegistry.Application.Common.DTOs;

namespace PhoneRegistry.Application.Features.Persons.Queries.GetPersonById;

public class GetPersonByIdQuery : IRequest<PersonDto?>
{
    public Guid PersonId { get; set; }
}
