using Inventory.IntTest;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace MessagePactProvider;

// Replay & Verify del lado del mensaje: reproduce el pacto que grabó MessagePactConsumer/ contra
// el productor REAL — EventProduce.BuildOrderCreatedEvent(), en QueueProducerApp — sin conectarse
// a RabbitMQ en ningún momento. El broker no entra acá a propósito: lo que este pacto contrata es
// el shape del mensaje, no el transporte que lo lleva. Eso es justo lo que dont.md acota como
// alcance de esta sesión sobre el seam asincrónico.
public class OrderCreatedEventProviderPactTests
{
    private readonly ITestOutputHelper _output;

    public OrderCreatedEventProviderPactTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void EventProduce_BuildsAMessage_ThatMatchesTheConsumerPact()
    {
        var config = new PactVerifierConfig
        {
            Outputters = new IOutput[] { new XunitOutput(_output) }
        };

        var pactPath = Path.Combine(
            "..", "..", "..", "..", "MessagePactConsumer", "pacts",
            "order-created-consumer-order-created-producer.json");

        using var verifier = new PactVerifier("order-created-producer", config);
        verifier
            .WithMessages(scenarios =>
            {
                scenarios.Add("un evento de orden creada", () =>
                {
                    var eventProduce = new EventProduce();
                    var orderCreated = eventProduce.BuildOrderCreatedEvent();
                    return eventProduce.SerializeOrderCreatedEvent(orderCreated);
                });
            })
            .WithFileSource(new FileInfo(pactPath))
            .Verify();
    }
}
