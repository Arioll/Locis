using Common.Entities;

namespace PlaylistMangment.Application
{
    public interface IPlaylistManager
    {
        Playlist GetPlaylistById(uint id);
        void AddSongsInUsersPlaylist(uint[] songIds, uint userId);
        void UploadUsersPlaylistToCurrentRoom(uint userId);
        void RemoveSongFromPlaylist(uint playlistId, uint songId);
    }
}
