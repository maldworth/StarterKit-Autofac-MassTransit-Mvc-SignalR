namespace StarterKit.Service.Consumer
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Contracts;

    public class MyConsumer : IConsumer<MyMessage>
    {
        public async Task Consume(ConsumeContext<MyMessage> context)
        {
            Console.Out.WriteLine("Received Message: " + context.Message.Message);
        }
    }
}
