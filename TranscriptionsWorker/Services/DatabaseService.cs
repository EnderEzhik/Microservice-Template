using Serilog;
using TranscriptionsWorker.CustomErrors;

namespace TranscriptionsWorker.Services;

public class DatabaseService(HttpClient httpClient)
{
    private readonly Serilog.ILogger logger = Log.ForContext<DatabaseService>();

    public async Task<Shared.Entities.Transcription?> GetTranscription(string url)
    {
        logger.Information("Getting transcription for url: {Url}", url);
        var response = await httpClient.GetAsync($"transcriptions?url={url}");
        if (response.IsSuccessStatusCode)
        {
            var transcription = await response.Content.ReadFromJsonAsync<Shared.Entities.Transcription>();
            return transcription!;
        }
        return null;
    }

    public async Task SaveTranscription(Shared.Entities.Transcription transcription)
    {
        logger.Information("Saving transcription");
        var response = await httpClient.PostAsJsonAsync($"transcriptions", transcription);
        if (!response.IsSuccessStatusCode)
        {
            throw new TranscriptionSaveException($"Failed to save transcription. Status: {response.StatusCode}");
        }
    }
}