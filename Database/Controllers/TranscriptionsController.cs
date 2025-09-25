using Microsoft.AspNetCore.Mvc;
using Serilog;
using Shared.Entities;
using Database.Services;

namespace Database.Controllers;

[ApiController]
[Route("[controller]")]
public class TranscriptionsController : ControllerBase
{
    private readonly Serilog.ILogger logger = Log.ForContext<TranscriptionsController>();

    [HttpGet]
    public async Task<ActionResult<Transcription>> GetTranscription(TranscriptionService transcriptionService, [FromQuery] string url)
    {
        logger.Information("GET request for get transcription for url: {Url}", url);

        try
        {
            var transcription = await transcriptionService.GetTranscription(url);

            if (transcription != null)
            {
                logger.Information("Found transcription for url: {Url}", url);
                return transcription;
            }

            logger.Information("Transcription not found for url: {Url}", url);
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while getting a transcription for url: {Url}", url);
            return Problem(ex.Message, nameof(this.GetTranscription), 500, $"An error occurred while getting a transcription for url: {url}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Transcription>> SaveTranscription(TranscriptionService transcriptionService, Transcription transcription)
    {
        logger.Information("POST request for save transcription for url: {Url}",  transcription.Url);

        try
        {
            await transcriptionService.SaveTranscription(transcription);
            logger.Information("Transcription saved successfully for url: {Url}", transcription.Url);
            return CreatedAtAction(nameof(GetTranscription), new { Url = transcription.Url }, transcription);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while saving a transcription for url: {Url}", transcription.Url);
            return Problem(ex.Message, nameof(this.SaveTranscription), 500,  $"An error occurred while saving a transcription for url: {transcription.Url}");
        }
    }
}
