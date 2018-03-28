using Common.Entities;

namespace UserMangment.Domain.EmailOperations.Application
{
    public interface IEmailConfirmation
    {
        void SendEmailToConfirmation(User user);
        void SendEmailToRestorePassword(uint userId);
        void SendMessageToChangeEmail(uint userId, string newEmail);
        void ConfirmUserEmailToRegistration(string guidFromLink);
        void RecoverUserPassword(string guidFromLink, string newPassword);
        void ConfirmUserEmailToChange(string guidFromLink);
        bool CheckGuidCurrently(string guid);
    }
}
