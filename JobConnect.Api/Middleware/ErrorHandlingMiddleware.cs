using JobConnect.Exceptions;
using JobConnect.Api.Services;
using System.Net;
using System.Text.Json;
using SendGrid.Helpers.Errors.Model;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IServiceProvider serviceProvider)
    {
        _next = next;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var errorId = Guid.NewGuid().ToString();
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Internal Server Error";

            switch (ex)
            {
                case BusinessException:
                    statusCode = 400;
                    message = ex.Message;
                    break;
                case KeyNotFoundException:
                    statusCode = 404;
                    message = ex.Message;
                    break;
                case UnauthorizedAccessException:
                    statusCode = 401;
                    message = ex.Message;
                    break;
                case ForbiddenException:
                    statusCode = 403;
                    message = ex.Message;
                    break;
            }

            _logger.LogError(ex, "Unhandled Exception [{ErrorId}]: {Message}", errorId, ex.Message);

            if (statusCode == 500)
            {
                using var scope = _serviceProvider.CreateScope();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                await emailSender.SendEmailAsync(
                    $" Lỗi hệ thống  - ID: {errorId}",
                    $"Exception: {ex.Message}\n"
                );
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode,
                message,
                errorId
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
