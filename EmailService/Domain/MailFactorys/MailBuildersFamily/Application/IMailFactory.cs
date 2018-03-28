using System.Net.Mail;

namespace EmailService.Domain.MailFactorys.MailBuildersFamily.Application
{
    interface IMailBuilder
    {
        MailMessage BuildMailMessage(MailAddress to, MailAddress from, string link, string targetNickname);
    }
}
