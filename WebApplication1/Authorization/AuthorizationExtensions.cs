using System.Security.Principal;
using UserMangment.Domain;

namespace WebApplication1.Authorization
{
    public static class AuthorizationExtensions
    {
        public static bool IsInRole(this IPrincipal principal, AuthStatus status)
        {
            var locisPrincipal = principal as LocisPrincipal;
            return locisPrincipal?.IsInRole(status) ?? false;
        }

        public static uint GetId(this IIdentity identity)
        {
            var locisIdentity = identity as LocisIdentity;
            return locisIdentity.UserId;
        }
    }
}