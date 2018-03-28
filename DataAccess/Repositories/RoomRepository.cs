using System;
using System.Collections.Generic;
using System.Linq;
using Common.Entities;
using DataAccess.Application;
using DataAccess.NHibernate;
using Journalist;
using NHibernate.Linq;

namespace DataAccess.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly SessionProvider _sessionProvider;

        public RoomRepository(SessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public uint CreateRoom(Room roomToCreate)
        {
            Require.NotNull(roomToCreate, nameof(roomToCreate));

            var session = _sessionProvider.GetCurrentSession();

            return (uint) session.Save(roomToCreate);
        }

        public IEnumerable<Room> GetRoomByPredicate(Func<Room, bool> predicate)
        {
            var session = _sessionProvider.GetCurrentSession();
            var response = predicate != null
                ? session.Query<Room>().Where(predicate)
                : session.Query<Room>();
            return response;
        }

        public IEnumerable<Room> GetRoomByName(string roomName)
        {
            Require.NotEmpty(roomName, nameof(roomName));

            var session = _sessionProvider.GetCurrentSession();
            var response = session.QueryOver<Room>().Where(room => room.RoomName == roomName).List();

            return response;
        }

        public void DeleteRoom(Room roomToDelete)
        {
            Require.NotNull(roomToDelete, nameof(roomToDelete));

            var session = _sessionProvider.GetCurrentSession();

            session.Delete(roomToDelete);
        }

        public void UpdateRoom(Room room)
        {
            Require.NotNull(room, nameof(room));

            var session = _sessionProvider.GetCurrentSession();
            session.Update(room);
        }

        public Room GetRoomById(uint roomId)
        {
            Require.Positive(roomId, nameof(roomId));

            var session = _sessionProvider.GetCurrentSession();
            var room =  session.Get<Room>(roomId);

            return room;
        }

        public IEnumerable<Room> GetRoomsByIds(IEnumerable<uint> roomIds)
        {
            Require.NotNull(roomIds, nameof(roomIds));

            var session = _sessionProvider.GetCurrentSession();

            return roomIds.Select(id => session.Get<Room>(id)).ToList();
        }
    }
}