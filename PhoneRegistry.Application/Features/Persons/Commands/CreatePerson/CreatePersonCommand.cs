using PhoneRegistry.Application.Common.DTOs;
using PhoneRegistry.Application.Common.Interfaces;

namespace PhoneRegistry.Application.Features.Persons.Commands.CreatePerson;

public class CreatePersonCommand : ICommand<PersonDto>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Company { get; set; }
}
