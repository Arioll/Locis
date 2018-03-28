using System.Collections.Generic;
using Common.Entities;

namespace RoomMangment.Application
{
    public interface IRoomManager
    {
        uint CreateRoom(string roomName, uint creatorId);
        Room GetRoomById(uint roomId);
        IEnumerable<Room> GetRoomsByName(string roomName);
        void AddUserInRoom(uint userId, uint roomId);
        void DeleteUserFromRoom(uint userId, uint roomId);
        void ChangeRoomAdmin(uint newAdminId, uint roomId);
        IEnumerable<Room> GetRoomsByIds(IEnumerable<uint> ids);
        void StartPlayingSong(uint songId, uint roomId);
    }
}