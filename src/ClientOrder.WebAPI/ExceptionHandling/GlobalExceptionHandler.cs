using ClientOrder.Domain.Exceptions;
using ClientOrder.Services.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClientOrder.WebAPI.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ServiceForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden"),
            ServiceNotFoundException => (StatusCodes.Status404NotFound, "Not found"),
            ServiceException => (StatusCodes.Status400BadRequest, "Bad request"),
            DomainException => (StatusCodes.Status400BadRequest, "Bad request"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        httpContext.Response.StatusCode = statusCode;

        var detail = statusCode == StatusCodes.Status500InternalServerError
            ? "An unexpected error occurred."
            : exception.Message;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            },
            cancellationToken);

        return true;
    }
}
