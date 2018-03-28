using System.Security.Principal;
using Journalist;
using UserMangment.Domain;

namespace WebApplication1.Authorization
{
    public class LocisPrincipal : IPrincipal
    {
        public LocisPrincipal(IIdentity locisIdentity, AuthStatus status)
        {
            Require.NotNull(locisIdentity, nameof(locisIdentity));

            _status = status;
            Identity = locisIdentity;
        }

        public bool IsEmpty { get; private set; }

        private readonly AuthStatus _status;

        public static IPrincipal EmptyPrincipal => new LocisPrincipal(LocisIdentity.EmptyIdentity, AuthStatus.UnAuthorized) {IsEmpty = true};

        public bool IsInRole(string role)
        {
            return !IsEmpty && _status.ToString("G").Equals(role);
        }

        public bool IsInRole(AuthStatus status)
        {
            return _status == status && !IsEmpty;
        }

        public IIdentity Identity { get; }
    }
}