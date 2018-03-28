using Common.Entities;

namespace DataAccess.Application
{
    public interface IPlaylistRepository
    {
        uint CreatePlaylist(Playlist playlistToCreate);
        Playlist GetPlaylistById(uint id);
        void UpdatePlaylist(Playlist playlistToUpdate);
        void DeletePlaylist(Playlist playlistToDelete);
    }
}
