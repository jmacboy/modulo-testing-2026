// See https://aka.ms/new-console-template for more information
using Inventory.IntTest;

Console.WriteLine("Enviando un evento a la cola");
await new EventProduce().produceEvent();
Console.WriteLine("Evento enviado");
Console.ReadLine();