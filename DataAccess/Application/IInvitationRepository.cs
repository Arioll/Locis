using System;
using System.Collections.Generic;
using Common.Entities;

namespace DataAccess.Application
{
    public interface IInvitationRepository
    {
        IEnumerable<Invitation> GetInvitationByPredicate(Func<Invitation, bool> predicate);
        IEnumerable<Invitation> GetInvitationByIds(IEnumerable<uint> ids);
        IEnumerable<Invitation> GetAllUserInvitations(uint userId);
        Invitation GetInvitationById(uint invitationId);
        void UpdateInvitation(Invitation invitation);
        void DeleteInvitation(Invitation invitation);
        uint CreateInvitation(Invitation invitationToCreate);
    }
}