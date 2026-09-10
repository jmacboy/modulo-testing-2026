using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace IntegrationTesterApp
{
    public class ConsumerProducerTest
            : IClassFixture<RabbitMqListenerFixture>
    {
        private readonly RabbitMqListenerFixture _fixture;

        public ConsumerProducerTest(RabbitMqListenerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task OrderCreated_MessageArrived()
        {
            var tcs = new TaskCompletionSource<string>();

            var consumer = new AsyncEventingBasicConsumer(_fixture.Channel);

            consumer.ReceivedAsync += async (_, args) =>
            {
                var body = Encoding.UTF8.GetString(args.Body.ToArray());
                tcs.TrySetResult(body);
                await Task.CompletedTask;
            };

            await _fixture.Channel.BasicConsumeAsync(
                queue: _fixture.QueueName,
                autoAck: true,
                consumer: consumer
            );

            // Espera hasta 10 segundos
            var completed = await Task.WhenAny(
                tcs.Task,
                Task.Delay(TimeSpan.FromSeconds(10))
            );

            Assert.True(completed == tcs.Task, "No se recibió el evento");

            var message = await tcs.Task;

            Assert.Contains("\"orderId\"", message);
            Assert.Contains("\"createdAt\"", message);
        }
    }
}