using System;
using System.Collections.Generic;

namespace Common.Entities
{
    public class Room
    {
        public Room(string roomName)
        {
            RoomName = roomName;
        }

        protected Room() { }

        public virtual uint RoomId { get; set; }
        public virtual Uri AvatarUri { get; set; }
        public virtual string RoomName { get; protected set; }
        public virtual uint AdministratorId { get; set; }
        public virtual uint CurrentSongId { get; set; }
        public virtual DateTime CurrentSongStartTime { get; set; }
        public virtual ISet<User> UsersInRoom { get; set; }
        public virtual uint PlaylistId { get; set; }

        public override bool Equals(object obj)
        {
            return (obj as Room)?.RoomId == RoomId;
        }
    }
}