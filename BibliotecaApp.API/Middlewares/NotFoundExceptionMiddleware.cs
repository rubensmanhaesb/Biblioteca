using BibliotecaApp.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace BibliotecaApp.API.Middlewares
{
    public class NotFoundExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public NotFoundExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BaseNotFoundException ex)
            {
                await HandleNotFoundExceptionAsync(context, ex);
            }
        }

        private static Task HandleNotFoundExceptionAsync(HttpContext context, BaseNotFoundException exception)
        {
            
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            
            var errorResponse = new
            {
                Details = exception.Message
            };

            
            var jsonResponse = JsonSerializer.Serialize(errorResponse);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
