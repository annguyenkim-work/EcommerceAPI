using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Ecommerce.Api.Middlewares;

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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path,
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Server Error",
            Detail = "An unexpected error occurred."
        };

        // Bắt lỗi Validation từ FluentValidation (Ném ra từ ValidationBehavior)
        if (exception is ValidationException validationException)
        {
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = "Validation Error";
            problemDetails.Detail = "One or more validation failures have occurred.";

            // Gom danh sách lỗi theo từng Field
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            problemDetails.Extensions.Add("errors", errors);
        }

        context.Response.StatusCode = problemDetails.Status ?? 500;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsJsonAsync(problemDetails, options);
    }
}