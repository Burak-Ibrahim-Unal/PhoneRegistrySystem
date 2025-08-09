using Microsoft.AspNetCore.Mvc;
using PhoneRegistry.Services.Interfaces;

namespace PhoneRegistry.ContactApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly IPersonService _personService;

    public PersonsController(IPersonService personService)
    {
        _personService = personService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        var result = await _personService.GetAllPersonsAsync(skip, take);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _personService.GetPersonByIdAsync(id);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonRequest request)
    {
        var result = await _personService.CreatePersonAsync(request.FirstName, request.LastName, request.Company);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _personService.DeletePersonAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/contact-infos")]
    public async Task<IActionResult> AddContactInfo(Guid id, [FromBody] AddContactInfoRequest request)
    {
        var result = await _personService.AddContactInfoAsync(id, request.Type, request.Content);
        return Ok(result);
    }
}

// Request DTOs
public class CreatePersonRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Company { get; set; }
}

public class AddContactInfoRequest
{
    public int Type { get; set; }
    public string Content { get; set; } = string.Empty;
}
