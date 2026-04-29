using Microsoft.AspNetCore.Mvc;
using TestFlowAI.API.DTOs;
using TestFlowAI.API.Models;
using TestFlowAI.API.Services;

namespace TestFlowAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestCasesController(TestCaseService service) : ControllerBase
{
    [HttpGet("campaign/{campaignId:int}")]
    public async Task<IActionResult> GetByCampaign(int campaignId) =>
        Ok(await service.GetByCampaignAsync(campaignId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var testCase = await service.GetByIdAsync(id);
        return testCase is null ? NotFound() : Ok(testCase);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestCaseRequest request)
    {
        var testCase = new TestCase
        {
            Title = request.Title,
            Description = request.Description,
            Preconditions = request.Preconditions,
            Steps = request.Steps,
            ExpectedResult = request.ExpectedResult,
            Priority = request.Priority,
            CampaignId = request.CampaignId
        };

        var created = await service.CreateAsync(testCase);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTestCaseStatusRequest request)
    {
        var updated = await service.UpdateStatusAsync(id, request.Status, request.FailureReason);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("campaign/{campaignId:int}/generate")]
    public async Task<IActionResult> GenerateWithAI(int campaignId, [FromBody] GenerateTestCasesRequest request)
    {
        var generated = await service.GenerateFromDescriptionAsync(
            campaignId, request.ComponentDescription, request.Model);

        return Ok(generated);
    }
}
