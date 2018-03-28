using System;
using System.Collections.Concurrent;
using System.Linq;
using Common.Entities;
using Common.Other;
using EmailService.Application;
using Journalist;
using UserMangment.Application;
using UserMangment.Domain.EmailOperations.Application;
using UserMangment.Domain.EmailOperations.Models;

namespace UserMangment.Domain.EmailOperations.Domain
{
    public class EmailConfirmation : IEmailConfirmation
    {
        public EmailConfirmation(IMailSendManager mailSendManager,
            TimeSpan linkLifeTime, 
            IUserManager userManager)
        {
            _mailSendManager = mailSendManager;
            _userManager = userManager;
            _linkLifeTime = linkLifeTime;
            _confirmationEmailGuids = new ConcurrentDictionary<KeyInfo, User>();
            _restorePasswordGuids = new ConcurrentDictionary<KeyInfo, uint>();
            _changeEmailGuids = new ConcurrentDictionary<KeyInfo, ChangeEmailInfo>();
        }

        private readonly TimeSpan _linkLifeTime;
        private readonly IUserManager _userManager;
        private readonly IMailSendManager _mailSendManager;
        private readonly ConcurrentDictionary<KeyInfo, User> _confirmationEmailGuids;
        private readonly ConcurrentDictionary<KeyInfo, uint> _restorePasswordGuids;
        private readonly ConcurrentDictionary<KeyInfo, ChangeEmailInfo> _changeEmailGuids;

        public void SendEmailToConfirmation(User user)
        {
            Require.NotNull(user, nameof(user));

            var keyInfo = GenerateKeyInfo();
            _confirmationEmailGuids.TryAdd(keyInfo, user);
            _mailSendManager.SendMessage(
                user.Email.Address,
                EmailConfirmationSettings.Default.RouteToConfirmEmail + keyInfo.KeyValue,
                user.Nickname,
                EmailTypes.EmailConfirmation);
        }

        public void SendEmailToRestorePassword(uint userId)
        {
            Require.Positive(userId, nameof(userId));

            var user = _userManager.GetUserById(userId);
            var keyInfo = GenerateKeyInfo();
            _restorePasswordGuids.TryAdd(keyInfo, userId);
            _mailSendManager.SendMessage(
                user.Email.Address, 
                EmailConfirmationSettings.Default.RouteToRestorePassword + keyInfo.KeyValue,
                user.Nickname,
                EmailTypes.PasswordRestore);
        }

        public void SendMessageToChangeEmail(uint userId, string newEmail)
        {
            Require.NotEmpty(newEmail, nameof(newEmail));
            Require.Positive(userId, nameof(userId));

            var user = _userManager.GetUserById(userId);
            var keyInfo = GenerateKeyInfo();
            _changeEmailGuids.TryAdd(keyInfo, new ChangeEmailInfo(newEmail, userId));
            _mailSendManager.SendMessage(
                newEmail,
                EmailConfirmationSettings.Default.RouteToChangeEmail + keyInfo.KeyValue,
                user.Nickname,
                EmailTypes.ChangeEmail);
        }

        public void ConfirmUserEmailToRegistration(string guidFromLink)
        {
            Require.NotEmpty(guidFromLink, nameof(guidFromLink));

            var keyInfo = GetKeyInfo(guidFromLink);
            _confirmationEmailGuids.TryRemove(keyInfo, out User user);
            _userManager.CreateUser(user);
        }

        public void RecoverUserPassword(string guidFromLink, string newPassword)
        {
            Require.NotEmpty(guidFromLink, nameof(guidFromLink));
            Require.NotEmpty(newPassword, nameof(newPassword));

            var keyInfo = GetKeyInfo(guidFromLink);
            _restorePasswordGuids.TryRemove(keyInfo, out uint userId);
            _userManager.ChangeUserPassword(newPassword, userId);
        }

        public void ConfirmUserEmailToChange(string guidFromLink)
        {
            Require.NotEmpty(guidFromLink, nameof(guidFromLink));

            var keyInfo = GetKeyInfo(guidFromLink);
            _changeEmailGuids.TryRemove(keyInfo, out ChangeEmailInfo user);
            _userManager.ChangeUserEmail(user.GetEmail, user.GetUserId);
        }

        public bool CheckGuidCurrently(string guid)
        {
            Require.NotEmpty(guid, nameof(guid));

            var keyInfo = _confirmationEmailGuids.Keys.SingleOrDefault(x => x.KeyValue == guid);
            if (keyInfo == null)
            {
                keyInfo = _restorePasswordGuids.Keys.SingleOrDefault(x => x.KeyValue == guid);
                if (keyInfo == null)
                {
                    keyInfo = _changeEmailGuids.Keys.SingleOrDefault(x => x.KeyValue == guid);
                    if (keyInfo == null)
                        return false;
                }
            }
            return DateTime.Now - keyInfo.TimeOfCreate <= _linkLifeTime;
        }

        private KeyInfo GenerateKeyInfo()
        {
            var key = Guid.NewGuid().ToString();
            var keyInfo = new KeyInfo(key, DateTime.Now);
            return keyInfo;
        }

        private KeyInfo GetKeyInfo(string guid)
        {
            var keyInfo = _confirmationEmailGuids.Keys.SingleOrDefault(x => x.KeyValue == guid);
            if (keyInfo == null)
            {
                keyInfo = _restorePasswordGuids.Keys.SingleOrDefault(x => x.KeyValue == guid);
                if (keyInfo == null)
                {
                    keyInfo = _changeEmailGuids.Keys.SingleOrDefault(x => x.KeyValue == guid);
                }
            }
            return keyInfo;
        }

        private struct ChangeEmailInfo
        {
            internal ChangeEmailInfo(string newEmail, uint userId)
            {
                NewEmail = newEmail;
                UserId = userId;
            }

            internal string GetEmail => NewEmail;
            internal uint GetUserId => UserId;
       
            private string NewEmail { get; }
            private uint UserId { get; }
        }
    }
}
