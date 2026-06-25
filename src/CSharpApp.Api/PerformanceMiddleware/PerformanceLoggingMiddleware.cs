using System.Diagnostics;

namespace CSharpApp.Api.PerformanceMiddleware
{
    public sealed class PerformanceLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceLoggingMiddleware> _logger;

        public PerformanceLoggingMiddleware(RequestDelegate next, ILogger<PerformanceLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(httpContext);
            stopwatch.Stop();
            _logger.LogInformation("{Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                httpContext.Request.Method, httpContext.Request.Path, httpContext.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
