using System;
using System.Collections.Generic;
using Common.Entities;
using Common.Entities.AbstractEntities;

namespace UserMangment.Application
{
    public interface IUserManager
    {
        IEnumerable<User> GetUserRoommates(uint userId);
        IEnumerable<User> GetUserFriends(uint userId);
        IEnumerable<User> SearshUsersByName(string userName);
        IEnumerable<Notification> GetAllUserNotification(uint userId);
        User GetUserById(uint userId);
        User GetUserByEmail(string email);
        void DeleteUser(uint userId);
        void ChangeUserCurrentRoom(uint userId, uint newCurrentRoomId);
        void ChangeUserPassword(string newPass, uint userId);
        void ChangeUserEmail(string newEmail, uint userId);
        void ChangeUserNickname(string newNickname, uint userId);
        void ChangeUserAvatarUri(Uri newAvatarUri, uint userId);
        uint CreateUser(User userToCreate);
        bool CheckUserEmail(string userEmail);
        bool CheckUserNickname(string userNickname);
        uint SendRequestToAddNewFriend(uint userId, uint friendId);
        void ResponseToRequest(uint requestId, bool response);
        void DeleteFriend(uint userId, uint friendId);
    }
}