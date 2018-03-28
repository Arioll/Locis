using System.Collections.Generic;
using Common.Entities;

namespace DataAccess.Application
{
    public interface ISongRepository
    {
        IEnumerable<Song> GetSongsByQuery(string query);
        Song GetSongById(uint songId);
        uint SaveSong(Song song);
        void DeleteSong(Song song);
        IEnumerable<Song> GetSongsByIds(IEnumerable<uint> ids);
        void UpdateSong(Song song);
    }
}