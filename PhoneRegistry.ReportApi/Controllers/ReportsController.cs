using Microsoft.AspNetCore.Mvc;
using PhoneRegistry.Services.Interfaces;

namespace PhoneRegistry.ReportApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost]
    public async Task<IActionResult> RequestReport()
    {
        var result = await _reportService.RequestReportAsync();
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        var result = await _reportService.GetAllReportsAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _reportService.GetReportByIdAsync(id);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }
}
