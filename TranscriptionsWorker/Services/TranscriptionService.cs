namespace TranscriptionsWorker.Services;

public class TranscriptionService
{
    private readonly Serilog.ILogger logger;
    private readonly DatabaseService databaseService;

    public TranscriptionService(DatabaseService _databaseService)
    {
        logger = Serilog.Log.ForContext<TranscriptionService>();
        databaseService = _databaseService;
    }

    public async Task<Shared.Entities.Transcription?> FindTranscription(string url)
    {
        return await databaseService.GetTranscription(url);
    }
    
    public async Task<Shared.Entities.Transcription> CreateTranscription(string url)
    {
        logger.Information("Creating transcription for url: {Url}", url);

        try
        {
            // Код создающий транскрипцию для указанного url
            // Можно подключить сторонний сервис или свою библиотеку
            var transcriptionText = await CreateTestTranscription();

            var transcription = new Shared.Entities.Transcription()
            {
                Url = url,
                Content = transcriptionText
            };
            
            await databaseService.SaveTranscription(transcription);
            return transcription;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error creating transcription for url: {Url}", url);
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