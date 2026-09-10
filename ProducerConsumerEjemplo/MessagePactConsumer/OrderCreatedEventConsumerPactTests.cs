using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Matchers;
using PactNet.Output.Xunit;
using System.Text.Json;
using Xunit.Abstractions;
using QueueConsumerApp;

namespace MessagePactConsumer;

// Pact de mensaje — la mitad "Play & Record" del diagrama de la lámina 19, aplicada al seam
// asincrónico en vez de al HTTP. Mismo mecanismo que demo-01-pactnet-consumer/: se declara qué
// mensaje se espera recibir, se corre el handler REAL contra él, y Pact graba el pacto — sin
// tocar RabbitMQ en ningún momento. MessagePactProvider/ lo verifica después contra el productor
// real de este mismo ejemplo (EventProduce, en QueueProducerApp).
public class OrderCreatedEventConsumerPactTests
{
    private readonly IMessagePactBuilderV4 _messagePact;

    public OrderCreatedEventConsumerPactTests(ITestOutputHelper output)
    {
        var config = new PactConfig
        {
            PactDir = "../../../pacts/",
            Outputters = new IOutput[] { new XunitOutput(output) }
        };

        IPactV4 pact = Pact.V4("order-created-consumer", "order-created-producer", config);
        _messagePact = pact.WithMessageInteractions();
    }

    [Fact]
    public void Handle_AnOrderCreatedEvent_AcceptsIt()
    {
        _messagePact
            .ExpectsToReceive("un evento de orden creada")
            .Given("una orden fue creada")
            .WithJsonContent(new
            {
                // Los tres campos que EventProduce.BuildOrderCreatedEvent() realmente produce —
                // ver QueueProducerApp/EventProduce.cs. orderId y total van con matcher de tipo:
                // a este consumidor no le importa el valor exacto, le importa la forma.
                orderId = Match.Type(123),
                total = Match.Decimal(250.50m),
                createdAt = Match.Type("2025-12-13T00:00:00Z"),
            })
            .Verify<JsonElement>(messageBody =>
            {
                new OrderCreatedEventHandler().Handle(messageBody.GetRawText());
            });
    }
}
