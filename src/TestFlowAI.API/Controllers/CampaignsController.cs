using Microsoft.AspNetCore.Mvc;
using TestFlowAI.API.DTOs;
using TestFlowAI.API.Models;
using TestFlowAI.API.Services;

namespace TestFlowAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController(CampaignService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var campaign = await service.GetByIdAsync(id);
        return campaign is null ? NotFound() : Ok(campaign);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCampaignRequest request)
    {
        var campaign = new Campaign
        {
            Name = request.Name,
            Description = request.Description,
            TargetSystem = request.TargetSystem
        };

        var created = await service.CreateAsync(campaign);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCampaignRequest request)
    {
        var updated = await service.UpdateAsync(id, new Campaign
        {
            Name = request.Name,
            Description = request.Description,
            TargetSystem = request.TargetSystem,
            Status = request.Status
        });

        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
