using System.Net;
using System.Text.Json;
using ChakChakShop.API.DTO.Responses;
using ChakChakShop.API.Exceptions;

namespace ChakChakShop.API.Middleware;

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

        var errorResponse = new ErrorResponse
        {
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case NotFoundException notFoundEx:
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = notFoundEx.Message;
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case BadRequestException badRequestEx:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = badRequestEx.Message;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case UnauthorizedAccessException:
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            default:
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An error occurred while processing your request";
                errorResponse.Details = exception.Message;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogError(exception, "Unhandled exception occurred");
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}


