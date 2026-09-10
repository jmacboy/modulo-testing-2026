using System.Text.Json;

namespace QueueConsumerApp;

public class OrderCreatedEventHandler
{
    public void Handle(string rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            throw new ArgumentException("OrderCreatedEvent JSON must not be null or empty.", nameof(rawJson));
        }

        OrderCreatedEvent orderCreated;

        try
        {
            orderCreated = JsonSerializer.Deserialize<OrderCreatedEvent>(rawJson)
                ?? throw new InvalidOperationException("OrderCreatedEvent JSON must not be null.");
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException("OrderCreatedEvent JSON is invalid.", exception);
        }

        if (orderCreated.OrderId <= 0)
        {
            throw new InvalidOperationException("orderId must be positive");
        }

        if (orderCreated.Total <= 0)
        {
            throw new InvalidOperationException("total must be greater than zero");
        }
    }
}
