namespace WebApplication1.Models
{
    public class RoomProfile
    {
        public RoomProfile(string roomName, uint roomId, string avatarUri, uint countOfMembers)
        {
            RoomName = roomName;
            RoomId = roomId;
            AvatarUri = avatarUri;
            CountOfMembers = countOfMembers;
        }

        public string AvatarUri { get; }
        public string RoomName { get; }
        public uint RoomId { get; }
        public uint CountOfMembers { get; }
    }
}
