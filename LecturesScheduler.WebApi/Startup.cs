using LecturesScheduler.Persistence;
using LecturesScheduler.WebApi.Middleware.DependencyContainer;
using LecturesScheduler.WebApi.Middleware.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Serilog;

namespace LecturesScheduler.WebApi
{
    public class Startup
    {
        private readonly string ServiceName = "LecturesScheduler";

        public IConfiguration Configuration { get; }

        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        [Obsolete]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            ConfigureDatabase(services);

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "LecturesScheduler API", Version = "v1" });
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddSingleton<IConfiguration>(Configuration);

            _ = services.AddMvc();

            return services.RegisterAutofacDependencies(Configuration);
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            var connectionString = Configuration.GetSection("DatabaseConnectionString").Value;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "Connection string not found");
            }

            services.AddDbContext<IDatabaseContext, DatabaseContext>(options =>
            {
                options.UseSqlServer(connectionString);
            }, ServiceLifetime.Transient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
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
            app.UseRouting();

            //app.UseMvc();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Schedule}/{action=Index}");
            });
        }
    }
}
