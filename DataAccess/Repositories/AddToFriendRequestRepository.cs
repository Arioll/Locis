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
    public class AddToFriendRequestRepository : IAddToFriendRequestRepository
    {
        public AddToFriendRequestRepository(SessionProvider sessionProvider1)
        {
            _sessionProvider = sessionProvider1;
        }

        private readonly SessionProvider _sessionProvider;

        public IEnumerable<AddToFriendRequest> GetRequestsByIds(IEnumerable<uint> ids)
        {
            Require.NotNull(ids, nameof(ids));

            return ids.Select(id => Session.Get<AddToFriendRequest>(id));
        }

        public void DeleteRequest(AddToFriendRequest request)
        {
            Require.NotNull(request, nameof(request));

            Session.Delete(request);
        }

        public AddToFriendRequest GetRequestById(uint id)
        {
            Require.Positive(id, nameof(id));

            return Session.Get<AddToFriendRequest>(id);
        }

        public uint SaveRequest(AddToFriendRequest requestToSave)
        {
            Require.NotNull(requestToSave, nameof(requestToSave));

            return (uint) Session.Save(requestToSave);
        }

        public IEnumerable<AddToFriendRequest> GetRequestsByPredicate(Func<AddToFriendRequest, bool> predicate)
        {
            return predicate == null
                ? Session.Query<AddToFriendRequest>()
                : Session.Query<AddToFriendRequest>().Where(predicate);
        }

        public IEnumerable<AddToFriendRequest> GetAllUserRequests(uint userId)
        {
            Require.Positive(userId, nameof(userId));

            return Session.Query<AddToFriendRequest>().Where(request => request.TargetId == userId);
        }

        private ISession Session => _sessionProvider.GetCurrentSession();
    }
}
