using System;
using System.Collections.Generic;
using System.Linq;
using Common.Entities;
using DataAccess.Application;
using DataAccess.NHibernate;
using Journalist;
using NHibernate;
using NHibernate.Linq;

namespace DataAccess.Repositories
{
    public class InvitationRepository : IInvitationRepository
    {
        public InvitationRepository(SessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        private readonly SessionProvider _sessionProvider;

        public void UpdateInvitation(Invitation invitation)
        {
            Require.NotNull(invitation, nameof(invitation));

            Session.Update(invitation);
        }

        public Invitation GetInvitationById(uint invitationId)
        {
            Require.Positive(invitationId, nameof(invitationId));

            var response = Session.Get<Invitation>(invitationId);

            return response;
        }

        public IEnumerable<Invitation> GetInvitationByPredicate(Func<Invitation, bool> predicate)
        {
            return predicate == null
                ? Session.Query<Invitation>().ToList()
                : Session.Query<Invitation>().Where(predicate).ToList();
        }

        public void DeleteInvitation(Invitation invitation)
        {
            Require.NotNull(invitation, nameof(invitation));

            Session.Delete(invitation);
        }

        public IEnumerable<Invitation> GetInvitationByIds(IEnumerable<uint> ids)
        {
            Require.NotNull(ids, nameof(ids));

            return ids.Select(id => Session.Get<Invitation>(id)).ToList();
        }

        public uint CreateInvitation(Invitation invitationToCreate)
        {
            Require.NotNull(invitationToCreate, nameof(invitationToCreate));

            return (uint) Session.Save(invitationToCreate);
        }

        public IEnumerable<Invitation> GetAllUserInvitations(uint userId)
        {
            Require.Positive(userId, nameof(userId));

            return Session.Query<Invitation>().Where(inv => inv.TargetId == userId);
        }

        private ISession Session => _sessionProvider.GetCurrentSession();
    }
}