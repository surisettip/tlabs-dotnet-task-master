using System.Net;
using System.Text.Json;

namespace TestApp.Server.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ArgumentException ex)
            {
                // Thrown by tracker/repo when an item id doesn't exist — treat as 404
                logger.LogWarning(ex, "Not found: {Message}", ex.Message);
                await WriteError(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");
                await WriteError(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteError(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var payload = new
            {
                status = (int)statusCode,
                error = message,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}