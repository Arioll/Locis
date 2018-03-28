using Common.Entities;

namespace AddFriendRequestMangment.Application
{
    public interface IAddFriendRequestManager
    {
        AddToFriendRequest GetRequestById(uint requestId);
    }
}
