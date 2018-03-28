using System;
using System.Collections.Generic;
using System.Net.Mail;
using Journalist;

namespace Common.Entities
{
    public class User
    {
        public User(string nickname, MailAddress email, Password password)
        {
            Nickname = nickname;
            Email = email;
            Password = password;
            IsDeleted = false;
        }

        protected User() { }

        public virtual ISet<Invitation> Invaitations { get; set; }
        public virtual ISet<Room> Rooms { get; set; }
        public virtual ISet<User> Friends { get; set; }
        public virtual ISet<AddToFriendRequest> AddToFriendRequests { get; set; }
        public virtual uint PlaylistId { get; set; }
        public virtual uint UserId { get; protected set; }
        public virtual string Nickname { get; protected set; }
        public virtual bool IsDeleted { get; set; }
        public virtual MailAddress Email { get; protected set; }
        public virtual Password Password { get; protected set; }
        public virtual uint CurrentRoomId { get; set; }
        public virtual Uri AvatarPicture { get; protected set; }

        public virtual void SetPassword(string newPassword)
        {
            Require.NotEmpty(newPassword, nameof(newPassword));

            var password = new Password(newPassword);
            Password = password;
        }

        public virtual void SetEmail(string newEmail)
        {
            Require.NotEmpty(newEmail, nameof(newEmail));

            var email = new MailAddress(newEmail);
            Email = email;
        }

        public virtual void SetNickname(string newNickname)
        {
            Require.NotEmpty(newNickname, nameof(newNickname));

            Nickname = newNickname;
        }

        public virtual void SetAvatarUri(Uri newAvatarUri)
        {
            Require.NotNull(newAvatarUri, nameof(newAvatarUri));

            AvatarPicture = newAvatarUri;
        }

        public override bool Equals(object obj)
        {
            return (obj as User)?.UserId == UserId;
        }
    }
}