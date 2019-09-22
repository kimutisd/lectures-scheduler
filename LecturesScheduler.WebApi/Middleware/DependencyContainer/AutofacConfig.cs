using Autofac;
using Autofac.Extensions.DependencyInjection;
using LecturesScheduler.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace LecturesScheduler.WebApi.Middleware.DependencyContainer
{
    public static class AutofacConfig
    {
        public static IServiceProvider RegisterAutofacDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var containerBuilder = new ContainerBuilder();

            services.AddSingleton(configuration);

            containerBuilder.Populate(services);

            containerBuilder.Register(x => Log.Logger).SingleInstance();

            containerBuilder.RegisterType<ScheduleService>().As<IScheduleService>().InstancePerDependency();

            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            return serviceProvider;
        }
    }
}
