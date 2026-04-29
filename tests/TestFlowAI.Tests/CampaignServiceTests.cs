using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestFlowAI.API.Data;
using TestFlowAI.API.Models;
using TestFlowAI.API.Services;

namespace TestFlowAI.Tests;

public class CampaignServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly CampaignService _service;

    public CampaignServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _service = new CampaignService(_db);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistCampaign()
    {
        var campaign = new Campaign
        {
            Name = "Campagne CAN Bus",
            Description = "Tests du bus CAN",
            TargetSystem = "ECU v2.1"
        };

        var result = await _service.CreateAsync(campaign);

        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Campagne CAN Bus");
        result.Status.Should().Be(CampaignStatus.Draft);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCampaigns()
    {
        await _db.Campaigns.AddRangeAsync(
            new Campaign { Name = "Camp 1", TargetSystem = "ECU-A" },
            new Campaign { Name = "Camp 2", TargetSystem = "ECU-B" });
        await _db.SaveChangesAsync();

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ShouldReturnNull()
    {
        var result = await _service.GetByIdAsync(999);
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetStartedAtWhenStatusBecomesInProgress()
    {
        var campaign = new Campaign { Name = "Camp", TargetSystem = "ECU" };
        await _db.Campaigns.AddAsync(campaign);
        await _db.SaveChangesAsync();

        var updated = await _service.UpdateAsync(campaign.Id, new Campaign
        {
            Name = "Camp",
            Description = "",
            TargetSystem = "ECU",
            Status = CampaignStatus.InProgress
        });

        updated.Should().NotBeNull();
        updated!.StartedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCampaign()
    {
        var campaign = new Campaign { Name = "Camp à supprimer", TargetSystem = "ECU" };
        await _db.Campaigns.AddAsync(campaign);
        await _db.SaveChangesAsync();

        var result = await _service.DeleteAsync(campaign.Id);

        result.Should().BeTrue();
        (await _db.Campaigns.FindAsync(campaign.Id)).Should().BeNull();
    }

    public void Dispose() => _db.Dispose();
}
