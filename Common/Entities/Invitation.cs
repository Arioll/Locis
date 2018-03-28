using System;
using Common.Entities.AbstractEntities;
using Common.Other;

namespace Common.Entities
{
    public class Invitation : Notification
    {
        public Invitation(uint roomId, 
            uint targetId, 
            string roomName,
            string senderName)
        {
            RoomId = roomId;
            TargetId = targetId;
            RoomName = roomName;
            SenderName = senderName;
            CreateDate = DateTime.Now;
            Type = NotificationTypes.Invitation;
        }

        protected Invitation() { }

        public virtual DateTime CreateDate { get; protected set; }
        public virtual string RoomName { get; protected set; }
        public virtual string SenderName { get; protected set; }
        public virtual uint InvitationId { get; protected set; }
        public virtual uint RoomId { get; protected set; }
        public virtual uint TargetId { get; protected set; }
    }
}