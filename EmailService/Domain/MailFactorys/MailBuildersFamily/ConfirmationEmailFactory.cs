using System.Net.Mail;
using EmailService.Domain.MailFactorys.MailBuildersFamily.Application;
using Journalist;

namespace EmailService.Domain.MailFactorys.MailBuildersFamily
{
    class ConfirmationEmailFactory : IMailBuilder
    {
        public MailMessage BuildMailMessage(MailAddress to, MailAddress from, string link, string targetNickname)
        {
            Require.NotNull(to, nameof(to));
            Require.NotNull(from, nameof(from));

            var message = new MailMessage(from, to);
            message.Body = string.Format(MessageTexts.ConfirmationEmailMessage, targetNickname, link);
            message.Subject = MessageTexts.ConfirmationEmailSubject;
            return message;
        }
    }
}
