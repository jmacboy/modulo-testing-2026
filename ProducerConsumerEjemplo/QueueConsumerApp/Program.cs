// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using QueueConsumerApp;

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest"
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: "orders.created.queue",
    durable: true,
    exclusive: false,
    autoDelete: false
);

var consumer = new AsyncEventingBasicConsumer(channel);
var handler = new OrderCreatedEventHandler();

consumer.ReceivedAsync += async (sender, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Evento recibido: {message}");

    handler.Handle(message);

    // Confirmar procesamiento solo después de procesar el mensaje correctamente.
    await channel.BasicAckAsync(args.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(
    queue: "orders.created.queue",
    autoAck: false,
    consumer: consumer
);

Console.WriteLine("Escuchando eventos...");
Console.ReadLine();
