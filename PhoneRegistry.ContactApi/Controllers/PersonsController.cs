using Microsoft.AspNetCore.Mvc;
using PhoneRegistry.Services.Interfaces;
using System.Linq;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.ValueObjects;

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
        var persons = await _personService.GetAllPersonsAsync(skip, take);
        var dto = persons.Select(p => new PersonDto
        {
            id = p.Id,
            firstName = p.FirstName,
            lastName = p.LastName,
            company = p.Company,
            contactInfos = p.ContactInfos.Select(ci => new ContactInfoDto
            {
                id = ci.Id,
                type = (int)ci.Type,
                content = ci.Content,
                isDeleted = ci.IsDeleted,
                cityName = ci.Type == ContactType.Location ? ci.City?.Name : null
            }).ToList()
        });
        return Ok(dto);
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
        try
        {
            var result = await _personService.AddContactInfoAsync(id, request.Type, request.Content, request.CityId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, details = ex.ToString() });
        }
    }

    [HttpDelete("{personId}/contact-infos/{contactInfoId}")]
    public async Task<IActionResult> RemoveContactInfo(Guid personId, Guid contactInfoId)
    {
        await _personService.RemoveContactInfoAsync(personId, contactInfoId);
        return NoContent();
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
    public Guid? CityId { get; set; }
}

public class PersonDto
{
    public Guid id { get; set; }
    public string firstName { get; set; } = string.Empty;
    public string lastName { get; set; } = string.Empty;
    public string? company { get; set; }
    public List<ContactInfoDto> contactInfos { get; set; } = new();
}

public class ContactInfoDto
{
    public Guid id { get; set; }
    public int type { get; set; }
    public string content { get; set; } = string.Empty;
    public bool isDeleted { get; set; }
    public string? cityName { get; set; }
}
