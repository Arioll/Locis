using System;

namespace WebApplication1.Filters.UnhandledExceptions
{
    public class UnhandledException : Exception
    {
        public UnhandledException()
        {

        }

        public UnhandledException(string message) : base(message)
        {

        }
    }
}