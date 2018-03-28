using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class FullRoomProfile
    {
        public FullRoomProfile(uint roomId, 
            IEnumerable<UserProfile> users, 
            string avatarUri, 
            string roomName, 
            uint playlistId, 
            DateTime currentSongStartTime, 
            uint currentSongId, 
            uint administratorId)
        {
            RoomId = roomId;
            Users = users;
            AvatarUri = avatarUri;
            RoomName = roomName;
            PlaylistId = playlistId;
            CurrentSongStartTime = currentSongStartTime;
            CurrentSongId = currentSongId;
            AdministratorId = administratorId;
        }

        public uint RoomId { get; }
        public IEnumerable<UserProfile> Users { get; }
        public string AvatarUri { get; }
        public string RoomName { get; }
        public uint AdministratorId { get; }
        public virtual uint CurrentSongId { get; }
        public virtual DateTime CurrentSongStartTime { get; }
        public uint PlaylistId { get; }
    }
}