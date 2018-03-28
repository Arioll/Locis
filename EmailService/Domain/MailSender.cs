using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Common.Entities;
using DataAccess.Application;
using EmailService.Application;
using EmailService.Domain.MailFactorys;

namespace EmailService.Domain
{
    public class MailSender
    {
        public MailSender(ISmtpClientFactory smtpClientFactory, 
            IEmailMessageRepository emailMessageRepository,
            CancellationTokenSource cancellationTokenSource, 
            MailBuildingDirector mailBuildingDirector)
        {
            _smtpClientFactory = smtpClientFactory;
            _emailMessageRepository = emailMessageRepository;
            _cancellationTokenSource = cancellationTokenSource;
            _mailBuildingDirector = mailBuildingDirector;
            _currentTimeout = _smtpClientFactory.GetBasicEmailTimeout();
        }

        private readonly ISmtpClientFactory _smtpClientFactory;
        private readonly IEmailMessageRepository _emailMessageRepository;
        private readonly MailBuildingDirector _mailBuildingDirector;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private TimeSpan _currentTimeout;

        public void StartSending()
        {
            Task.Factory.StartNew(() =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var process = SendProcessEmail();
                    _currentTimeout = process
                        ? _smtpClientFactory.GetBasicEmailTimeout()
                        : _currentTimeout >= _smtpClientFactory.GetMaxEmailTimeout()
                            ? _smtpClientFactory.GetMaxEmailTimeout()
                            : _currentTimeout + _smtpClientFactory.GetTimeoutIncrement();
                    Task.Delay(_currentTimeout).Wait(_cancellationTokenSource.Token);
                }
            });
        }

        private bool SendProcessEmail()
        {
            var email = _emailMessageRepository.PullEmailMessage();
            if (email == null) return false;
            try
            {
                SendMail(email);
                _emailMessageRepository.DeleteEmailMessage(email);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                _emailMessageRepository.DeleteEmailMessage(email);
                return false;
            }
            return true;
        }

        private void SendMail(EmailMessage message)
        {
            var smtpClient = _smtpClientFactory.GetSmtpClient();
            foreach (var address in message.TargetEmails)
            {
                var mail = _mailBuildingDirector.BuildMessage(
                    new MailAddress(address),
                    _smtpClientFactory.GetSenderMailAddress(),
                    message.OperationType,
                    message.Link,
                    message.TargetNickname);
                smtpClient.Send(mail);
                mail.Dispose();
            }
            smtpClient.Dispose();
        }
    }
}
