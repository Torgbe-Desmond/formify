using System.Net;
using System.Text.Json;
using FastTransfers.Application.Common.Exceptions;
using FastTransfers.Domain.Exceptions;

namespace FastTransfers.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, errors) = exception switch
        {
            ValidationException ve =>
                (HttpStatusCode.UnprocessableEntity, "Validation failed", ve.Errors),

            NotFoundException =>
                (HttpStatusCode.NotFound, exception.Message,
                 (IDictionary<string, string[]>?)null),

            UnauthorizedDomainException =>
                (HttpStatusCode.Forbidden, exception.Message, null),

            ConflictException =>
                (HttpStatusCode.Conflict, exception.Message, null),

            DomainException =>
                (HttpStatusCode.BadRequest, exception.Message, null),

            _ =>
                (HttpStatusCode.InternalServerError, "An unexpected error occurred.", null)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        var response = new
        {
            status  = (int)statusCode,
            title,
            errors
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
