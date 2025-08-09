using MediatR;
using PhoneRegistry.Application.Common.DTOs;

namespace PhoneRegistry.Application.Features.Persons.Commands.CreatePerson;

public class CreatePersonCommand : IRequest<PersonDto>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Company { get; set; }
}
