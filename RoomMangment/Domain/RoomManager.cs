using System;
using System.Collections.Generic;
using System.Linq;
using Common.Entities;
using DataAccess.Application;
using Journalist;
using RoomMangment.Application;

namespace RoomMangment.Domain
{
    public class RoomManager : IRoomManager
    {
        public RoomManager(IRoomRepository roomRrepository, 
            IUserRepository userRepository, 
            IPlaylistRepository playlistRepository)
        {
            _roomRepository = roomRrepository;
            _userRepository = userRepository;
            _playlistRepository = playlistRepository;
        }

        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPlaylistRepository _playlistRepository;

        public uint CreateRoom(string roomName, uint creatorId)
        {
            Require.NotEmpty(roomName, nameof(roomName));
            Require.Positive(creatorId, nameof(creatorId));

            var creator = _userRepository.GetUserById(creatorId);
            var room = new Room(roomName)
            {
                UsersInRoom = new HashSet<User> {creator},
                AdministratorId = creator.UserId,
                PlaylistId = _playlistRepository.CreatePlaylist(new Playlist())
            };

            creator.CurrentRoomId = _roomRepository.CreateRoom(room);
            creator.Rooms.Add(room);
            _userRepository.UpdateUser(creator);

            return creator.CurrentRoomId;
        }

        public Room GetRoomById(uint roomId)
        {
            Require.Positive(roomId, nameof(roomId));
            
            return _roomRepository.GetRoomById(roomId);
        }

        public IEnumerable<Room> GetRoomsByName(string roomName)
        {
            Require.NotEmpty(roomName, nameof(roomName));

            return _roomRepository.GetRoomByName(roomName);
        }

        public void AddUserInRoom(uint userId, uint roomId)
        {
            Require.Positive(userId, nameof(userId));
            Require.Positive(roomId, nameof(roomId));

            var room = _roomRepository.GetRoomById(roomId);
            var user = _userRepository.GetUserById(userId);

            if (room.UsersInRoom.Any(us => us.UserId == userId)) return;

            room.UsersInRoom.Add(user);
            _roomRepository.UpdateRoom(room);

            user.Rooms.Add(room);
            user.CurrentRoomId = roomId;

            _userRepository.UpdateUser(user);
        }

        public void DeleteUserFromRoom(uint userId, uint roomId)
        {
            Require.Positive(userId, nameof(userId));
            Require.Positive(roomId, nameof(roomId));

            var room = _roomRepository.GetRoomById(roomId);
            var user = _userRepository.GetUserById(userId);

            if (room.UsersInRoom.All(us => us.UserId != userId)) return;

            room.UsersInRoom.Remove(user);
            _roomRepository.UpdateRoom(room);

            user.Rooms.Remove(room);
            _userRepository.UpdateUser(user);

            if (room.UsersInRoom.ToList().Count == 0)
            {
                var playlist = _playlistRepository.GetPlaylistById(room.PlaylistId);
                if (playlist != null) _playlistRepository.DeletePlaylist(playlist);
                _roomRepository.DeleteRoom(room);
            }
            else
            {
                if (room.AdministratorId == userId)
                {
                    room.AdministratorId = room.UsersInRoom.First(us => us.UserId != userId).UserId;
                    _roomRepository.UpdateRoom(room);
                }
            }

            if (room.RoomId != user.CurrentRoomId) return;

            user.CurrentRoomId = 0;
            _userRepository.UpdateUser(user);
        }

        public IEnumerable<Room> GetRoomsByIds(IEnumerable<uint> ids)
        {
            Require.NotNull(ids, nameof(ids));

            return _roomRepository.GetRoomsByIds(ids);
        }

        public void ChangeRoomAdmin(uint newAdminId, uint roomId)
        {
            Require.Positive(newAdminId, nameof(newAdminId));

            var room = _roomRepository.GetRoomById(roomId);
            room.AdministratorId = newAdminId;
            _roomRepository.UpdateRoom(room);
        }

        public void StartPlayingSong(uint songId, uint roomId)
        {
            Require.Positive(songId, nameof(songId));
            Require.Positive(roomId, nameof(roomId));

            var room = _roomRepository.GetRoomById(roomId);
            room.CurrentSongId = songId;
            room.CurrentSongStartTime = DateTime.Now;
            _roomRepository.UpdateRoom(room);
        }
    }
}