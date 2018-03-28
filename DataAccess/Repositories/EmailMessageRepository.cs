using System.Linq;
using Common.Entities;
using DataAccess.Application;
using DataAccess.NHibernate;
using Journalist;
using NHibernate;

namespace DataAccess.Repositories
{
    public class EmailMessageRepository : IEmailMessageRepository
    {
        public EmailMessageRepository(SessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        private readonly SessionProvider _sessionProvider;

        public EmailMessage GetMessageById(uint messageId)
        {
            Require.Positive(messageId, nameof(messageId));
            var session = _sessionProvider.GetCurrentSession();
            return session.Get<EmailMessage>(messageId);
        }

        public void SaveEmailMessage(EmailMessage messageToSave)
        {
            Require.NotNull(messageToSave, nameof(messageToSave));
            var session = _sessionProvider.GetCurrentSession();
            session.Save(messageToSave);
        }

        public EmailMessage PullEmailMessage()
        {
            _sessionProvider.OpenSession();
            var result = Session.QueryOver<EmailMessage>().List();
            _sessionProvider.CloseSession();
            return result.Any() ? result.First() : null;
        }

        public void DeleteEmailMessage(EmailMessage messageToDelete)
        {
            Require.NotNull(messageToDelete, nameof(messageToDelete));
            _sessionProvider.OpenSession();
            Session.Delete(messageToDelete);
            _sessionProvider.CloseSession();
        }

        private ISession Session => _sessionProvider.GetCurrentSession();
    }
}
