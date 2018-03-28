using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common.Entities;
using PlaylistMangment.Application;
using RoomMangment.Application;
using SongMangment.Application;
using UserMangment.Application;
using UserMangment.Domain;
using WebApplication1.Authorization;
using WebApplication1.Extensions;

namespace WebApplication1.Controllers
{
    [Authorization(AuthStatus.Authorized)]
    public class RoomController : ApiController
    {
        public RoomController(IRoomManager roomManager, 
            IUserManager userManager,
            ISongManager songManager, 
            IPlaylistManager playlistManager)
        {
            _roomManager = roomManager;
            _userManager = userManager;
            _songManager = songManager;
            _playlistManager = playlistManager;
        }

        private readonly IUserManager _userManager;
        private readonly ISongManager _songManager;
        private readonly IRoomManager _roomManager;
        private readonly IPlaylistManager _playlistManager;

        [HttpPost]
        [Route("room")]
        public HttpResponseMessage CreateRoom([FromBody]string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomName))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Room name is null or empty");

            var creatorId = User.Identity.GetId();

            return Request.CreateResponse(HttpStatusCode.OK, _roomManager.CreateRoom(roomName, creatorId));
        }

        [HttpGet]
        [Route("room/my")]
        public HttpResponseMessage GetCurrentRoomForUser()
        {
            var userId = User.Identity.GetId();
            var user = _userManager.GetUserById(userId);
            return user.CurrentRoomId == 0
                ? Request.CreateResponse(HttpStatusCode.NotFound, "The user don't have a current room")
                : Request.CreateResponse(HttpStatusCode.OK,
                    user.Rooms.First(room => room.RoomId == user.CurrentRoomId).RoomToFullRoomProfile());
        }

        [HttpGet]
        [Route("room/{roomId}")]
        public HttpResponseMessage GetRoomById([FromUri]uint roomId)
        {
            if (roomId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, roomId + " id is invalid");
            if (_roomManager.GetRoomById(roomId) == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The room with specified id is not found");
            }
            var user = _userManager.GetUserById(User.Identity.GetId());
            if (user.Rooms.All(room => room.RoomId != roomId))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "You dont have an access to this room");
            }
            _userManager.ChangeUserCurrentRoom(user.UserId, roomId);
            return Request.CreateResponse(HttpStatusCode.OK, 
                    user.Rooms.First(room => room.RoomId == roomId).RoomToFullRoomProfile());
        }

        [HttpGet]
        [Route("room/my/playlist")]
        public HttpResponseMessage GetRoomsPlaylist()
        {
            var userId = User.Identity.GetId();
            var user = _userManager.GetUserById(userId);
            if (user.CurrentRoomId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user doesn't have a current room");
            }
            var room = _roomManager.GetRoomById(user.CurrentRoomId);
            var playlist = _playlistManager.GetPlaylistById(room.PlaylistId) ?? new Playlist();
            return Request.CreateResponse(HttpStatusCode.OK, playlist.List.SongsToSongProfiles());
        }

        [HttpGet]
        [Route("room/my/roommates")]
        public HttpResponseMessage GetMyRoomMates()
        {
            var userId = User.Identity.GetId();
            var user = _userManager.GetUserById(userId);
            if (user.CurrentRoomId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user doesn't have current room");
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                _userManager.GetUserRoommates(userId).UsersToUserProfiles());
        }

        [HttpPost]
        [Route("room/playing/{songId}")]
        public HttpResponseMessage StartPlayingSong([FromUri]uint songId)
        {
            if (songId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, songId + " id is invalid");

            var userId = User.Identity.GetId();
            var user = _userManager.GetUserById(userId);
            if (user.CurrentRoomId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "You don't have a current room");
            }
            var song = _songManager.GetSongById(songId);
            if (song == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The song is not exist");
            }

            _roomManager.StartPlayingSong(songId, user.CurrentRoomId);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Route("room/playlist/songs")]
        public HttpResponseMessage DeleteSongFromRoomPlaylist([FromBody] uint songId)
        {
            if (songId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, songId + " id is invalid");

            if (_songManager.GetSongById(songId) == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Specified song is not found");
            }
            var userId = User.Identity.GetId();
            if (_userManager.GetUserById(userId).CurrentRoomId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The user don't have a current room");
            }

            var roomId = _userManager.GetUserById(userId).CurrentRoomId;
            _playlistManager.RemoveSongFromPlaylist(_roomManager.GetRoomById(roomId).PlaylistId, songId);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}