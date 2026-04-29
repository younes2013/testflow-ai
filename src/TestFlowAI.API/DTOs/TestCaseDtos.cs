using TestFlowAI.API.Models;

namespace TestFlowAI.API.DTOs;

public record CreateTestCaseRequest(
    string Title,
    string Description,
    string Preconditions,
    string Steps,
    string ExpectedResult,
    TestCasePriority Priority,
    int CampaignId);

public record UpdateTestCaseStatusRequest(
    TestCaseStatus Status,
    string? FailureReason);

public record GenerateTestCasesRequest(
    string ComponentDescription,
    string? Model);
