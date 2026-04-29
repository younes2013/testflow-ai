using Microsoft.EntityFrameworkCore;
using TestFlowAI.API.Data;
using TestFlowAI.API.Models;

namespace TestFlowAI.API.Services;

public class CampaignService(AppDbContext db)
{
    public async Task<List<Campaign>> GetAllAsync() =>
        await db.Campaigns
                .Include(c => c.TestCases)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

    public async Task<Campaign?> GetByIdAsync(int id) =>
        await db.Campaigns
                .Include(c => c.TestCases)
                .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Campaign> CreateAsync(Campaign campaign)
    {
        db.Campaigns.Add(campaign);
        await db.SaveChangesAsync();
        return campaign;
    }

    public async Task<Campaign?> UpdateAsync(int id, Campaign updated)
    {
        var campaign = await db.Campaigns.FindAsync(id);
        if (campaign is null) return null;

        campaign.Name = updated.Name;
        campaign.Description = updated.Description;
        campaign.TargetSystem = updated.TargetSystem;
        campaign.Status = updated.Status;

        if (updated.Status == CampaignStatus.InProgress && campaign.StartedAt is null)
            campaign.StartedAt = DateTime.UtcNow;

        if (updated.Status == CampaignStatus.Completed && campaign.CompletedAt is null)
            campaign.CompletedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return campaign;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var campaign = await db.Campaigns.FindAsync(id);
        if (campaign is null) return false;

        db.Campaigns.Remove(campaign);
        await db.SaveChangesAsync();
        return true;
    }
}
