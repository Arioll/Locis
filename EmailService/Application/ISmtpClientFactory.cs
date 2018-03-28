using System;
using System.Net.Mail;

namespace EmailService.Application
{
    public interface ISmtpClientFactory
    {
        SmtpClient GetSmtpClient();
        MailAddress GetSenderMailAddress();
        TimeSpan GetBasicEmailTimeout();
        TimeSpan GetMaxEmailTimeout();
        TimeSpan GetTimeoutIncrement();
    }
}
