using System.Text.Json.Serialization;

namespace Shared.DTOs;

public class TranscriptionCreateRequest
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}