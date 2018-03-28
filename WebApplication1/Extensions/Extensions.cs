using System.Collections.Generic;
using System.Linq;
using Common.Entities;
using WebApplication1.Models;

namespace WebApplication1.Extensions
{
    public static class Extensions
    {
        public static IEnumerable<SongProfile> SongsToSongProfiles(this IEnumerable<Song> songs)
        {
            return
                songs.Select(
                    song =>
                        new SongProfile(song.SongId, song.Title, song.Performers, song.Genres, song.Length, song.Album));
        }

        public static IEnumerable<UserProfile> UsersToUserProfiles(this IEnumerable<User> users)
        {
            return users.Select(user => new UserProfile(user.UserId, user.Nickname, user.AvatarPicture?.AbsoluteUri ?? null));
        }

        public static IEnumerable<RoomProfile> RoomsToRoomProfiles(this IEnumerable<Room> rooms)
        {
            return rooms.Select(room => new RoomProfile(room.RoomName, room.RoomId, room.AvatarUri?.AbsoluteUri ?? null, (uint)room.UsersInRoom.Count));
        }

        public static FullRoomProfile RoomToFullRoomProfile(this Room room)
        {
            return new FullRoomProfile(room.RoomId, 
                room.UsersInRoom.UsersToUserProfiles(), 
                room.AvatarUri?.AbsoluteUri ?? null, 
                room.RoomName, 
                room.PlaylistId, 
                room.CurrentSongStartTime, 
                room.CurrentSongId, 
                room.AdministratorId);
        }
    }
}