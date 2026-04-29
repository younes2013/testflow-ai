using System.Text;
using System.Text.Json;

namespace TestFlowAI.API.Services;

public class OllamaService(HttpClient httpClient, IConfiguration configuration, ILogger<OllamaService> logger)
{
    private readonly string _baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
    private readonly string _defaultModel = configuration["Ollama:DefaultModel"] ?? "llama3";

    public async Task<string> GenerateAsync(string prompt, string? model = null)
    {
        var requestBody = new
        {
            model = model ?? _defaultModel,
            prompt,
            stream = false
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync($"{_baseUrl}/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OllamaResponse>(responseJson);
            return result?.Response ?? string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de l'appel à Ollama");
            throw;
        }
    }

    public async Task<List<string>> ListModelsAsync()
    {
        try
        {
            var response = await httpClient.GetAsync($"{_baseUrl}/api/tags");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OllamaTagsResponse>(json);
            return result?.Models?.Select(m => m.Name).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la récupération des modèles Ollama");
            return [];
        }
    }

    private record OllamaResponse(
        [property: JsonPropertyName("response")] string Response);

    private record OllamaModel(
        [property: JsonPropertyName("name")] string Name);

    private record OllamaTagsResponse(
        [property: JsonPropertyName("models")] List<OllamaModel> Models);
}
