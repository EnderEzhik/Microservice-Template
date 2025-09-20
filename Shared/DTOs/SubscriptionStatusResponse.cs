using System.Text.Json.Serialization;

namespace Shared.DTOs;

public record SubscriptionStatusResponse
{
    [JsonPropertyName("userId")]
    public long UserId { get; init; }

    [JsonPropertyName("isSubscribed")]
    public bool IsSubscribed { get; init; }
}
