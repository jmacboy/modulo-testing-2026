using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Inventory.IntTest
{
    // El shape real del mensaje que este productor publica. Antes vivía como un string embebido
    // dentro de produceEvent(), sin forma de reutilizarlo fuera de una publicación real a
    // RabbitMQ. Extraerlo a un tipo con su propio método de construcción es lo que permite
    // verificar el mensaje real contra un pacto sin tocar el broker — ver
    // MessagePactProvider/OrderCreatedEventProviderPactTests.cs.
    public record OrderCreatedEvent(int OrderId, decimal Total, DateTime CreatedAt);

    public class EventProduce
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Mismos valores de ejemplo que tenía el string embebido — el mensaje que llega a
        // RabbitMQ no cambia, solo se vuelve testeable sin RabbitMQ de por medio.
        public OrderCreatedEvent BuildOrderCreatedEvent() =>
            new(OrderId: 123, Total: 250.50m, CreatedAt: DateTime.Now);

        public string SerializeOrderCreatedEvent(OrderCreatedEvent orderCreated) =>
            JsonSerializer.Serialize(orderCreated, JsonOptions);

        public async Task produceEvent()
        {

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // 1. Exchange
            await channel.ExchangeDeclareAsync(
                exchange: "orders.exchange",
                type: ExchangeType.Direct,
                durable: true
            );

            // 2. Queue
            await channel.QueueDeclareAsync(
                queue: "orders.created.queue",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // 3. Binding
            await channel.QueueBindAsync(
                queue: "orders.created.queue",
                exchange: "orders.exchange",
                routingKey: "order.created"
            );

            // 4. Evento
            var body = Encoding.UTF8.GetBytes(SerializeOrderCreatedEvent(BuildOrderCreatedEvent()));

            // 5. Publicar
            await channel.BasicPublishAsync(
                exchange: "orders.exchange",
                routingKey: "order.created",
                mandatory: false,
                body: body
            );
        }

    }
}
