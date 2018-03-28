using System.Net.Mail;
using Common.Other;
using EmailService.Domain.MailFactorys.MailBuildersFamily;
using EmailService.Domain.MailFactorys.MailBuildersFamily.Application;

namespace EmailService.Domain.MailFactorys
{
    public class MailBuildingDirector
    {
        private IMailBuilder _builder;

        public MailMessage BuildMessage(MailAddress to, MailAddress from, EmailTypes type, string link, string targetNickname)
        {
            switch (type)
            {
                case EmailTypes.ChangeEmail:
                    _builder = new ChangeAddressEmailBuilder();
                    break;
                case EmailTypes.EmailConfirmation:
                    _builder = new ConfirmationEmailFactory();
                    break;
                case EmailTypes.PasswordRestore:
                    _builder = new RestorePasswordEmailBuilder();
                    break;
            }
            return _builder.BuildMailMessage(to, from, link, targetNickname);
        }
    }
}
