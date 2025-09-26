namespace TranscriptionsWorker.CustomErrors;

public class TranscriptionSaveException : Exception
{
    public TranscriptionSaveException(string message) : base(message) {}
}