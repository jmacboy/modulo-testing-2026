using System.Text.Json.Serialization;

namespace QueueConsumerApp;

public record OrderCreatedEvent(
    [property: JsonPropertyName("orderId")] int OrderId,
    [property: JsonPropertyName("total")] decimal Total,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt);
