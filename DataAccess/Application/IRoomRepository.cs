using System;
using System.Collections.Generic;
using Common.Entities;

namespace DataAccess.Application
{
    public interface IRoomRepository
    {
        uint CreateRoom(Room roomToCreate);
        IEnumerable<Room> GetRoomByName(string roomName);
        Room GetRoomById(uint roomId);
        void UpdateRoom(Room room);
        void DeleteRoom(Room roomToDelete);
        IEnumerable<Room> GetRoomsByIds(IEnumerable<uint> roomIds);
        IEnumerable<Room> GetRoomByPredicate(Func<Room, bool> predicate);
    }
}