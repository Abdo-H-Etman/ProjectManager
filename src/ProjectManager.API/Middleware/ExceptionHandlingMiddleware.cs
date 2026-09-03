using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using Domain.Exceptions;

namespace ProjectManager.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var statusCode = exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            NotFoundException => HttpStatusCode.NotFound,
            KeyNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        response.StatusCode = (int)statusCode;

        object responseBody;

        if (exception is ValidationException validationEx)
        {
            _logger.LogWarning("Validation failure occurred: {@Errors}", validationEx.Errors);
            responseBody = new
            {
                status = (int)HttpStatusCode.BadRequest,
                title = "Validation Failed",
                errors = validationEx.Errors
            };
        }
        else if (exception is NotFoundException notFoundEx)
        {
            _logger.LogWarning("Resource not found: {Message}", notFoundEx.Message);
            responseBody = new
            {
                status = (int)HttpStatusCode.NotFound,
                title = "Not Found",
                detail = notFoundEx.Message
            };
        }
        else
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
            responseBody = new
            {
                status = (int)HttpStatusCode.InternalServerError,
                title = "Internal Server Error",
                detail = exception.Message
            };
        }

        var json = JsonSerializer.Serialize(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(json);
    }
}
