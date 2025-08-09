using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Application.Features.Persons.Commands.CreatePerson;

public class CreatePersonCommand : ICommand<Person>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Company { get; set; }
}
