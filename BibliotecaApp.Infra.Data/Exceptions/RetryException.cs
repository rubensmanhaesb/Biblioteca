using System;

namespace BibliotecaApp.Infra.Data.Exceptions
{
    public class RetryException : Exception
    {
        public RetryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public RetryException(Exception innerException)
         : base ($"Ocorreu um erro durante o processo de nova tentativa de reconexão {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", innerException)
        { }
}
}