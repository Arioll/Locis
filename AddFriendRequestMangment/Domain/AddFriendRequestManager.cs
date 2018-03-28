using AddFriendRequestMangment.Application;
using Common.Entities;
using DataAccess.Application;
using Journalist;

namespace AddFriendRequestMangment.Domain
{
    public class AddFriendRequestManager : IAddFriendRequestManager
    {
        public AddFriendRequestManager(IAddToFriendRequestRepository addToFriendRequestRepository)
        {
            _addToFriendRequestRepository = addToFriendRequestRepository;
        }

        private readonly IAddToFriendRequestRepository _addToFriendRequestRepository;

        public AddToFriendRequest GetRequestById(uint requestId)
        {
            Require.Positive(requestId, nameof(requestId));

            return _addToFriendRequestRepository.GetRequestById(requestId);
        }
    }
}
