namespace EmailService.Models
{
    public class MailCredentials
    {
        public MailCredentials(string email, string password, string smtpServer, int smtpPort)
        {
            Email = email;
            Password = password;
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
        }

        internal string Email { get; }
        internal string Password { get; }
        internal string SmtpServer { get; }
        internal int SmtpPort { get; }
    }
}
