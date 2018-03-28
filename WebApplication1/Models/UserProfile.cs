namespace WebApplication1.Models
{
    public class UserProfile
    {
        public UserProfile(uint userId, string nickname, string avatarPictureUri)
        {
            UserId = userId;
            Nickname = nickname;
            AvatarPictureUri = avatarPictureUri;
        }

        public uint UserId { get; set; }
        public string Nickname { get; set; }
        public string AvatarPictureUri { get; set; }
    }
}