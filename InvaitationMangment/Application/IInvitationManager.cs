using System.Collections.Generic;
using Common.Entities;

namespace InvaitationMangment.Application
{
    public interface IInvitationManager
    {
        Invitation GetInvitationById(uint invitationId);
        IEnumerable<Invitation> GetInvitationsByIds(IEnumerable<uint> ids);
        uint InviteUserInRoom(uint userId, uint roomId, uint senderId);
        void DeleteInvitation(uint invitationId);
        void ResponseToInvitation(uint invitationId, bool response);
    }
}
