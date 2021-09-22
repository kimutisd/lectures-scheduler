﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using LecturesScheduler.Services.Reposiroty;
using LecturesScheduler.Services.Services;
using Serilog;

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
            containerBuilder.RegisterType<ScheduleRepository>().As<IScheduleRepository>().InstancePerDependency();

            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            return serviceProvider;
        }
    }
}
