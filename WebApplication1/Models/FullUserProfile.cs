namespace WebApplication1.Models
{
    public class FullUserProfile
    {
        public FullUserProfile(
            string nickname,
            uint currentRoomId,
            string avatarPicture = null)
        {
            Nickname = nickname;
            CurrentRoomId = currentRoomId;
            AvatarPicture = avatarPicture;
        }

        public string Nickname { get; private set; }
        public uint CurrentRoomId { get; private set; }
        public string AvatarPicture { get; private set; }
    }
}