using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TestFlowAI.API.Data;
using TestFlowAI.API.Models;

namespace TestFlowAI.API.Services;

public class TestCaseService(AppDbContext db, OllamaService ollama, ILogger<TestCaseService> logger)
{
    public async Task<List<TestCase>> GetByCampaignAsync(int campaignId) =>
        await db.TestCases
                .Where(t => t.CampaignId == campaignId)
                .OrderByDescending(t => t.RiskScore)
                .ThenBy(t => t.Priority)
                .ToListAsync();

    public async Task<TestCase?> GetByIdAsync(int id) =>
        await db.TestCases.FindAsync(id);

    public async Task<TestCase> CreateAsync(TestCase testCase)
    {
        db.TestCases.Add(testCase);
        await db.SaveChangesAsync();
        return testCase;
    }

    public async Task<TestCase?> UpdateStatusAsync(int id, TestCaseStatus status, string? failureReason = null)
    {
        var testCase = await db.TestCases.FindAsync(id);
        if (testCase is null) return null;

        testCase.Status = status;
        testCase.ExecutedAt = DateTime.UtcNow;

        if (status == TestCaseStatus.Failed)
            testCase.FailureReason = failureReason;

        await db.SaveChangesAsync();
        return testCase;
    }

    public async Task<List<TestCase>> GenerateFromDescriptionAsync(int campaignId, string componentDescription, string? model = null)
    {
        var prompt = BuildGenerationPrompt(componentDescription);

        logger.LogInformation("Génération de cas de test pour campagne {CampaignId} via Ollama", campaignId);
        var rawResponse = await ollama.GenerateAsync(prompt, model);

        var testCases = ParseGeneratedTestCases(rawResponse, campaignId);

        db.TestCases.AddRange(testCases);
        await db.SaveChangesAsync();

        return testCases;
    }

    private static string BuildGenerationPrompt(string description) => $"""
        Tu es un expert en tests de systèmes embarqués.
        Génère 5 cas de test structurés en JSON pour le composant suivant :

        {description}

        Réponds UNIQUEMENT avec un tableau JSON valide, sans texte avant ou après.
        Format de chaque cas de test :
        {{
          "title": "Titre court et explicite",
          "description": "Description du test",
          "preconditions": "Conditions préalables nécessaires",
          "steps": "Étapes numérotées séparées par \\n",
          "expectedResult": "Résultat attendu précis",
          "priority": "Low|Medium|High|Critical",
          "riskScore": 0.0
        }}
        """;

    private static List<TestCase> ParseGeneratedTestCases(string rawJson, int campaignId)
    {
        try
        {
            var start = rawJson.IndexOf('[');
            var end = rawJson.LastIndexOf(']');
            if (start < 0 || end < 0) return [];

            var jsonArray = rawJson[start..(end + 1)];
            var items = JsonSerializer.Deserialize<List<GeneratedTestCaseDto>>(jsonArray,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return items?.Select(dto => new TestCase
            {
                Title = dto.Title,
                Description = dto.Description,
                Preconditions = dto.Preconditions,
                Steps = dto.Steps,
                ExpectedResult = dto.ExpectedResult,
                Priority = Enum.TryParse<TestCasePriority>(dto.Priority, true, out var p) ? p : TestCasePriority.Medium,
                RiskScore = dto.RiskScore,
                IsAiGenerated = true,
                CampaignId = campaignId
            }).ToList() ?? [];
        }
        catch
        {
            return [];
        }
    }

    private record GeneratedTestCaseDto(
        string Title, string Description, string Preconditions,
        string Steps, string ExpectedResult, string Priority, double RiskScore);
}
