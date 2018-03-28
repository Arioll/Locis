using System;

namespace WebApplication1.Models
{
    public class InvitationProfile
    {
        public InvitationProfile(DateTime createDate, 
            string roomName, 
            string senderName, 
            uint invitationId)
        {
            CreateDate = createDate;
            RoomName = roomName;
            SenderName = senderName;
            InvitationId = invitationId;
        }

        public DateTime CreateDate { get; protected set; }
        public string RoomName { get; protected set; }
        public string SenderName { get; protected set; }
        public uint InvitationId { get; protected set; }
    }
}