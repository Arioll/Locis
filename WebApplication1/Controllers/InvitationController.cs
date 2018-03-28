using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvaitationMangment.Application;
using UserMangment.Application;
using UserMangment.Domain;
using WebApplication1.Authorization;

namespace WebApplication1.Controllers
{
    [Authorization(AuthStatus.Authorized)]
    public class InvitationController : ApiController
    {
        public InvitationController(
            IInvitationManager invitationManager,
            IUserManager userManager)
        {
            _invitationManager = invitationManager;
            _userManager = userManager;
        }

        private readonly IInvitationManager _invitationManager;
        private readonly IUserManager _userManager;

        [HttpPut]
        [Route("invitation")]
        public HttpResponseMessage InvateUserInRoom([FromBody]uint userId)
        {
            if (userId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Id is invalid");

            var userTo = _userManager.GetUserById(userId);
            if (userTo == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user specified in the invitation does not exist");
            }
            var user = _userManager.GetUserById(User.Identity.GetId());
            if (user.CurrentRoomId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user connot invite users because he doesn't have a current room");
            }

            _invitationManager.InviteUserInRoom(userId, user.CurrentRoomId, user.UserId);
            
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPut]
        [Route("invitation/{invitationId}/response")]
        public HttpResponseMessage ResponseToInvitation([FromUri]uint invitationId, [FromBody]bool isAccepted)
        {
            if (invitationId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invitation id is invalid");

            var invitation = _invitationManager.GetInvitationById(invitationId);
            if (invitation == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The invitation is not found");
            }
            var userId = User.Identity.GetId();
            if (userId != invitation.TargetId)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "The user can not send response to that invitation");
            }
            
            _invitationManager.ResponseToInvitation(invitationId, isAccepted);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}