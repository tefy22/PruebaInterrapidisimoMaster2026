using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace UniversityMaster.Middleware
{
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
                _logger.LogError(ex, $"Ocurrio una excepcion: {ex.Message}");
                var excepcionDetails = GetExceptionDetails(ex);
                var problemDetails = new ProblemDetails
                {
                    Status = excepcionDetails.Status,
                    Type = excepcionDetails.Type,
                    Title = excepcionDetails.Title,
                    Detail = excepcionDetails.Detail
                };

                if (excepcionDetails.Errors is not null)
                {
                    problemDetails.Extensions["errors"] = excepcionDetails.Errors;
                }

                context.Response.StatusCode = excepcionDetails.Status;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
        private static ExceptionDetails GetExceptionDetails(Exception ex)
        {
            return ex switch
            {
                ValidationException ve => new ExceptionDetails(
                    StatusCodes.Status400BadRequest,
                    "ValidationFailure",
                    "Validacion de error",
                    "Han ocurrido uno o mas errores de validacion",
                    ve.Errors                        
                ),
                _=> new ExceptionDetails(
                        StatusCodes.Status500InternalServerError,
                        "ServerError",
                        "Error de Servidor",
                        "Un inesperado error ha ocurrido en la app", 
                        null)
            };
        }

    }

    internal record ExceptionDetails(int Status, string Type, string Title, string Detail, IEnumerable<object>? Errors);
}
