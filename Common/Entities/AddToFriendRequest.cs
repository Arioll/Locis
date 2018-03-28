using System;
using Common.Entities.AbstractEntities;
using Common.Other;

namespace Common.Entities
{
    public class AddToFriendRequest : Notification
    {
        public AddToFriendRequest(uint senderId, uint targetId, string senderName)
        {
            SenderId = senderId;
            TargetId = targetId;
            SenderName = senderName;
            Type = NotificationTypes.AddToFriendRequest;
            SendDate = DateTime.Now;
        }

        protected AddToFriendRequest() { }

        public virtual uint RequestId { get; protected set; }
        public virtual uint SenderId { get; protected set; }
        public virtual uint TargetId { get; protected set; }
        public virtual string SenderName { get; protected set; }
        public virtual DateTime SendDate { get; protected set; }
    }
}
