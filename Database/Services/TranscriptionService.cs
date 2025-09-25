using Database.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shared.Entities;

namespace Database.Services;

public class TranscriptionService(MyDbContext _db)
{
    private readonly Serilog.ILogger logger = Log.ForContext<TranscriptionService>();
    private readonly MyDbContext db = _db;

    public async Task<Transcription?> GetTranscription(string url)
    {
        logger.Information("Getting transcription with url: {Url}", url);
        var transcription = await db.Transcriptions.AsNoTracking().FirstOrDefaultAsync(t => t.Url == url);
        return transcription;
    }

    public async Task SaveTranscription(Transcription transcription)
    {
        logger.Information("Saving transcription");
        db.Transcriptions.Add(transcription);
        await db.SaveChangesAsync();
    }
}
