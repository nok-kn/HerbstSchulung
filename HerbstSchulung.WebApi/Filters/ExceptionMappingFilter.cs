using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HerbstSchulung.WebApi.Filters;

/// <summary>
/// Mappt bestimmte Ausnahmen auf HTTP-Statuscodes und erstellt ProblemDetails-Antworten.
/// </summary>
/// <param name="problemDetailsFactory"></param>
/// <param name="logger"></param>
public sealed class ExceptionMappingFilter(ProblemDetailsFactory problemDetailsFactory, ILogger<ExceptionMappingFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        int? status = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => null
        };

        if (status is null)
        {
            // Not handled here; let other handlers/middleware process it
            return;
        }

        logger.LogDebug(exception, "Mapped exception {ExceptionType} to HTTP {StatusCode}", exception.GetType().Name, status.Value);

        var problem = problemDetailsFactory.CreateProblemDetails(
            context.HttpContext,
            statusCode: status,
            title: status == StatusCodes.Status400BadRequest ? "Validation failed." : "Resource not found.",
            detail: exception.Message
        );

        // Include trace id
        problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        context.Result = new ObjectResult(problem)
        {
            StatusCode = status
        };
        context.ExceptionHandled = true;
    }
}
