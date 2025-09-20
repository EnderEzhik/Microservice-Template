using System.Text.Json.Serialization;

namespace Shared.Entities;

public class Transcription
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}
