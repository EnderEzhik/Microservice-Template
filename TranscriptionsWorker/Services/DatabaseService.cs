using System.Text.Json;
using Serilog;

namespace TranscriptionsWorker.Services;

public class DatabaseService
{
    private readonly Serilog.ILogger logger;
    private readonly HttpClient httpClient;

    public DatabaseService(HttpClient _httpClient)
    {
        logger = Log.ForContext<DatabaseService>();
        httpClient = _httpClient;
    }
    
    public async Task<Shared.Entities.Transcription?> GetTranscription(string url)
    {
        logger.Information("Finding transcription");

        try
        {
            var response = await httpClient.GetAsync($"transcription?url={url}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var transcription = JsonSerializer.Deserialize<Shared.Entities.Transcription>(json)!;
                return transcription;
            }

            logger.Information("Transcription not found");
            return null;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error finding transcription");
            return null;
        }
    }

    public async Task SaveTranscription(Shared.Entities.Transcription transcription)
    {
        logger.Information("Saving transcription");

        try
        {
            var response = await httpClient.PostAsJsonAsync($"transcription", transcription);
            if (response.IsSuccessStatusCode)
            {
                logger.Information("Transcription successful saved");
                return;
            }
            
            logger.Error("Failed to save transcription for URL: {Url}. Status: {StatusCode}", transcription.Url, response.StatusCode);
            throw new HttpRequestException($"Failed to create transcription. Status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error saving transcription");
        }
    }
}