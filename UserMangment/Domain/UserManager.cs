using System;
using System.Collections.Generic;
using System.Linq;
using Common.Entities;
using Common.Entities.AbstractEntities;
using DataAccess.Application;
using Journalist;
using RoomMangment.Application;
using UserMangment.Application;

namespace UserMangment.Domain
{
    public class UserManager : IUserManager
    {
        public UserManager(IUserRepository userRepository, 
            IRoomRepository roomRepository,
            IAuthorization authorization,
            IRoomManager roomManager,
            IInvitationRepository invitationRepository, 
            IPlaylistRepository playlistRepository, 
            IAddToFriendRequestRepository addToFriendRequestRepository)
        {
            _userRepository = userRepository;
            _roomRepository = roomRepository;
            _authorization = authorization;
            _roomManager = roomManager;
            _invitationRepository = invitationRepository;
            _playlistRepository = playlistRepository;
            _addToFriendRequestRepository = addToFriendRequestRepository;
        }

        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IAuthorization _authorization;
        private readonly IRoomManager _roomManager;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IAddToFriendRequestRepository _addToFriendRequestRepository;

        public uint CreateUser(User userToCreate)
        {
            Require.NotNull(userToCreate, nameof(userToCreate));
            userToCreate.PlaylistId = _playlistRepository.CreatePlaylist(new Playlist());
            return _userRepository.CreateUser(userToCreate);
        }

        public IEnumerable<User> SearshUsersByName(string userName)
        {
            Require.NotEmpty(userName, nameof(userName));

            return _userRepository.GetUserByName(userName);
        }

        public bool CheckUserEmail(string userEmail)
        {
            Require.NotEmpty(userEmail, nameof(userEmail));

            return _userRepository.GetUsersByPredicate(x => x.Email.Address == userEmail).Any();
        }

        public void DeleteUser(uint userId)
        {
            Require.Positive(userId, nameof(userId));

            var user = _userRepository.GetUserById(userId);
            foreach (var room in user.Rooms)
            {
                _roomManager.DeleteUserFromRoom(user.UserId, room.RoomId);
            }

            foreach (var invitation in user.Invaitations)
            {
                _invitationRepository.DeleteInvitation(invitation);
            }

            var usersRequests = _addToFriendRequestRepository
                .GetRequestsByPredicate(request => request.SenderId == user.UserId);
            foreach (var request in usersRequests)
            {
                _addToFriendRequestRepository.DeleteRequest(request);
            }
            
            foreach (var friend in user.Friends)
            {
                friend.Friends.Remove(user);
                _userRepository.UpdateUser(friend);
            }
            user.Friends = new HashSet<User>();
            _userRepository.UpdateUser(user);

            var playlist = _playlistRepository.GetPlaylistById(user.PlaylistId);
            playlist.Clear();
            _playlistRepository.UpdatePlaylist(playlist);

            user.IsDeleted = true;
            _userRepository.UpdateUser(user);
            _authorization.CancelTokenById(userId);
        }

        public bool CheckUserNickname(string userNickname)
        {
            Require.NotEmpty(userNickname, nameof(userNickname));

            return _userRepository.GetUsersByPredicate(user => user.Nickname == userNickname).Any();
        }

        public IEnumerable<User> GetUserRoommates(uint userId)
        {
            Require.Positive(userId, nameof(userId));

            var user = _userRepository.GetUserById(userId);
            var room = _roomRepository.GetRoomById(user.CurrentRoomId);
            return room.UsersInRoom.Where(us => us.UserId != userId);
        }

        public IEnumerable<User> GetUserFriends(uint userId)
        {
            Require.Positive(userId, nameof(userId));

            return _userRepository.GetUserById(userId).Friends;
        }

        public User GetUserById(uint userId)
        {
            Require.Positive(userId, nameof(userId));

            return _userRepository.GetUserById(userId);
        }

        public User GetUserByEmail(string email)
        {
            Require.NotEmpty(email, nameof(email));

            return _userRepository.GetUsersByPredicate(user => user.Email.Address == email).FirstOrDefault();
        }

        public void ChangeUserPassword(string newPass, uint userId)
        {
            Require.NotEmpty(newPass, nameof(newPass));
            Require.Positive(userId, nameof(userId));

            var user = _userRepository.GetUserById(userId);
            user.SetPassword(newPass);
            _userRepository.UpdateUser(user);
        }

        public void ChangeUserEmail(string newEmail, uint userId)
        {
            Require.NotEmpty(newEmail, nameof(newEmail));
            Require.Positive(userId, nameof(userId));

            var user = _userRepository.GetUserById(userId);
            user.SetEmail(newEmail);
            _userRepository.UpdateUser(user);
        }

        public void ChangeUserNickname(string newNickname, uint userId)
        {
            Require.NotEmpty(newNickname, nameof(newNickname));
            Require.Positive(userId, nameof(userId));

            var user = _userRepository.GetUserById(userId);
            user.SetNickname(newNickname);
            _userRepository.UpdateUser(user);
        }

        public void ChangeUserAvatarUri(Uri newAvatarUri, uint userId)
        {
            Require.NotNull(newAvatarUri, nameof(newAvatarUri));
            Require.Positive(userId, nameof(userId));

            var user = _userRepository.GetUserById(userId);
            user.SetAvatarUri(newAvatarUri);
            _userRepository.UpdateUser(user);
        }

        public uint SendRequestToAddNewFriend(uint userId, uint friendId)
        {
            Require.Positive(userId, nameof(userId));
            Require.Positive(friendId, nameof(friendId));

            var userName = _userRepository.GetUserById(userId).Nickname;
            var request = new AddToFriendRequest(userId, friendId, userName);
            var id = _addToFriendRequestRepository.SaveRequest(request);
            var friend = _userRepository.GetUserById(friendId);
            friend.AddToFriendRequests.Add(_addToFriendRequestRepository.GetRequestById(id));
            _userRepository.UpdateUser(friend);
            return id;
        }

        public void ResponseToRequest(uint requestId, bool response)
        {
            Require.Positive(requestId, nameof(requestId));

            var request = _addToFriendRequestRepository.GetRequestById(requestId);
            if (response)
            {
                AddNewFriend(request.SenderId, request.TargetId);
            }
            var user = _userRepository.GetUserById(request.TargetId);
            user.AddToFriendRequests.Remove(request);
            _userRepository.UpdateUser(user);
            _addToFriendRequestRepository.DeleteRequest(request);
        }

        private void AddNewFriend(uint userId, uint friendId)
        {
            Require.Positive(userId, nameof(userId));
            Require.Positive(friendId, nameof(friendId));

            var user = _userRepository.GetUserById(userId);
            var friend = _userRepository.GetUserById(friendId);
            user.Friends.Add(friend);
            _userRepository.UpdateUser(user);
            friend.Friends.Add(user);
            _userRepository.UpdateUser(friend);
        }

        public IEnumerable<Notification> GetAllUserNotification(uint userId)
        {
            Require.Positive(userId, nameof(userId));
            
            return new List<Notification>()
                .Concat(_addToFriendRequestRepository.GetAllUserRequests(userId))
                .Concat(_invitationRepository.GetAllUserInvitations(userId));
        }

        public void DeleteFriend(uint userId, uint friendId)
        {
            Require.Positive(userId, nameof(userId));
            Require.Positive(friendId, nameof(friendId));

            var user = _userRepository.GetUserById(userId);
            var friend = _userRepository.GetUserById(friendId);
            if (user.Friends.Any(x => x.UserId == friendId))
                user.Friends.Remove(user.Friends.First(x => x.UserId == friendId));
            if (friend.Friends.Any(x => x.UserId == userId))
                friend.Friends.Remove(user);
            _userRepository.UpdateUser(user);
        }

        public void ChangeUserCurrentRoom(uint userId, uint newCurrentRoomId)
        {
            Require.Positive(userId, nameof(userId));
            Require.Positive(newCurrentRoomId, nameof(newCurrentRoomId));

            var user = _userRepository.GetUserById(userId);
            user.CurrentRoomId = newCurrentRoomId;
            _userRepository.UpdateUser(user);
        }
    }
}