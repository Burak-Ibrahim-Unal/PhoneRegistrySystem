using Microsoft.AspNetCore.Mvc;
using PhoneRegistry.Application.Features.Persons.Commands.CreatePerson;
using PhoneRegistry.Application.Features.Persons.Commands.DeletePerson;
using PhoneRegistry.Application.Features.Persons.Queries.GetPersonById;
using PhoneRegistry.Application.Features.Persons.Queries.GetAllPersons;
using PhoneRegistry.Application.Features.ContactInfos.Commands.AddContactInfo;

namespace PhoneRegistry.ContactApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly CreatePersonCommandHandler _createPersonHandler;
    private readonly DeletePersonCommandHandler _deletePersonHandler;
    private readonly GetPersonByIdQueryHandler _getPersonByIdHandler;
    private readonly GetAllPersonsQueryHandler _getAllPersonsHandler;
    private readonly AddContactInfoCommandHandler _addContactInfoHandler;

    public PersonsController(
        CreatePersonCommandHandler createPersonHandler,
        DeletePersonCommandHandler deletePersonHandler,
        GetPersonByIdQueryHandler getPersonByIdHandler,
        GetAllPersonsQueryHandler getAllPersonsHandler,
        AddContactInfoCommandHandler addContactInfoHandler)
    {
        _createPersonHandler = createPersonHandler;
        _deletePersonHandler = deletePersonHandler;
        _getPersonByIdHandler = getPersonByIdHandler;
        _getAllPersonsHandler = getAllPersonsHandler;
        _addContactInfoHandler = addContactInfoHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        var query = new GetAllPersonsQuery { Skip = skip, Take = take };
        var result = await _getAllPersonsHandler.HandleAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetPersonByIdQuery { PersonId = id };
        var result = await _getPersonByIdHandler.HandleAsync(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonCommand command)
    {
        var result = await _createPersonHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeletePersonCommand { PersonId = id };
        await _deletePersonHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpPost("{id}/contact-infos")]
    public async Task<IActionResult> AddContactInfo(Guid id, [FromBody] AddContactInfoCommand command)
    {
        command.PersonId = id;
        var result = await _addContactInfoHandler.HandleAsync(command);
        return Ok(result);
    }
}
