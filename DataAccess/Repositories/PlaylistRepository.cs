using Common.Entities;
using DataAccess.Application;
using DataAccess.NHibernate;
using Journalist;

namespace DataAccess.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        public PlaylistRepository(SessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        private readonly SessionProvider _sessionProvider;

        public uint CreatePlaylist(Playlist playlistToCreate)
        {
            Require.NotNull(playlistToCreate, nameof(playlistToCreate));

            var session = _sessionProvider.GetCurrentSession();

            return (uint) session.Save(playlistToCreate);
        }

        public Playlist GetPlaylistById(uint id)
        {
            Require.Positive(id, nameof(id));

            var session = _sessionProvider.GetCurrentSession();

            return session.Get<Playlist>(id);
        }

        public void UpdatePlaylist(Playlist playlistToUpdate)
        {
            Require.NotNull(playlistToUpdate, nameof(playlistToUpdate));

            var session = _sessionProvider.GetCurrentSession();

            session.Update(playlistToUpdate);
        }

        public void DeletePlaylist(Playlist playlistToDelete)
        {
            Require.NotNull(playlistToDelete, nameof(playlistToDelete));

            var session = _sessionProvider.GetCurrentSession();

            session.Delete(playlistToDelete);
        }
    }
}
