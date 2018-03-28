using Common.Entities;
using System;
using System.Net;
using System.Net.Http;
using UserMangment.Domain.Authorization.Exceptions;
using System.Web.Http;
using UserMangment.Application;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AuthorizationController : ApiController
    {
        public AuthorizationController(IAuthorization authorization)
        {
            _authorization = authorization;
        }

        private readonly IAuthorization _authorization;

        [HttpPost]
        [Route("login")]
        public HttpResponseMessage Authorization([FromBody]AuthorizationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            try
            {
                var token = _authorization.Autorize(request.Email, new Password(request.Password));
                return Request.CreateResponse(HttpStatusCode.OK, token.Token);
            }
            catch (ArgumentException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Uncorrect password");
            }
            catch (UserNotFoundException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user with this email or password is not found");
            }
            catch (UnauthorizedAccessException)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unathorized Access");
            }
        }
    }
}