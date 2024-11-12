using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using BibliotecaApp.Infra.Data.Exceptions;

namespace BibliotecaApp.API.Middlewares
{
    public class RetryMiddleware
    {
        private readonly RequestDelegate _next;

        public RetryMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (RetryException ex)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync
                    ($"Erro ocorrido durante o processo de retry. Por favor, tente mais tarde. Error: {ex.Message}");
            }
        }
    }
}
