using System.Text.Json.Serialization;

namespace Shared.Models;

public class Transcription
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("url")]
    public string Url { get; set; }
    
    [JsonPropertyName("content")]
    public string Content { get; set; }
}