using System;

namespace UserMangment.Domain.Authorization.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
        {

        }

        public UserNotFoundException(string message) : base(message)
        {

        }
    }
}
