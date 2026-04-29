using TestFlowAI.API.Models;

namespace TestFlowAI.API.DTOs;

public record CreateCampaignRequest(
    string Name,
    string Description,
    string TargetSystem);

public record UpdateCampaignRequest(
    string Name,
    string Description,
    string TargetSystem,
    CampaignStatus Status);

public record CampaignSummaryDto(
    int Id,
    string Name,
    string TargetSystem,
    CampaignStatus Status,
    int TotalTests,
    int PassedTests,
    int FailedTests,
    DateTime CreatedAt);
