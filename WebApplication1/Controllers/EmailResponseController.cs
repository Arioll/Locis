using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UserMangment.Domain.EmailOperations.Application;

namespace WebApplication1.Controllers
{
    public class EmailResponseController : ApiController
    {
        public EmailResponseController(IEmailConfirmation emailConfirmation)
        {
            _emailConfirmation = emailConfirmation;
        }

        private readonly IEmailConfirmation _emailConfirmation;

        [HttpGet]
        [Route("email/{guid}")]
        public HttpResponseMessage ConfirmEmail([FromUri] string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid message guid format");

            if (!_emailConfirmation.CheckGuidCurrently(guid))
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Uncorrect link key");

            _emailConfirmation.ConfirmUserEmailToRegistration(guid);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("password/{guid}")]
        public HttpResponseMessage ChangePasswordMessage([FromUri] string guid, [FromBody] string newPassword)
        {
            if (string.IsNullOrWhiteSpace(guid) || string.IsNullOrWhiteSpace(newPassword))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "invalid new password or guid");

            if (!_emailConfirmation.CheckGuidCurrently(guid))
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Uncorrect link key");
            try
            {
                _emailConfirmation.RecoverUserPassword(guid, newPassword);
            }
            catch (ArgumentException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Uncorrect password");
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("newEmail/{guid}")]
        public HttpResponseMessage ConfirmChangeEmailRequest([FromUri] string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Guid is invalid");

            if (!_emailConfirmation.CheckGuidCurrently(guid))
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Uncorrect link key");

            _emailConfirmation.ConfirmUserEmailToChange(guid);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
