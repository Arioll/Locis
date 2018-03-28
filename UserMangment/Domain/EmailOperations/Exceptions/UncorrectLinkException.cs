using System;

namespace UserMangment.Domain.EmailOperations.Exceptions
{
    public class UncorrectLinkException : Exception
    {
        public UncorrectLinkException(string message) : base(message)
        {
            
        }

        public UncorrectLinkException() { }
    }
}
