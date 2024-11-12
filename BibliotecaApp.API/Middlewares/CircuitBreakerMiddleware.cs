using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using BibliotecaApp.Infra.Data.Exceptions;

namespace BibliotecaApp.API.Middlewares
{
    public class CircuitBreakerMiddleware
    {
        private readonly RequestDelegate _next;

        public CircuitBreakerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (CircuitBreakerOpenException ex)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync
                    ($"Serviço temporariamente indisponível. Por favor, tente mais tarde. Error: {ex.Message}");
            }
        }
    }
}
