using RabbitMQ.Client;

namespace IntegrationTesterApp
{
    public class RabbitMqListenerFixture : IDisposable
    {
        public IConnection Connection { get; private set; }
        public IChannel Channel { get; private set; }
        public string QueueName { get; private set; }

        public RabbitMqListenerFixture()
        {
            init().GetAwaiter().GetResult();
        }

        private async Task init()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            Connection = await factory.CreateConnectionAsync();
            Channel = await Connection.CreateChannelAsync();

            // Queue temporal
            QueueName = (await Channel.QueueDeclareAsync(
                queue: "",
                durable: false,
                exclusive: true,
                autoDelete: true
            )).QueueName;

            // Bind to the real exchange
            await Channel.QueueBindAsync(
                queue: QueueName,
                exchange: "orders.exchange",
                routingKey: "order.created"
            );
        }

        private async Task DisposeAsync()
        {
            if (Channel != null)
            {
                await Channel.CloseAsync();
            }

            if (Connection != null)
            {
                await Connection.CloseAsync();
            }
        }

        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
