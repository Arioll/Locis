using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using UserMangment.Application;
using UserMangment.Domain;
using WebApplication1.Authorization;

namespace WebApplication1.Filters
{
    public class AuthenticationFilter : IAuthenticationFilter
    {
        public AuthenticationFilter(IAuthorization authorization)
        {
            _authorization = authorization;
        }

        private readonly IAuthorization _authorization;

        public bool AllowMultiple => true;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var tokenString = context?.Request?.Headers?.Authorization?.Parameter;
            if (string.IsNullOrEmpty(tokenString))
            {
                SetupUnauthenticated();
                return;
            }

            var tokenInfo = _authorization.GetTokenInfo(tokenString);
            if (tokenInfo == null)
            {
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[] {}, context.Request);
                await context.ErrorResult.ExecuteAsync(cancellationToken);
                SetupUnauthenticated();
                return;
            }

            var identity = new LocisIdentity(tokenInfo.UserId, true);
            var principal = new LocisPrincipal(identity, AuthStatus.Authorized);

            Thread.CurrentPrincipal = principal;
            context.Principal = principal;
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void SetupUnauthenticated()
        {
            Thread.CurrentPrincipal = LocisPrincipal.EmptyPrincipal;
        }
    }
}