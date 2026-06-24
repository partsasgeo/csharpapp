using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.ExceptionHandling
{
    public class ExternalApiExceptionHandler : IExceptionHandler
    {

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not HttpRequestException httpRequestException)
            {
                return false;
            }

            httpContext.Response.StatusCode = ((int?)httpRequestException.StatusCode!.Value ?? StatusCodes.Status500InternalServerError);
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails {
                Status = (int)httpRequestException.StatusCode.Value,
                Title = "The external API returned an error.",
                Detail = httpRequestException.Message
            });
            return true;
        }
    }
}
