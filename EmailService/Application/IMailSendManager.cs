using Common.Other;

namespace EmailService.Application
{
    public interface IMailSendManager
    {
        void SendMessage(string userEmail, string link, string nickname, EmailTypes type);
    }
}
