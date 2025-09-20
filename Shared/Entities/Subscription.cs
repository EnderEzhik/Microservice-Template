using System.Text.Json.Serialization;

namespace Shared.Entities;

public class Subscription
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("isCanceled")]
    public bool IsCanceled { get; set; }

    [JsonPropertyName("paymentGuid")]
    public string PaymentGuid { get; set; }

}
