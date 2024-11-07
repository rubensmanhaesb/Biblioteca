using FluentValidation;
using System.Net;
using System.Text.Json;

namespace BibliotecaApp.API.Middlewares
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
        }

        private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errors = exception.Errors
                .Select(e => new
                {
                    Field = e.PropertyName,
                    ErrorMessage = e.ErrorMessage,
                    Severity = e.Severity.ToString()
                });

            var errorResponse = new
            {
                Message = "Ocorreram erros de validação.",
                Errors = errors
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
