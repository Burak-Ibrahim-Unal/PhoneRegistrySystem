using PhoneRegistry.Application.Common.Interfaces;

namespace PhoneRegistry.Application.Features.Persons.Commands.DeletePerson;

public class DeletePersonCommand : ICommand
{
    public Guid PersonId { get; set; }
}
