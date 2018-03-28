namespace WebApplication1.Models
{
    public class AuthorizationRequest
    {
        public AuthorizationRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; private set; }
        public string Password { get; private set; }
    }
}