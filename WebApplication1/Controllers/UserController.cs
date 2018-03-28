using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using AddFriendRequestMangment.Application;
using Common.Entities;
using UserMangment.Application;
using PlaylistMangment.Application;
using RoomMangment.Application;
using SongMangment.Application;
using UserMangment.Domain;
using UserMangment.Domain.EmailOperations.Application;
using WebApplication1.Authorization;
using WebApplication1.Extensions;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : ApiController
    {
        public UserController(IUserManager userManager,
            IRoomManager roomManager,
            ISongManager songManager,
            IEmailConfirmation emailConfirmation,
            IPlaylistManager playlistManager, 
            IAddFriendRequestManager addFriendRequestManager)
        {
            _userManager = userManager;
            _roomManager = roomManager;
            _songManager = songManager;
            _emailConfirmation = emailConfirmation;
            _playlistManager = playlistManager;
            _addFriendRequestManager = addFriendRequestManager;
        }

        private readonly IUserManager _userManager;
        private readonly IRoomManager _roomManager;
        private readonly ISongManager _songManager;
        private readonly IEmailConfirmation _emailConfirmation;
        private readonly IPlaylistManager _playlistManager;
        private readonly IAddFriendRequestManager _addFriendRequestManager;

        [HttpGet]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/me")]
        public HttpResponseMessage GetProfileOfCurrentUser()
        {
            var userId = User.Identity.GetId();
            var user =  _userManager.GetUserById(userId);
            var avatarUri = user.AvatarPicture != null ? user.AvatarPicture.AbsoluteUri : null;
            var profile = new FullUserProfile(user.Nickname, user.CurrentRoomId, avatarUri);

            return Request.CreateResponse(HttpStatusCode.OK, profile);
        }

        [HttpGet]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/songs")]
        public HttpResponseMessage GetMyPlaylist()
        {
            var userId = User.Identity.GetId();
            var user = _userManager.GetUserById(userId);
            var playlist = _playlistManager.GetPlaylistById(user.PlaylistId);

            return Request.CreateResponse(HttpStatusCode.OK, playlist.List.SongsToSongProfiles());
        }

        [HttpGet]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/rooms")]
        public HttpResponseMessage GetMyRooms()
        {
            var userId = User.Identity.GetId();
            var user = _userManager.GetUserById(userId);

            return Request.CreateResponse(HttpStatusCode.OK, user.Rooms.RoomsToRoomProfiles());
        }

        [HttpGet]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/notifications")]
        public HttpResponseMessage GetMyNotifications()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _userManager.GetAllUserNotification(User.Identity.GetId()));
        }

        [HttpGet]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/friends")]
        public HttpResponseMessage GetUserFriends()
        {
            return Request.CreateResponse
                (HttpStatusCode.OK, _userManager.GetUserFriends(User.Identity.GetId()).UsersToUserProfiles());
        }

        [HttpGet]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/songs/current")]
        public HttpResponseMessage GetCurrentSong()
        {
            var userId = User.Identity.GetId();
            var user = _userManager.GetUserById(userId);
            if (user.CurrentRoomId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "The user doesn't have a current room");
            }
            var room = _roomManager.GetRoomById(user.CurrentRoomId);
            var info = new InfoToUserInRoom
            {
                CurrentSongId = room.CurrentSongId,
                CurrentSongStartTime = room.CurrentSongStartTime
            };
            return Request.CreateResponse(HttpStatusCode.OK, info);
        }

        [HttpPut]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/songs/upload")]
        public HttpResponseMessage UploadMyPlaylistInCurrentRoom()
        {
            var userId = User.Identity.GetId();
            var user = _userManager.GetUserById(userId);
            if (user.CurrentRoomId == 0)
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user doesn't have a current room");
            _playlistManager.UploadUsersPlaylistToCurrentRoom(userId);
            
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPut]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/songs")]
        public HttpResponseMessage AddSongInMyPlaylist([FromBody]uint[] songIds)
        {
            if (songIds == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Song ids is equals to null");

            if (songIds.Select(id => _songManager.GetSongById(id)).Any(song => song == null))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Song or songs from array is are not found");
            }

            _playlistManager.AddSongsInUsersPlaylist(songIds, User.Identity.GetId());
            
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/songs")]
        public HttpResponseMessage DeleteSongFromUserPlaylist([FromBody]uint songId)
        {
            if (songId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, songId + " is invalid");

            if (_songManager.GetSongById(songId) == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The song is not found");
            }

            _playlistManager.RemoveSongFromPlaylist(_userManager.GetUserById(User.Identity.GetId()).PlaylistId, songId);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Authorization(AuthStatus.Authorized)]
        [Route("user")]
        public HttpResponseMessage DeleteUserAccount()
        {
            var userId = User.Identity.GetId();
            _userManager.DeleteUser(userId);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/room/{roomId}/exit")]
        public HttpResponseMessage ExitFromRoom([FromUri]uint roomId)
        {
            if (roomId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, roomId + "is invalid");

            var userId = User.Identity.GetId();
            var room = _roomManager.GetRoomById(roomId);
            if (room == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The room is not found");
            }
            
            _roomManager.DeleteUserFromRoom(userId, roomId);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPut]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/room/{roomId}/admin/{newAdminId}")]
        public HttpResponseMessage AssignNewRoomAdmin([FromUri]uint newAdminId, [FromUri]uint roomId)
        {
            if (newAdminId <= 0 || roomId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "New Admin id or room id is invalid");

            var userId = User.Identity.GetId();
            var user = _userManager.GetUserById(userId);
            var room = _roomManager.GetRoomById(user.CurrentRoomId);
            if (room.AdministratorId != userId)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, 
                    "You can't assign new admin to the room since you are not admin");
            }
            var newAdmin = _userManager.GetUserById(newAdminId);
            if (newAdmin == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user is not found");
            }

            _roomManager.ChangeRoomAdmin(newAdminId, roomId);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/expel")]
        public HttpResponseMessage ExpelUserFromCurrentRoom([FromBody]uint userToExpelId)
        {
            if (userToExpelId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, userToExpelId + " id is invalid");

            var userId = User.Identity.GetId();
            var user = _userManager.GetUserById(userId);
            if (user.CurrentRoomId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user doesn't have a current room");
            }
            var room = _roomManager.GetRoomById(user.CurrentRoomId);
            var userToExpel = _userManager.GetUserById(userToExpelId);
            if (userToExpel == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user is not found");
            }
            if (User.Identity.GetId() != room.AdministratorId)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Only room administrator can expell the user");
            }
            if (User.Identity.GetId() == userToExpelId)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Room creator can not expel room creator from room");
            }
            
            _roomManager.DeleteUserFromRoom(userToExpelId, user.CurrentRoomId);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("user")]
        public HttpResponseMessage SendConfirmationMessageToUser([FromBody]CreateUserRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            if (request.Nickname == null | request.Email == null | request.Password == "")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    $"Uncorrect user data: Nickname {request.Nickname}, Email: {request.Email}, Password: {request.Password}");
            }
            if (_userManager.CheckUserEmail(request.Email))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "User with this email does already exist");
            }
            if (_userManager.CheckUserNickname(request.Nickname))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "User with this nickname does already exist");
            }
            Password pass;
            try
            {
                pass = new Password(request.Password);
            }
            catch (ArgumentException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Uncorrect password");
            }
            MailAddress mail;
            try
            {
                mail = new MailAddress(request.Email);
            }
            catch (FormatException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Uncorrect email");
            }

            var userToConfirmation = new User(request.Nickname, mail, pass);

            _emailConfirmation.SendEmailToConfirmation(userToConfirmation);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("user/password")]
        public HttpResponseMessage SendRestorePassMessageToUser([FromBody]string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "User Email is null or empty");

            var user = _userManager.GetUserByEmail(userEmail);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user with this email doesn't found");
            }
            _emailConfirmation.SendEmailToRestorePassword(user.UserId);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/email")]
        public HttpResponseMessage ChangeUserEmail([FromBody]string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "New email in null or empty");
            try
            {
                new MailAddress(newEmail);
            }
            catch (FormatException)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Uncorrect email");
            }
            if (_userManager.GetUserById(User.Identity.GetId()).Email.Address == newEmail)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            if (_userManager.CheckUserEmail(newEmail))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "User with this email is already exist");
            }

            _emailConfirmation.SendMessageToChangeEmail(User.Identity.GetId(), newEmail);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPut]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/nickname")]
        public HttpResponseMessage ChangeUserNickname([FromBody]string newNickname)
        {
            if (string.IsNullOrWhiteSpace(newNickname))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "New nickname is empty or null");
            
            var user = _userManager.GetUserById(User.Identity.GetId());
            if (user.Nickname == newNickname)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            if (_userManager.CheckUserNickname(newNickname))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "User with this nickname is already exist");
            }

            _userManager.ChangeUserNickname(newNickname, user.UserId);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPut]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/avatar")]
        public HttpResponseMessage ChangeAvatarUri([FromBody]string newAvatarUri)
        {
            if (string.IsNullOrWhiteSpace(newAvatarUri))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Avatar uri string id null or empty");

            var user = _userManager.GetUserById(User.Identity.GetId());
            Uri newAvatar;
            try
            {
                newAvatar = new Uri(newAvatarUri);
            }
            catch (UriFormatException)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Uncorrect uri format");
            }
            if (user.AvatarPicture.AbsoluteUri == newAvatarUri)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            user.SetAvatarUri(newAvatar);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPut]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/friends/new")]
        public HttpResponseMessage AddNewFriendToUser([FromBody]uint friendId)
        {
            if (friendId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, friendId + "is invalid");

            if (User.Identity.GetId() == friendId)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "You cannot send request to yourself");
            }
            var friend = _userManager.GetUserById(friendId);
            if (friend == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user from request is not found");
            }
            if (friend.Friends.Any(us => us.UserId == User.Identity.GetId()))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            var requests = friend.AddToFriendRequests;
            if (requests.Any(request => request.SenderId == User.Identity.GetId()))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            _userManager.SendRequestToAddNewFriend(User.Identity.GetId(), friendId);
            
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPut]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/friends/requests/{requestId}/response")]
        public HttpResponseMessage ResponseToAddInFriendsRequest([FromBody]bool answer, [FromUri]uint requestId)
        {
            if (requestId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, requestId + " is invalid");

            var request = _addFriendRequestManager.GetRequestById(requestId);
            if (request == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The request is not found");
            }
            if (request.TargetId != User.Identity.GetId())
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "You can not answer to this request");
            }

            _userManager.ResponseToRequest(requestId, answer);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/friends/{friendId}")]
        public HttpResponseMessage DeleteFromFriends([FromUri]uint friendId)
        {
            if (friendId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, friendId + " is invalid");
            var userId = User.Identity.GetId();
            var user = _userManager.GetUserById(userId);
            var friend = _userManager.GetUserById(friendId);
            if (friend == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user with specific id is not found");
            }
            if (user.Friends.All(us => us.UserId != friendId))
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            
            _userManager.DeleteFriend(userId, friendId);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        [Authorization(AuthStatus.Authorized)]
        [Route("user/search/{searchString}")]
        public HttpResponseMessage SearchUsersByNickname([FromUri]string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Empty or null search string");

            var profiles = _userManager.SearshUsersByName(searchString).Where(x => x.UserId != User.Identity.GetId());
            
            return Request.CreateResponse(HttpStatusCode.OK, profiles.UsersToUserProfiles());
        }
        
        [HttpPost]
        [Route("user/testcreation")]
        public HttpResponseMessage CreateTestUser([FromBody]CreateUserRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            if (request.Nickname == null | request.Email == null | request.Password == "")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    $"Uncorrect user data: Nickname {request.Nickname}, Email: {request.Email}, Password: {request.Password}");
            }
            if (_userManager.GetUserByEmail(request.Email) != null)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "User with this email is already exist");
            }
            Password pass;
            try
            {
                pass = new Password(request.Password);
            }
            catch (ArgumentException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Uncorrect password");
            }
            MailAddress mail;
            try
            {
                mail = new MailAddress(request.Email);
            }
            catch (FormatException)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Uncorrect email");
            }

            var user = new User(request.Nickname, mail, pass);
            _userManager.CreateUser(user);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}