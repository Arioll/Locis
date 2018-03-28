using System.Net.Mail;
using EmailService.Domain.MailFactorys.MailBuildersFamily.Application;
using Journalist;

namespace EmailService.Domain.MailFactorys.MailBuildersFamily
{
    class ChangeAddressEmailBuilder : IMailBuilder
    {
        public MailMessage BuildMailMessage(MailAddress to, MailAddress @from, string link, string targetNickname)
        {
            Require.NotNull(to, nameof(to));
            Require.NotNull(from, nameof(from));

            var message = new MailMessage(from, to);
            message.Body = string.Format(MessageTexts.ChangeEmailMessage, targetNickname, link);
            message.Subject = MessageTexts.ChangeEmailSubgect;
            return message;
        }
    }
}
