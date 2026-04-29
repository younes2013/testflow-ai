namespace TestFlowAI.API.Models;

public class TestCase
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Preconditions { get; set; } = string.Empty;
    public string Steps { get; set; } = string.Empty;
    public string ExpectedResult { get; set; } = string.Empty;
    public TestCasePriority Priority { get; set; } = TestCasePriority.Medium;
    public TestCaseStatus Status { get; set; } = TestCaseStatus.NotRun;
    public bool IsAiGenerated { get; set; } = false;
    public double RiskScore { get; set; } = 0.0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExecutedAt { get; set; }
    public string? FailureReason { get; set; }

    public int CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
}

public enum TestCasePriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum TestCaseStatus
{
    NotRun,
    InProgress,
    Passed,
    Failed,
    Blocked,
    Skipped
}
