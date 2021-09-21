using LecturesScheduler.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.Net;
using Serilog;

namespace LecturesScheduler.WebApi.Middleware.ErrorHandling
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        public ErrorHandlingMiddleware(RequestDelegate next, Serilog.ILogger logger, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await LogErrorExceptionWithRequestBody(context, ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorMessage = "Internal server error happened. Please contact support";

            if (_environment.IsDevelopment())
            {
                errorMessage = JsonConvert.SerializeObject(exception, Formatting.Indented);
            }

            return context.Response.WriteAsync(new ErrorDetails
            {
                Message = errorMessage
            }.ToString());
        }

        private async Task LogErrorExceptionWithRequestBody(HttpContext context, Exception exception)
        {
            context.Request.EnableBuffering();
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(context.Request.Body))
            {
                var body = await reader.ReadToEndAsync();

                _logger.Error(
                    exception,
                    $"WebApi exception, Method: {{method}}, Content: {{faultMessage}}",
                    $"{context.Request.Method} {context.Request.GetDisplayUrl()}",
                    JsonConvert.SerializeObject(body));

                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
