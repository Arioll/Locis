using Common.Entities;

namespace DataAccess.Application
{
    public interface IEmailMessageRepository
    {
        EmailMessage GetMessageById(uint messageId);
        void SaveEmailMessage(EmailMessage messageToSave);
        void DeleteEmailMessage(EmailMessage messageTodelete);
        EmailMessage PullEmailMessage();
    }
}
