using System.Text.Json.Serialization;

namespace Shared.Models;

public class Transcription
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
    
    [JsonPropertyName("content")]
    public string Content { get; set; }
}