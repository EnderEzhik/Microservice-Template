using System.Text.Json;
using Serilog;

namespace TranscriptionsWorker.Services;

public class DatabaseService(HttpClient httpClient)
{
    private readonly Serilog.ILogger logger = Log.ForContext<DatabaseService>();

    public async Task<Shared.Entities.Transcription?> GetTranscription(string url)
    {
        logger.Information("Getting transcription for url: {Url}", url);
        var transcription = await httpClient.GetFromJsonAsync<Shared.Entities.Transcription>($"transcriptions?url={url}");
        return transcription;
    }

    public async Task SaveTranscription(Shared.Entities.Transcription transcription)
    {
        logger.Information("Saving transcription");
        var response = await httpClient.PostAsJsonAsync($"transcriptions", transcription);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Failed to save transcription. Status: {response.StatusCode}");
        }
    }
}