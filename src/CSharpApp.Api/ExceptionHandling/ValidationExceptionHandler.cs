using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.ExceptionHandling
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        public ValidationExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            ProblemDetailsService = problemDetailsService;
        }

        public IProblemDetailsService ProblemDetailsService { get; }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not ValidationException validationException)
            {
                return false;
            }

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return await ProblemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid request input."
                }
            });

        }
    }
}
