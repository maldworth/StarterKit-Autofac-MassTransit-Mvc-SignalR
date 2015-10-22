namespace StarterKit.Web.Bootstrapper
{
    using System.Threading;
    using System.Web.Mvc;
    using Autofac;
    using AutofacMvc = Autofac.Integration.Mvc;
    using AutofacSignalR = Autofac.Integration.SignalR;
    using MassTransit;
    using Microsoft.Owin;
    using Owin;
    using System;
    using log4net.Config;
    using MassTransit.Log4NetIntegration.Logging;
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Infrastructure;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Loads the config from our App.config
            XmlConfigurator.Configure();

            // MassTransit to use Log4Net
            Log4NetLogger.Use();

            var container = IocConfig.RegisterDependencies();

            // Get your HubConfiguration. In OWIN, we create one rather than using GlobalHost
            var hubConfig = new HubConfiguration();

            // Sets the dependency resolver to be autofac.
            hubConfig.Resolver = new AutofacSignalR.AutofacDependencyResolver(container);

            // Sets the Mvc resolver
            DependencyResolver.SetResolver(new AutofacMvc.AutofacDependencyResolver(container));

            // Sets SignalR and Mvc Owin resolver as well
            app.UseAutofacMiddleware(container);
            app.MapSignalR("/signalr", hubConfig);
            app.UseAutofacMvc();

            // There's not a lot of documentation or discussion for owin getting the hubcontext
            // Got this from here: https://stackoverflow.com/questions/29783898/owin-signalr-autofac
            var builder = new ContainerBuilder();
            var connManager = hubConfig.Resolver.Resolve<IConnectionManager>();
            builder.RegisterInstance(connManager)
                .As<IConnectionManager>()
                .SingleInstance();
            builder.Update(container);

            // Starts Mass Transit Service bus, and registers stopping of bus on app dispose
            var bus = container.Resolve<IBusControl>();
            var busHandle = bus.Start();

            if (app.Properties.ContainsKey("host.OnAppDisposing"))
            {
                var context = new OwinContext(app.Properties);
                var token = context.Get<CancellationToken>("host.OnAppDisposing");
                if (token != CancellationToken.None)
                {
                    token.Register(() => busHandle.Stop(TimeSpan.FromSeconds(30)));
                }
            }
        }
    }
}