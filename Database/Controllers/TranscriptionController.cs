using Microsoft.AspNetCore.Mvc;
using Serilog;
using Shared.Entities;
using Database.Services;

namespace Database.Controllers;

[ApiController]
[Route("[controller]")]
public class TranscriptionController : ControllerBase
{
    private readonly Serilog.ILogger logger = Log.ForContext<TranscriptionController>();

    [HttpGet]
    public async Task<ActionResult<Transcription>> FindTranscription(TranscriptionService transcriptionService, [FromQuery] string url)
    {
        logger.Information("GET request for find transcription with url: {Url}", url);

        try
        {
            var transcription = await transcriptionService.FindTranscription(url);

            if (transcription != null)
            {
                logger.Information("Found transcription with url: {Url}", url);
                return transcription;
            }

            logger.Information("No transcription found with url: {Url}", url);
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.Error("An error occurred while finding a transcription with url: {Url}", url);
            return Problem(ex.Message, nameof(this.FindTranscription), 500, $"An error occurred while finding a transcription with url: {url}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Transcription>> CreateTranscription(
        TranscriptionService transcriptionService, Transcription transcription)
    {
        logger.Information("POST request for save transcription with url: {Url}",  transcription.Url);

        try
        {
            await transcriptionService.SaveTranscription(transcription);
            return CreatedAtAction(nameof(FindTranscription), new { Url = transcription.Url }, transcription);
        }
        catch (Exception ex)
        {
            logger.Error("An error occurred while saving a transcription with url: {Url}", transcription.Url);
            return Problem(ex.Message, nameof(this.CreateTranscription), 500,  "An error occurred while saving a transcription with url: {Url}");
        }
    }
}
