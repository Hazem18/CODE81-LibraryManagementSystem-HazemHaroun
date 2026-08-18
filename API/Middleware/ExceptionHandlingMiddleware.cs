using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using FluentValidation;

namespace API.Middleware;

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
        context.Response.ContentType = "application/json";
        object response;

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = "One or more validation errors occurred.",
                    errors = validationEx.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage })
                };
                break;

            case NotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new { statusCode = context.Response.StatusCode, message = notFoundEx.Message };
                break;

            case UnauthorizedAccessException unauthorizedEx:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new { statusCode = context.Response.StatusCode, message = unauthorizedEx.Message };
                break;

            case BusinessRuleException businessEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new { statusCode = context.Response.StatusCode, message = businessEx.Message };
                break;


            default:
                _logger.LogError(exception, "Unhandled exception");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = "An unexpected error occurred. Please try again later."
                };
                break;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}