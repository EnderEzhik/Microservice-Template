using TranscriptionsWorker.CustomErrors;

namespace TranscriptionsWorker.Services;

public class TranscriptionService(DatabaseService databaseService)
{
    private readonly Serilog.ILogger logger = Serilog.Log.ForContext<TranscriptionService>();

    public async Task<Shared.Entities.Transcription?> FindTranscription(string url)
    {
        logger.Information("Finding transcription for url: {Url}", url);

        try
        {
            var transcription = await databaseService.GetTranscription(url);
            if (transcription != null)
            {
                logger.Information("Transcription found for url: {Url}", url);
                return transcription;
            }

            logger.Information("Transcription not found for url: {Url}", url);
            return null;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error finding transcription for url: {Url}", url);
            throw;
        }
    }
    
    public async Task<Shared.Entities.Transcription> CreateTranscription(string url)
    {
        logger.Information("Creating transcription for url: {Url}", url);

        try
        {
            var transcriptionText = await CreateTestTranscription();

            var transcription = new Shared.Entities.Transcription()
            {
                Url = url,
                Content = transcriptionText
            };

            logger.Information("Transcription created for url: {Url}", url);

            await databaseService.SaveTranscription(transcription);
            logger.Information("Transcription saved for url: {Url}", url);
            return transcription;
        }
        catch (TranscriptionSaveException ex)
        {
            logger.Error(ex, "Failed saving transcription for url: {Url}", url);
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed creating transcription for url: {Url}", url);
            throw;
        }
    }
    
    private async Task<string> CreateTestTranscription()
    {
        string[] testTranscriptions = [
            "Всем привет! Сегодня мы построим 1000 домов для белок",
            "Это очень умное и крутое видео, досмотрите его до конца",
            "Давайте сегодня посмотрим на что способен наш новый робот и дадим ему этот карандаш"
        ];
            
        var index = Random.Shared.Next(testTranscriptions.Length);
        return testTranscriptions[index];
    }
}