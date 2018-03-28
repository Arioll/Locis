using System.Security.Principal;

namespace WebApplication1.Authorization
{
    public class LocisIdentity : IIdentity
    {
        public LocisIdentity(uint userId, bool isAuthenticated)
        {
            UserId = userId;
            IsAuthenticated = isAuthenticated;
        }

        public static LocisIdentity EmptyIdentity => new LocisIdentity(0, false);

        public uint UserId { get; }

        public string Name => UserId.ToString();

        public string AuthenticationType => "Token";

        public bool IsAuthenticated { get; }
    }
}