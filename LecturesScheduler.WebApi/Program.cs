using LecturesScheduler.Persistence;
using LecturesScheduler.Persistence.Extensions;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace LecturesScheduler.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            MigrateDatabase(host);
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static void MigrateDatabase(IWebHost host)
        {
            var scope = host.Services.CreateScope();
            using (scope)
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetSection("DatabaseConnectionString").Value;
                var baseOptions = scope.ServiceProvider.GetRequiredService<DbContextOptions<DatabaseContext>>();
                var options = new DbContextOptionsBuilder<DatabaseContext>(baseOptions).UseSqlServer(connectionString);

                using (var db = new DatabaseContext(options.Options))
                {
                    db.MigrateDatabase();
                }

            }
        }
    }
}













/*using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "LecturesScheduler.WebApi", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LecturesScheduler.WebApi v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
*/