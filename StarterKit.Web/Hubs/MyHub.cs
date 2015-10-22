namespace StarterKit.Web.Hubs
{
    using MassTransit;
    using Microsoft.AspNet.SignalR;
    using StarterKit.Contracts;

    public class MyHub : Hub
    {
        private readonly IBus _bus;

        public MyHub(IBus bus)
        {
            _bus = bus;
        }
        public void Send(string message)
        {
            _bus.Publish<MyMessage>(new { Message = message });
        }
    }
}