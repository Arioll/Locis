using System;
using System.Collections.Generic;
using Common.Entities;

namespace DataAccess.Application
{
    public interface IAddToFriendRequestRepository
    {
        IEnumerable<AddToFriendRequest> GetRequestsByIds(IEnumerable<uint> ids);
        IEnumerable<AddToFriendRequest> GetRequestsByPredicate(Func<AddToFriendRequest, bool> predicate);
        IEnumerable<AddToFriendRequest> GetAllUserRequests(uint userId);
        AddToFriendRequest GetRequestById(uint id);
        void DeleteRequest(AddToFriendRequest request);
        uint SaveRequest(AddToFriendRequest requestToSave);
    }
}
