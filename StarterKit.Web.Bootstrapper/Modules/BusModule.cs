namespace StarterKit.Web.Bootstrapper.Modules
{
    using System;
    using Autofac;
    using MassTransit;
    using System.Configuration;

    public class BusModule : Module
    {
        private readonly System.Reflection.Assembly[] _assembliesToScan;

        public BusModule(params System.Reflection.Assembly[] assembliesToScan)
        {
            _assembliesToScan = assembliesToScan;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterConsumers(_assembliesToScan);

            // Creates our bus from the factory and registers it as a singleton against two interfaces
            builder.Register(c => Bus.Factory.CreateUsingRabbitMq(sbc => sbc.Host(new Uri(ConfigurationManager.AppSettings["RabbitMQHost"]), h =>
            {
                h.Username(ConfigurationManager.AppSettings["RabbitMQUsername"]);
                h.Password(ConfigurationManager.AppSettings["RabbitMQPassword"]);
            })))
                .As<IBusControl>()
                .As<IBus>()
                .SingleInstance();
        }
    }
}
