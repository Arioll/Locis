using System.Collections.Generic;
using System.Net.Mail;
using Common.Entities;
using Common.Other;
using DataAccess.Application;
using EmailService.Application;

namespace EmailService.Domain
{
    public class MailSendManager : IMailSendManager
    {
        public MailSendManager(IEmailMessageRepository emailMessageRepository)
        {
            _emailMessageRepository = emailMessageRepository;
        }

        private readonly IEmailMessageRepository _emailMessageRepository;

        public void SendMessage(string userEmail, string link, string nickname, EmailTypes type)
        {
            var message = new EmailMessage(new HashSet<string> {userEmail}, link, nickname, type);
            _emailMessageRepository.SaveEmailMessage(message);
        }
    }
}