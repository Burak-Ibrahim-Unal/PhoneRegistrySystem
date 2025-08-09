using MediatR;

namespace PhoneRegistry.Application.Features.Persons.Commands.DeletePerson;

public class DeletePersonCommand : IRequest
{
    public Guid PersonId { get; set; }
}
