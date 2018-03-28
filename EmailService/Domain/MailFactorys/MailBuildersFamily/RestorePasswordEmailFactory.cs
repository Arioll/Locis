using System.Net.Mail;
using EmailService.Domain.MailFactorys.MailBuildersFamily.Application;
using Journalist;

namespace EmailService.Domain.MailFactorys.MailBuildersFamily
{
    class RestorePasswordEmailBuilder : IMailBuilder
    {
        public MailMessage BuildMailMessage(MailAddress to, MailAddress from, string link, string targetNickname)
        {
            Require.NotNull(to, nameof(to));
            Require.NotNull(from, nameof(from));

            var message = new MailMessage(from, to);
            message.Body = string.Format(MessageTexts.ChangePasswordMessage, targetNickname, link);
            message.Subject = MessageTexts.ChangePasswordEmailSubject;
            return message;
        }
    }
}
