using System.Net;
using System.Text.Json;
using CoreBankingSystem.Shared.Exceptions;

namespace CoreBankingSystem.API.Middlewares;

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
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, message) = exception switch
        {
            AccountNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            AccountInactiveException => (HttpStatusCode.UnprocessableEntity, exception.Message),
            DuplicateDocumentException => (HttpStatusCode.Conflict, exception.Message),
            InsufficientBalanceException => (HttpStatusCode.UnprocessableEntity, exception.Message),
            InvalidAmountException => (HttpStatusCode.BadRequest, exception.Message),
            SameAccountTransferException => (HttpStatusCode.BadRequest, exception.Message),
            InvalidCpfException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(response);
    }
}