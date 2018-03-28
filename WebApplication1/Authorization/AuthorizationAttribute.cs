using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using UserMangment.Domain;

namespace WebApplication1.Authorization
{
    public class AuthorizationAttribute : AuthorizeAttribute
    {
        public AuthorizationAttribute(AuthStatus status)
        {
            _status = status;
        }

        private readonly AuthStatus _status;

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return Thread.CurrentPrincipal.IsInRole(_status);
        }
    }
}