using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace WebApiAutores.Middlewares
{
    public static class LoggerResponseHttpMiddlewareExtension
    {
        public static IApplicationBuilder UseLoggerResponseHttp(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggerResponseHttpMiddleware>();
        }
    }

    public class LoggerResponseHttpMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public LoggerResponseHttpMiddleware(RequestDelegate next, ILogger<LoggerResponseHttpMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;   
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRepuesta = context.Response.Body;
                context.Response.Body = ms;
                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRepuesta);
                context.Response.Body = cuerpoOriginalRepuesta;

                logger.LogInformation(respuesta);
            }

        }
    }
}
