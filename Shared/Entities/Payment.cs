using System.Text.Json.Serialization;

namespace Shared.Entities;

public class Payment
{
    [JsonPropertyName("guid")]
    public string Guid { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("paidUrl")]
    public string PaidUrl { get; set; }
}
