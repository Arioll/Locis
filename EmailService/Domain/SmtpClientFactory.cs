using System;
using System.Net;
using System.Net.Mail;
using EmailService.Application;
using EmailService.Models;

namespace EmailService.Domain
{
    public class SmtpClientFactory : ISmtpClientFactory
    {
        public SmtpClientFactory(
            MailCredentials mailCredentials)
        {
            _mailCredentials = mailCredentials;
            _basicEmailTimeout = TimeSpan.FromSeconds(EmailTimeouts.Default.BasicEmailTimeout);
            _maxEmailTimeout = TimeSpan.FromSeconds(EmailTimeouts.Default.MaxEmailTimeout);
            _timeoutIncrement = TimeSpan.FromSeconds(EmailTimeouts.Default.TimeoutIncrement);
        }

        private readonly MailCredentials _mailCredentials;
        private readonly TimeSpan _basicEmailTimeout;
        private readonly TimeSpan _maxEmailTimeout;
        private readonly TimeSpan _timeoutIncrement;

        public SmtpClient GetSmtpClient()
        {
            var client = new SmtpClient
            {
                Host = _mailCredentials.SmtpServer,
                Port = _mailCredentials.SmtpPort,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_mailCredentials.Email, _mailCredentials.Password),
                EnableSsl = true
            };
            return client;
        }

        public MailAddress GetSenderMailAddress()
        {
            return new MailAddress(_mailCredentials.Email);
        }

        public TimeSpan GetBasicEmailTimeout()
        {
            return _basicEmailTimeout;
        }

        public TimeSpan GetMaxEmailTimeout()
        {
            return _maxEmailTimeout;
        }

        public TimeSpan GetTimeoutIncrement()
        {
            return _timeoutIncrement;
        }
    }
}
