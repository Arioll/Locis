namespace WebApplication1.Models
{
    public class CreateUserRequest
    {
        public CreateUserRequest(string nickname, string email, string password)
        {
            Nickname = nickname;
            Email = email;
            Password = password;
        }

        public string Nickname { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
    }
}