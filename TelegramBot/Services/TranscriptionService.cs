using System.Net.Http.Json;
using System.Text.Json;
using Serilog;

namespace TelegramBot.Services;

public class TranscriptionService
{
    private readonly ILogger logger;
    private readonly HttpClient httpClient;

    public TranscriptionService(HttpClient _httpClient)
    {
        logger = Log.ForContext<TranscriptionService>();
        httpClient = _httpClient;
    }

    public async Task<string> ProcessTranscription(string url)
    {
        logger.Information("Processing transcription for url: {Url}", url);

        try
        {
            var transcription = await FindTranscription(url);
            if (transcription != null)
            {
                return transcription;
            }

            // Если транскрипция не найдена, создаем новую
            return await CreateTranscription(url);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error processing transcription for url: {Url}", url);
            throw;
        }
    }

    private async Task<string?> FindTranscription(string url)
    {
        logger.Information("Finding transcription for url: {Url}", url);

        try
        {
            var response = await httpClient.GetAsync($"transcription?url={url}");
            if (response.IsSuccessStatusCode)
            {
                logger.Information("Successfully found Transcription for url: {Url}", url);
                var content = await response.Content.ReadAsStringAsync();
                var transcription = JsonSerializer.Deserialize<Shared.Models.Transcription>(content)!;
                return transcription.Content;
            }

            logger.Information("Transcription not found for url: {Url}", url);
            return null;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error finding transcription for url: {Url}", url);
            return null;
        }
    }

    private async Task<string> CreateTranscription(string url)
    {
        logger.Information("Creating transcription for url: {Url}", url);

        try
        {
            var response = await httpClient.PostAsJsonAsync("transcription", new { url = url });
            if (response.IsSuccessStatusCode)
            {
                logger.Information("Successfully created Transcription for url: {Url}", url);
                var content = await response.Content.ReadAsStringAsync();
                var transcription = JsonSerializer.Deserialize<Shared.Models.Transcription>(content)!;
                return transcription.Content;
            }

            logger.Error("Failed to create transcription for URL: {Url}. Status: {StatusCode}", url, response.StatusCode);
            throw new HttpRequestException($"Failed to create transcription. Status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error creating transcription for url: {Url}", url);
            throw;
        }
    }
}
