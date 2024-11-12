using System;

namespace BibliotecaApp.Infra.Data.Exceptions
{
    public class CircuitBreakerOpenException : Exception
    {

        public CircuitBreakerOpenException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public CircuitBreakerOpenException(Exception innerException) : 
            base($"The circuit breaker process is open {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}"
                , innerException)
        { }
    }
}