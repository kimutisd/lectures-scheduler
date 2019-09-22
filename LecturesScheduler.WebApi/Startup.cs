using LecturesScheduler.WebApi.Middleware.DependencyContainer;
using LecturesScheduler.WebApi.Middleware.ErrorHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace LecturesScheduler.WebApi
{
    public class Startup
    {
        private readonly string ServiceName = "LecturesScheduler";

        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Info { Title = "LecturesScheduler API", Version = "v1" });
                x.DescribeAllEnumsAsStrings();
            });

            services.AddSingleton(Configuration);

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            return services.RegisterAutofacDependencies(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", ServiceName + " API V1");
            });

            app.Use(async (httpContext, next) =>
            {
                if (string.IsNullOrEmpty(httpContext.Request.Path) ||
                    httpContext.Request.Path == "/" ||
                    httpContext.Request.Path == "/api")
                {
                    httpContext.Response.Redirect(httpContext.Request.PathBase + "/swagger");
                    return;
                }

                await next();
            });

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
