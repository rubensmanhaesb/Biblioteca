using BibliotecaApp.Domain.Exceptions;
using System.Text.Json;

namespace BibliotecaApp.API.Middlewares
{
    public class RecordAlreadyExistsException
    {
        private readonly RequestDelegate _next;

        public RecordAlreadyExistsException(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BaseRecordAlreadyExistsException ex)
            {
                await HandleNotFoundExceptionAsync(context, ex);
            }
        }

        private static Task HandleNotFoundExceptionAsync(HttpContext context, BaseRecordAlreadyExistsException exception)
        {

            context.Response.StatusCode = StatusCodes.Status409Conflict;
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
