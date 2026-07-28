using eCommerce.BLL.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace eCommerce.API.Middlewares;
// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project

/// <summary>
/// Middleware that catches exceptions thrown anywhere in the request pipeline
/// and converts them into standardized JSON error responses, distinguishing
/// between argument/validation errors and unexpected server errors.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ExceptionHandlingMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    /// <param name="logger">Logger used to record caught exceptions.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the next middleware in the pipeline, catching <see cref="ArgumentNullException"/>
    /// and <see cref="CustomValidationException"/> as 400 Bad Request responses, and any other
    /// unhandled exception as a 500 Internal Server Error response.
    /// </summary>
    /// <param name="httpContext">The current HTTP context for the request.</param>
    public async Task InvokeAsync(HttpContext httpContext)
    {

        try
        {
            await _next(httpContext);
        }
        catch (ArgumentNullException ex)
        {
            // catch validations
            _logger.LogError(ex, "Argument null error occurred.");

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(ex.Message);

        }
        catch (CustomValidationException ex)
        {
            // catch validations
            _logger.LogError(ex, "Validation or Argument error occurred.");

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/json";

            var problemDetails = new HttpValidationProblemDetails(ex.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            };


            await httpContext.Response.WriteAsJsonAsync(problemDetails);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred in the server.");

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(new { Error = "An unexpected error occurred. Please try again later." });
        }

    }
}

// Extension method used to add the middleware to the HTTP request pipeline.

/// <summary>
/// Provides an extension method for registering <see cref="ExceptionHandlingMiddleware"/>
/// into the application's request pipeline.
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Adds <see cref="ExceptionHandlingMiddleware"/> to the application's request pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance, for chaining.</returns>
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}