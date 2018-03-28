using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Common.Entities;
using DataAccess.Application;
using DataAccess.NHibernate;
using Journalist;
using NHibernate;
using NHibernate.Linq;

namespace DataAccess.Repositories
{
    public class SongRepository : ISongRepository
    {
        public SongRepository(SessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        private readonly SessionProvider _sessionProvider;

        public uint SaveSong(Song song)
        {
            Require.NotNull(song, nameof(song));

            return (uint) Session.Save(song);
        }

        public Song GetSongById(uint songId)
        {
            Require.Positive(songId, nameof(songId));

            return Session.Get<Song>(songId);
        }

        public IEnumerable<Song> GetSongsByQuery(string query)
        {
            Require.NotEmpty(query, nameof(query));
            
            var regex = new Regex("[^0-9a-zA-Zа-яА-Я]");
            regex.Replace(query, "");
            var result = Session
                .Query<Song>()
                .Where(song => song.Title.ToLower().Contains(query.ToLower()) ||
                               song.FileName.ToLower().Contains(query.ToLower())).ToList();

            return result;
        }

        public void DeleteSong(Song song)
        {
            Require.NotNull(song, nameof(song));
            
            Session.Delete(song);
        }

        public IEnumerable<Song> GetSongsByIds(IEnumerable<uint> ids)
        {
            Require.NotNull(ids, nameof(ids));
            
            return ids.Select(id => Session.Get<Song>(id)).ToList();
        }

        public void UpdateSong(Song song)
        {
            Require.NotNull(song, nameof(song));
           
            Session.Update(song);
        }

        private ISession Session => _sessionProvider.GetCurrentSession();
    }
}